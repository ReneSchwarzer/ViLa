#!/usr/bin/env bash
# Installs ViLa as a Docker container on a Raspberry Pi (64 bit Raspberry Pi OS / Debian / Ubuntu).
#
# The script
#   - installs Docker Engine with the Compose plugin if missing,
#   - creates the installation and data directories with the correct owner and permissions,
#   - creates default WebExpress and ViLa settings files if missing,
#   - detects the GPIO device and its group,
#   - generates a signing key for new installations and updates the public address in .env,
#   - writes a docker-compose.yml that uses the published image (no local build),
#   - pulls the image and starts the container.
#
# Usage:   curl -fsSL https://raw.githubusercontent.com/ReneSchwarzer/ViLa/Develop/docker/install.sh | sudo bash
#          or: sudo ./install.sh
# Options (environment variables, all optional):
#   VILA_DIR           installation directory               (default: /opt/vila)
#   VILA_IMAGE         image to run                         (default: rschwarzer/vila:2.0.1)
#   VILA_PORT          host port                            (default: 80)
#   VILA_EXTERNAL_URI  public base URI                      (default: http://<hostname>:<port>/)
#   VILA_GPIO_CHIP     GPIO chip number                     (default: auto, 4 on Raspberry Pi 5, otherwise 0)
#   VILA_UID           numeric user id of the container user (default: 1654, the "app" user of .NET images)
#   VILA_SKIP_DOCKER_INSTALL=1   do not try to install Docker
set -euo pipefail

# Everything is wrapped in a function so that bash reads the complete script before running it.
# This makes "curl ... | sudo bash" safe (no commands can consume the script from stdin).
main() {

VILA_DIR="${VILA_DIR:-/opt/vila}"
VILA_IMAGE="${VILA_IMAGE:-rschwarzer/vila:2.0.1}"
VILA_PORT="${VILA_PORT:-80}"
VILA_UID="${VILA_UID:-1654}"

log()  { printf '\033[1;32m==>\033[0m %s\n' "$*"; }
warn() { printf '\033[1;33mWarning:\033[0m %s\n' "$*" >&2; }
die()  { printf '\033[1;31mError:\033[0m %s\n' "$*" >&2; exit 1; }

# --- checks -----------------------------------------------------------------------------------

[ "$(id -u)" -eq 0 ] || die "Run this script as root (e.g. curl -fsSL <url> | sudo bash)"

case "$(uname -m)" in
	aarch64|arm64|x86_64) ;;
	*) die "Unsupported architecture $(uname -m). A 64 bit operating system is required." ;;
esac

# --- Docker -----------------------------------------------------------------------------------

if ! command -v docker >/dev/null 2>&1; then
	[ "${VILA_SKIP_DOCKER_INSTALL:-0}" = "1" ] && die "Docker is not installed."
	command -v curl >/dev/null 2>&1 || { apt-get update && apt-get install -y --no-install-recommends curl ca-certificates; }
	log "Installing Docker Engine"
	curl -fsSL https://get.docker.com | sh
fi

docker compose version >/dev/null 2>&1 \
	|| die "The Docker Compose plugin is missing. Install it with: apt-get install docker-compose-plugin"

systemctl enable --now docker >/dev/null 2>&1 || true

# --- directories and permissions --------------------------------------------------------------

log "Creating directories in $VILA_DIR"
mkdir -p "$VILA_DIR"/{data,settings,assets,packages,log}
mkdir -p "$VILA_DIR/data/measurements" "$VILA_DIR/data/archive"
VILA_DIR="$(cd "$VILA_DIR" && pwd -P)"
chown root:root "$VILA_DIR"
chown -R "$VILA_UID:$VILA_UID" "$VILA_DIR"/{data,settings,assets,packages,log}
chmod 750 "$VILA_DIR"/{data,settings,assets,packages,log}
chmod 750 "$VILA_DIR/data/measurements" "$VILA_DIR/data/archive"
chmod 755 "$VILA_DIR"

# --- default settings -------------------------------------------------------------------------

