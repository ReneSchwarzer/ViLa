# Docker

This directory contains the helper files for the ViLa container build. The `Dockerfile` and `docker-compose.yml` are located in the repository root.

## Contents

|File           |Purpose
|---------------|-----------------------------------------------------------------------------
|`nuget.config` |NuGet configuration for the build inside the container. It points to the local WebExpress feed (`/nuget`).
|`publish.ps1`  |Builds the image for `linux/amd64` and `linux/arm64` and pushes it as a single tag (PowerShell).
|`publish.sh`   |Same as the PowerShell script, for Linux and macOS (Bash).
|`install.sh`   |Installs ViLa as a container on a Raspberry Pi (directories, permissions, GPIO, `.env`, Compose).
|`deinstall.sh` |Removes the container deployment, local service images, Compose file and environment file while preserving application data.

## Prerequisites

- Docker Engine with the Buildx plugin (included in Docker Desktop)
WebExpress `2.0.1-alpha` packages
- A Docker login with write access to the repository (`docker login`)

The feed path is provided through the `WEBEXPRESS_FEED` environment variable. Without it, the script uses `%USERPROFILE%\Documents\.nuget`.

## Build and run locally

```sh
docker compose up -d --build
```

`WEBEXPRESS_FEED`, `VILA_SIGNING_KEY` and `VILA_GPIO_GID` must be set (see `.env.example`).

Persistent storage uses bind mounts under `/opt/vila` by default. Set `VILA_DIR` to an absolute host path to use another directory. Before starting Compose directly, create the `data/measurements`, `data/archive`, `settings`, `assets`, `packages` and `log` subdirectories and grant the container user with UID `1654` write access. Existing named Docker volumes are not migrated automatically; copy their contents into the corresponding directories while the application is stopped before switching to bind mounts.

## Install on a Raspberry Pi

Install it directly with curl (as root):

```sh
curl -fsSL https://raw.githubusercontent.com/ReneSchwarzer/ViLa/Develop/docker/install.sh | sudo bash
```

With options: `curl -fsSL <url> | sudo VILA_PORT=8090 bash`. Alternatively copy the script to the Pi and run it as root:

```sh
chmod +x install.sh
sudo ./install.sh
```

The script:

- installs Docker Engine and the Compose plugin if they are missing,
- creates `data, settings, assets, packages, log` under `/opt/vila` by default (`VILA_DIR`), owned by UID 1654 (the `app` user of the .NET image) with mode `750`,
- detects the GPIO device (`/dev/gpiochip4` on a Raspberry Pi 5, otherwise `/dev/gpiochip0`) and its group id,
- writes `./.env` with a generated signing key (mode `600`; an existing file is kept),
- writes `./docker-compose.yml`, pulls the image and starts the container.

Optional environment variables: `VILA_DIR`, `VILA_IMAGE`, `VILA_PORT`, `VILA_EXTERNAL_URI`, `VILA_GPIO_CHIP`, `VILA_UID`, `VILA_SKIP_DOCKER_INSTALL=1`.

The installation directory defaults to `/opt/vila` regardless of the working directory. Its owner is `root` with mode `755`. The `docker-compose.yml` and `.env` files belong to `root` with modes `644` and `600`, respectively. All application files remain together beneath this directory, while container paths remain under `/app`. For an existing installation elsewhere, pass its directory through `VILA_DIR`; the installer does not move existing data.

Default settings are created in `settings/webexpress.settings.json` and `settings/vila.settings.json` when the files do not exist. Existing files are preserved. New files are owned by `VILA_UID` with mode `640`, allowing the application to save configuration changes. WebExpress listens on container port `8080`, and Compose supplies the public URI and signing key through environment variables. ViLa starts in `ManuallyControlled` mode with the application defaults.

```sh
sudo VILA_PORT=8090 VILA_GPIO_CHIP=0 ./install.sh
```

Afterwards the web interface is available at `http://<hostname>/vila/`. Update with `cd <install directory> && docker compose pull && docker compose up -d`.

The default host port is `80`, mapped to container port `8080`. Existing `.env` files are preserved by the installer. To change an existing installation, set `VILA_PORT=80` and adjust `VILA_EXTERNAL_URI` to `http://<hostname>/` in `/opt/vila/.env`, then run `sudo docker compose -f /opt/vila/docker-compose.yml up -d` to apply the new port mapping.

## Uninstall on a Raspberry Pi

The uninstaller stops and removes the Compose deployment, its network and local service images using `docker compose down --rmi all`. After successful removal, it deletes `docker-compose.yml` and `.env` from the installation directory. It preserves application settings, measurement data, assets, packages, logs and Docker volumes. Docker remains installed.

The uninstaller targets `/opt/vila` by default and can be run from any working directory:

```sh
sudo bash deinstall.sh
```

For a custom installation directory, use the same `VILA_DIR` value used during installation:

```sh
sudo VILA_DIR=/path/to/vila bash deinstall.sh
```

## Publish a multi-architecture image

A plain `docker build` or the Visual Studio container build only produces an image for the host platform. To publish one tag for Raspberry Pi (ARM64) and amd64:

```powershell
./docker/publish.ps1 -Version 2.0.0
```

If PowerShell reports that script execution is disabled, run the script with a process-scoped bypass. This does not change the system policy:

```powershell
powershell -ExecutionPolicy Bypass -File ./docker/publish.ps1 -Version 2.0.0
```

Alternatively, allow local scripts permanently for your user with `Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned`. If Windows marks the file as downloaded, run `Unblock-File ./docker/publish.ps1`.

On Linux or macOS use the Bash script. It takes positional arguments `[version] [suffix] [repository]` and reads the feed from `WEBEXPRESS_FEED`:

```sh
chmod +x docker/publish.sh
WEBEXPRESS_FEED=/path/to/feed ./docker/publish.sh 2.0.0
```

Parameters of the PowerShell script:

|Parameter     |Default                |Description
|--------------|-----------------------|-----------------------------------------
|`-Version`    |`2.0.0`                |Version number of the tag
|`-Suffix`     |                       |Tag suffix (leave empty for a release without suffix)
|`-Repository` |`rschwarzer/vila`      |Image name in the registry
|`-Feed`       |`$env:WEBEXPRESS_FEED` |Path to the local NuGet feed

If needed, the script creates the `multiarch` builder (driver `docker-container`), because the default builder cannot produce multi-platform images. It then pushes the image and prints the manifest.

Verify the result:

The default image tag is `rschwarzer/vila:2.0.0` without a prerelease suffix. Existing installations retain their `.env` file. Set `VILA_IMAGE=rschwarzer/vila:2.0.0` in `/opt/vila/.env`, then run `sudo docker compose -f /opt/vila/docker-compose.yml pull` and `sudo docker compose -f /opt/vila/docker-compose.yml up -d` to switch an existing installation after the image has been published.

```sh
docker buildx imagetools inspect rschwarzer/vila:2.0.0
```

The manifest must list `linux/amd64` and `linux/arm64`. The `unknown/unknown` entry is only the attestation manifest and not a separate image.

## Notes

- On an amd64 host the arm64 part runs under emulation (QEMU), so the build takes longer. On a plain Linux host, run `docker run --privileged --rm tonistiigi/binfmt --install arm64` first if necessary.
- The container name (for example in `launchSettings.json`) must not contain a slash. The image name `rschwarzer/vila` is not affected.
- GPIO access can only be tested on real hardware. Set `VILA_GPIO_CHIP` and `VILA_GPIO_GID` for it.
