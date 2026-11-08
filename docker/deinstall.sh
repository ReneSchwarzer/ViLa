#!/usr/bin/env bash
# Removes the ViLa Docker Compose deployment and configuration while preserving application data.
#
# Usage: sudo bash deinstall.sh
#        curl -fsSL https://raw.githubusercontent.com/ReneSchwarzer/ViLa/Develop/docker/deinstall.sh | sudo bash
# Options (environment variables, all optional):
#   VILA_DIR           installation directory (default: /opt/vila)
set -euo pipefail

# Writes a progress message to standard output.
log() { printf '\033[1;32m==>\033[0m %s\n' "$*"; }

# Writes an error message to standard error and exits with a failure status.
die() { printf '\033[1;31mError:\033[0m %s\n' "$*" >&2; exit 1; }

# Removes the deployment after Bash has read the complete function from standard input.
main() {

VILA_DIR="${VILA_DIR:-/opt/vila}"
log "Checking ViLa installation in $VILA_DIR"

[ "$(id -u)" -eq 0 ] || die "Run this script as root (e.g. sudo bash deinstall.sh)"
[ -d "$VILA_DIR" ] || die "Installation directory does not exist: $VILA_DIR"
cd "$VILA_DIR"
VILA_DIR="$(pwd -P)"
[ -f docker-compose.yml ] || die "No docker-compose.yml found in $VILA_DIR. Set VILA_DIR to the installation directory."
command -v docker >/dev/null 2>&1 || die "Docker is not installed."
docker compose version >/dev/null 2>&1 || die "The Docker Compose plugin is missing."

log "Stopping and removing the ViLa Compose deployment and its local images"
docker compose -f docker-compose.yml down --rmi all \
	|| die "Docker Compose could not remove the deployment. Check the Docker service and configuration."

log "Removing docker-compose.yml and .env"
rm -f -- docker-compose.yml .env \
	|| die "Could not remove docker-compose.yml and .env. Check the directory permissions."

cat <<EOF

ViLa has been stopped and its Compose deployment and local service images removed.

The docker-compose.yml and .env files have been removed.
Application settings, data, assets, packages, logs and volumes have been preserved.
Docker remains installed.
Installation directory: $VILA_DIR
EOF

}

main "$@"