WEBEXPRESS_SETTINGS_FILE="$VILA_DIR/settings/webexpress.settings.json"
if [ -e "$WEBEXPRESS_SETTINGS_FILE" ]; then
	log "Keeping existing $WEBEXPRESS_SETTINGS_FILE"
else
	log "Writing $WEBEXPRESS_SETTINGS_FILE"
	cat > "$WEBEXPRESS_SETTINGS_FILE" <<'EOF'
{
  "WebExpress": {
    "Endpoints": [
      { "Uri": "http://0.0.0.0:8080/" }
    ],
    "ExternalUri": "http://localhost:8080/",
    "Culture": "de-DE",
    "PackagePath": "./packages",
    "AssetPath": "./assets",
    "DataPath": "./data",
    "ContextPath": ""
  }
}
EOF
	chown "$VILA_UID:$VILA_UID" "$WEBEXPRESS_SETTINGS_FILE"
	chmod 640 "$WEBEXPRESS_SETTINGS_FILE"
fi

VILA_SETTINGS_FILE="$VILA_DIR/settings/vila.settings.json"
if [ -e "$VILA_SETTINGS_FILE" ]; then
	log "Keeping existing $VILA_SETTINGS_FILE"
else
	log "Writing $VILA_SETTINGS_FILE"
	cat > "$VILA_SETTINGS_FILE" <<'EOF'
{
  "Plugins": {
    "vila": {
      "DebugMode": false,
      "ImpulsePerkWh": 500,
      "ElectricityPricePerkWh": 0.28,
      "MaxWattage": 40,
      "MinWattage": 0,
      "MaxChargingTime": 24,
      "Currency": "€",
      "BillingDayOffset": 0,
      "Mode": "ManuallyControlled"
    }
  }
}
EOF
	chown "$VILA_UID:$VILA_UID" "$VILA_SETTINGS_FILE"
	chmod 640 "$VILA_SETTINGS_FILE"
fi

# --- GPIO -------------------------------------------------------------------------------------

if [ -z "${VILA_GPIO_CHIP:-}" ]; then
	if grep -qa "Raspberry Pi 5" /proc/device-tree/model 2>/dev/null && [ -e /dev/gpiochip4 ]; then
		VILA_GPIO_CHIP=4
	else
		VILA_GPIO_CHIP=0
	fi
fi

GPIO_DEVICE="/dev/gpiochip${VILA_GPIO_CHIP}"
if [ -e "$GPIO_DEVICE" ]; then
	VILA_GPIO_GID="$(stat -c '%g' "$GPIO_DEVICE")"
	log "GPIO device $GPIO_DEVICE found (group id $VILA_GPIO_GID)"
else
	warn "$GPIO_DEVICE does not exist. Check the GPIO chip with: ls /dev/gpiochip*"
	warn "Set VILA_GPIO_CHIP and run the script again. Continuing with group id 0, the container will not start without the device."
	VILA_GPIO_GID=0
fi

# --- .env -------------------------------------------------------------------------------------

ENV_FILE="$VILA_DIR/.env"
PUBLIC_HOST="$(hostname)"
if [ "$VILA_PORT" = "80" ]; then
	EXTERNAL_URI="${VILA_EXTERNAL_URI:-http://${PUBLIC_HOST}/}"
else
	EXTERNAL_URI="${VILA_EXTERNAL_URI:-http://${PUBLIC_HOST}:${VILA_PORT}/}"
fi
[[ "$VILA_PORT" =~ ^[0-9]+$ ]] && [ "${#VILA_PORT}" -le 5 ] \
	&& [ "$((10#$VILA_PORT))" -ge 1 ] && [ "$((10#$VILA_PORT))" -le 65535 ] \
	|| die "VILA_PORT must be a number between 1 and 65535."
[[ "$EXTERNAL_URI" != *$'\n'* && "$EXTERNAL_URI" != *$'\r'* ]] \
	|| die "VILA_EXTERNAL_URI must not contain line breaks."

