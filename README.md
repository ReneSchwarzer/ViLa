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

Measurement configuration remains in `config/vila.settings.xml`. Verify the meter impulse factor, tariff, currency, and operating mode for the installation. Existing measurement files are read from `data/measurements` beside the executable when that directory exists, otherwise from the legacy `measurements` directory.

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
