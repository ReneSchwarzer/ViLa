# WebExpress 2.0 migration

## Framework integration

The application references WebExpress `2.0.0-alpha` and targets .NET 10. Removed `ControlPanelCard` and `ControlPanelGrid` usages are replaced by `ControlCard` and `ControlGrid`. The consumption overview uses `ControlCardCounter` and a responsive `ControlChart`. Tags use the current `Value` and `Color` properties.

The host reads `settings/webexpress.settings.json`, which is included in build and publish output. ViLa measurement settings retain their XML format. The GPIO controller uses the current parameterless constructor and preserves the existing logical pin numbers.

The cryptography dependency is pinned to `System.Security.Cryptography.Pkcs` 10.0.13 to address the advisories reported by NuGet for the transitive 10.0.0 version. Microsoft documents the patched release in [GHSA-2fmv-rv2c-59x7](https://github.com/dotnet/runtime/security/advisories/GHSA-2fmv-rv2c-59x7).

## Live dashboard

The charging endpoint explicitly registers HTTP GET through `MethodAttribute`. The tag lookup endpoint receives the same registration. The charging response sends `Cache-Control: no-store` so subsequent requests receive current measurements.

The JSON contract retains its existing PascalCase property names and string chart values. `CurrentPower` now reports kilowatts by multiplying the measured minute energy by 60. `Power` remains session energy in kilowatt hours. Display numbers follow the request culture, while chart values use invariant decimal formatting. Clients that previously interpreted `CurrentPower` as minute energy must update their unit handling.

The client obtains the endpoint URI from the rendered host and updates the public WebExpress chart controller through `updateData`. It avoids overlapping requests, uses a ten second request timeout, pauses polling in hidden tabs, and releases requests, timers, and the theme observer when destroyed. A failed request keeps previous values visible and changes the connection status until a later request succeeds.

The responsive layout displays four metric cards on wide screens and stacks them on small screens. On the mobile dashboard, the redundant property pane is hidden so the main content can use the available height. Both charts update their axis and legend colors when the WebExpress theme changes. The historical overview is rebuilt when the page is loaded; it does not poll independently.

## Billing and historical values

The billing selector uses an inclusive start and an exclusive end. Both boundaries subtract `BillingDayOffset` from the corresponding first day of the month. This includes January, fractional seconds at month end, leap days, and periods crossing a year boundary without counting an endpoint twice. A session belongs to the period containing its completion time; energy is not split across periods.

The summaries use `FinalPower` and `FinalCost` instead of recalculating completed sessions with the current tariff. Costs are grouped by stored currency, with the configured currency used only for legacy records that have no currency. Tags are split on spaces and semicolons and compared as complete ordinal tokens.

The measurement model supplies final timestamps for records without samples and returns zero for empty power readings or an unconfigured impulse factor. A newly started session records its start before its first sample. Repeated start calls preserve an existing session, and stopping an inactive session has no effect.

## Verification

The C# regression suite covers billing boundaries, offset periods, tag matching, stored currency totals, empty measurements, power conversion, HTTP registration, and cache headers. The JavaScript suite covers successful updates, failed requests and recovery, hidden tabs, cancellation, cleanup, and malformed snapshots. Run the commands in the repository README to repeat these checks.

The browser checks cover the rendered dashboard, both chart canvases, a simulated HTTP 503 followed by recovery, desktop and mobile layouts, dark theme changes, and the history, help, general settings, mode settings, and logging pages. The time control page is conditional on the time-controlled operating mode and is unavailable in other modes. Physical GPIO signals and contactor operation require validation on the target hardware.
