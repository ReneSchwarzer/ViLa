# Docker

This directory contains the helper files for the ViLa container build. The `Dockerfile` and `docker-compose.yml` are located in the repository root.

## Contents

| File | Purpose |
| --- | --- |
| `nuget.config` | NuGet configuration for the build inside the container. It points to the local WebExpress feed (`/nuget`). |
| `publish.ps1` | Builds the image for `linux/amd64` and `linux/arm64` and pushes it as a single tag (PowerShell). |
| `publish.sh` | Same as the PowerShell script, for Linux and macOS (Bash). |
| `install.sh` | Installs ViLa as a container on a Raspberry Pi (directories, permissions, GPIO, `.env`, Compose). |

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
- creates `/opt/vila/{data,settings,assets,packages,log}` owned by UID 1654 (the `app` user of the .NET image) with mode `750`,
- detects the GPIO device (`/dev/gpiochip4` on a Raspberry Pi 5, otherwise `/dev/gpiochip0`) and its group id,
- writes `/opt/vila/.env` with a generated signing key (mode `600`; an existing file is kept),
- writes `/opt/vila/docker-compose.yml`, pulls the image and starts the container.

Optional environment variables: `VILA_DIR`, `VILA_IMAGE`, `VILA_PORT`, `VILA_EXTERNAL_URI`, `VILA_GPIO_CHIP`, `VILA_UID`, `VILA_SKIP_DOCKER_INSTALL=1`.

```sh
sudo VILA_PORT=8090 VILA_GPIO_CHIP=0 ./install.sh
```

Afterwards the web interface is available at `http://<hostname>:8080/vila/`. Update with `cd /opt/vila && docker compose pull && docker compose up -d`.

## Publish a multi-architecture image

A plain `docker build` or the Visual Studio container build only produces an image for the host platform. To publish one tag for Raspberry Pi (ARM64) and amd64:

```powershell
./docker/publish.ps1 -Version 2.0.0 -Suffix alpha
```

If PowerShell reports that script execution is disabled, run the script with a process-scoped bypass. This does not change the system policy:

```powershell
powershell -ExecutionPolicy Bypass -File ./docker/publish.ps1 -Version 2.0.0 -Suffix alpha
```

Alternatively, allow local scripts permanently for your user with `Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned`. If Windows marks the file as downloaded, run `Unblock-File ./docker/publish.ps1`.

On Linux or macOS use the Bash script. It takes positional arguments `[version] [suffix] [repository]` and reads the feed from `WEBEXPRESS_FEED`:

```sh
chmod +x docker/publish.sh
WEBEXPRESS_FEED=/path/to/feed ./docker/publish.sh 2.0.0 alpha
```

Parameters of the PowerShell script:

| Parameter | Default | Description |
| --- | --- | --- |
| `-Version` | `2.0.0` | Version number of the tag |
| `-Suffix` | `alpha` | Tag suffix (leave empty for a release without suffix) |
| `-Repository` | `rschwarzer/vila` | Image name in the registry |
| `-Feed` | `$env:WEBEXPRESS_FEED` | Path to the local NuGet feed |

If needed, the script creates the `multiarch` builder (driver `docker-container`), because the default builder cannot produce multi-platform images. It then pushes the image and prints the manifest.

Verify the result:

```sh
docker buildx imagetools inspect rschwarzer/vila:2.0.0-alpha
```

The manifest must list `linux/amd64` and `linux/arm64`. The `unknown/unknown` entry is only the attestation manifest and not a separate image.

## Notes

- On an amd64 host the arm64 part runs under emulation (QEMU), so the build takes longer. On a plain Linux host, run `docker run --privileged --rm tonistiigi/binfmt --install arm64` first if necessary.
- The container name (for example in `launchSettings.json`) must not contain a slash. The image name `rschwarzer/vila` is not affected.
- GPIO access can only be tested on real hardware. Set `VILA_GPIO_CHIP` and `VILA_GPIO_GID` for it.