if [ -f "$ENV_FILE" ]; then
	log "Updating public address in $ENV_FILE while preserving the signing key and other settings"
	ENV_TEMP="$(mktemp "$VILA_DIR/.env.XXXXXX")"
	if ! VILA_INSTALL_PORT="$VILA_PORT" VILA_INSTALL_URI="$EXTERNAL_URI" awk '
		!/^[[:space:]]*(export[[:space:]]+)?VILA_(PORT|EXTERNAL_URI)[[:space:]]*=/ { print }
		END {
			print "VILA_PORT=" ENVIRON["VILA_INSTALL_PORT"]
			print "VILA_EXTERNAL_URI=" ENVIRON["VILA_INSTALL_URI"]
		}
	' "$ENV_FILE" > "$ENV_TEMP"; then
		rm -f -- "$ENV_TEMP"
		die "Could not update $ENV_FILE."
	fi
	mv -f -- "$ENV_TEMP" "$ENV_FILE"
else
	log "Writing $ENV_FILE"
	if command -v openssl >/dev/null 2>&1; then
		SIGNING_KEY="$(openssl rand -base64 32)"
	else
		SIGNING_KEY="$(head -c 32 /dev/urandom | base64)"
	fi

	umask 077
	cat > "$ENV_FILE" <<EOF
# Generated by install.sh
VILA_IMAGE=$VILA_IMAGE
VILA_SIGNING_KEY=$SIGNING_KEY
VILA_GPIO_CHIP=$VILA_GPIO_CHIP
VILA_GPIO_GID=$VILA_GPIO_GID
VILA_PORT=$VILA_PORT
VILA_EXTERNAL_URI=$EXTERNAL_URI
EOF
	umask 022
fi
chmod 600 "$ENV_FILE"
chown root:root "$ENV_FILE"

# --- docker-compose.yml -----------------------------------------------------------------------

COMPOSE_FILE="$VILA_DIR/docker-compose.yml"
log "Writing $COMPOSE_FILE"
cat > "$COMPOSE_FILE" <<'EOF'
services:
  vila:
    hostname: vila
    container_name: vila
    image: ${VILA_IMAGE:-rschwarzer/vila:2.0.1}
    ports:
      - "${VILA_PORT:-80}:8080"
    environment:
      WEBEXPRESS_WebExpress__Endpoints__0__Uri: http://0.0.0.0:8080/
      WEBEXPRESS_WebExpress__ExternalUri: ${VILA_EXTERNAL_URI:-http://localhost:${VILA_PORT:-80}/}
      WEBEXPRESS_WebExpress__Authentication__SigningKey: ${VILA_SIGNING_KEY:?set VILA_SIGNING_KEY in .env}
      VILA_GPIO_CHIP: "${VILA_GPIO_CHIP:-0}"
    devices:
      - "/dev/gpiochip${VILA_GPIO_CHIP:-0}:/dev/gpiochip${VILA_GPIO_CHIP:-0}:rw"
    group_add:
      - "${VILA_GPIO_GID:?set VILA_GPIO_GID to the numeric group of the GPIO device}"
    volumes:
      - ./data:/app/data
      - ./settings:/app/settings
      - ./assets:/app/assets
      - ./packages:/app/packages
      - ./log:/app/log
    restart: unless-stopped
    stop_grace_period: 45s
EOF
chown root:root "$COMPOSE_FILE"
chmod 644 "$COMPOSE_FILE"

# --- start ------------------------------------------------------------------------------------

cd "$VILA_DIR"
log "Pulling $VILA_IMAGE"
docker compose pull

log "Starting ViLa"
docker compose up -d

sleep 3
docker compose ps

cat <<EOF

ViLa is installed in $VILA_DIR.

  Web interface:  ${EXTERNAL_URI%/}/vila/
  Logs:           sudo docker compose -f $COMPOSE_FILE logs -f
  Update:         cd $VILA_DIR && sudo docker compose pull && sudo docker compose up -d
  Stop:           cd $VILA_DIR && sudo docker compose down

Check the impulse factor, tariff and operating mode before use, and verify the S0 readings
and contactor operation on the installed hardware.
EOF

}

main "$@"
