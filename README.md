# ViLa

ViLa monitors a wallbox through an S0 electricity meter and a GPIO contactor. The application runs on .NET 10 and uses WebExpress WebCore, WebUI, WebIndex, and WebApp `2.0.0-alpha`.

## Features

The dashboard displays current power, session energy, session cost, duration, and a live power chart. It refreshes every five seconds, retains the last successful values during a connection failure, and reconnects automatically. Responsive metric cards and chart colors follow the WebExpress theme.

The consumption overview shows twelve billing months and the number of completed sessions in the current billing period. Historical summaries use stored consumption and cost values, preserve separate currencies, and match complete space or semicolon separated tags. The history page provides the existing search, filtering, pagination, and detail views.

The vehicle catalog manages names, maximum AC charging power, automatic recognition, and availability. Each completed measurement supports manual assignment or recognition from a unique power profile. Assigned vehicles appear on dashboard session cards, in history, and in measurement details. The [vehicle management guide](doc/vehicles.md) explains recognition requirements and persistence.

The hardware integration is intended for a Raspberry Pi with a compatible S0 meter and contactor circuit. Multi-tenant billing and complete access control remain outside the implemented feature set.

## Build and run

Development requires the .NET 10 SDK. Run these commands from the repository root:

```powershell
dotnet build src/ViLa.slnx
dotnet run --project src/ViLa.App/ViLa.App.csproj --no-build
```

The default address is [http://localhost:8080/vila/](http://localhost:8080/vila/). The host configuration is copied from `src/ViLa.App/settings/webexpress.settings.json`. The obsolete `config/webexpress.config.xml` is no longer used by WebExpress 2.0.

Deployment settings define the listener in `WebExpress:Endpoints:0:Uri` and the public address in `WebExpress:ExternalUri`. Set both values when changing the port or using a reverse proxy. Environment overrides use the `WEBEXPRESS_` prefix, for example:

```powershell
$env:WEBEXPRESS_WebExpress__Endpoints__0__Uri = 'http://localhost:8096/'
$env:WEBEXPRESS_WebExpress__ExternalUri = 'http://localhost:8096/'
dotnet run --project src/ViLa.App/ViLa.App.csproj --no-build
```

Measurement configuration is stored in `settings/vila.settings.json` under `Plugins:vila`, using the settings directory selected by the WebExpress host. Operating modes are named strings such as `ManuallyControlled`. The shared WebExpress configuration loader applies overrides from `webexpress.settings.json` and environment variables such as `WEBEXPRESS_Plugins__vila__ImpulsePerkWh`. Verify the meter impulse factor, tariff, currency, and operating mode for the installation. Existing measurement files are read from `data/measurements` beside the executable when that directory exists, otherwise from the legacy `measurements` directory.

Legacy configuration is migrated from `Config/vila.settings.xml`, with a fallback to `vila.settings.xml` beside the executable, when the JSON settings file is absent. The XML source is retained. Existing JSON takes precedence, so remove the shipped default JSON file before the first start when migrating an installation with customized XML settings. Invalid JSON is not replaced with legacy values.

## Visual Studio container profile

Open `src/ViLa.slnx`, select `ViLa.App` as the startup project and choose **Container (Dockerfile)** in the launch menu. With Visual Studio Container Tools and a Linux Docker engine running, press F5 to open `http://localhost:8080/vila/`.

Visual Studio uses the local WebExpress feed in `%USERPROFILE%\Documents\.nuget` if present. For a different feed, set `WEBEXPRESS_FEED` before starting Visual Studio. For hardware debugging, also set `VILA_GPIO_CHIP` and `VILA_GPIO_GID` as described below when the Docker host exposes the device. The desktop profile can start without GPIO hardware.

## Docker on a Raspberry Pi

Run Docker Engine with the Compose plugin on a Raspberry Pi running 64 bit Linux. The build uses .NET 10 and a local NuGet feed containing the four WebExpress `2.0.0-alpha` packages listed above. Set `WEBEXPRESS_FEED` to the absolute path of that directory on the build host.

Identify the GPIO chip connected to the header with `gpioinfo` on the Pi. ViLa uses line 3 for the S0 input and line 13 for the contactor output. For example, if both lines belong to `gpiochip0`, inspect its permissions and prepare the environment file from the repository root.

```sh
ls -l /dev/gpiochip0
stat -c '%g' /dev/gpiochip0
cp .env.example .env
openssl rand -base64 32
```

Set `VILA_GPIO_CHIP` to the chip number and `VILA_GPIO_GID` to the numeric group printed by `stat`. The device must grant that group read and write access. Compose passes the selected device and group to the application user. Set `VILA_SIGNING_KEY` to the generated key and `VILA_EXTERNAL_URI` to the public base address, for example `http://raspberrypi.local:8080/`.

Start the service and inspect its status with the following commands.

```sh
docker compose up -d --build
docker compose ps
```

Open `/vila/` at the configured base address. The HTTP health check verifies the web host. To verify GPIO initialization, open the ViLa log and check for the GPIO initialization entry. Verify S0 readings and contactor operation on the installed hardware.

Docker volumes retain measurements and vehicles in `/app/data`, JSON settings in `/app/settings`, generated assets in `/app/assets` and installed plugins in `/app/packages`. The existing `/app/Config` volume remains mounted for legacy migration. On first start the settings volume receives the JSON files shipped with the application. Check the impulse factor, tariff and operating mode before use.

## Data import and export

The settings page **Data import and export** downloads a versioned ZIP backup containing saved measurements, sampled readings, comments, descriptions, tags, vehicle assignments, vehicle profiles and current application settings. The two download options exclude or include archived measurements. An ongoing charging session is not persisted yet and is excluded. Finish the session before exporting if its data must be included.

Backup format version 2 stores settings in `settings.json`, vehicle profiles in `vehicles.json`, and measurement records as XML. Version 1 backups containing `settings.xml` remain importable and are converted to the local JSON settings format when no existing settings are present. The Backup entry in the application settings menu opens this page.

The import adds missing records by their stable IDs and preserves existing settings, vehicles and measurements, including IDs already present in the archive or an ongoing session. Imported history and vehicles become available immediately. Repeating an import does not duplicate records. Archived records retain their archive location. The entire backup is validated before writing; unsupported versions, invalid records, duplicate identities and unexpected paths are rejected. ZIP uploads and expanded content are limited to 256 MiB and 100,000 entries. Storage failures can leave some new records imported, so the page reports this condition and permits a safe retry.

## Validation

The automated checks run with the following commands. The JavaScript tests require Node.js and use its built-in test runner without additional packages.

```powershell
dotnet test src/ViLa.Test/ViLa.Test.csproj
node --test src/ViLa.Test/JavaScript/dashboard.test.mjs
dotnet build src/ViLa.slnx -c Release
```

The release solution build also creates the WebExpress plugin package in `pkg/Release`. Browser validation and physical GPIO validation are separate activities. A desktop browser test cannot verify contactor behavior or real meter pulses.

## Migration details

The [WebExpress 2.0 migration notes](doc/webexpress-2-migration.md) describe the new controls, live API behavior, billing corrections, and verification coverage. The older hardware setup reference remains available in `doc/ViLa.docx`.

## License and transparency

The project is distributed under the [MIT license](LICENSE). Parts of the software, documentation, and assets were created with AI-assisted tools. The project maintainer reviews AI-assisted contributions before accepting them.
