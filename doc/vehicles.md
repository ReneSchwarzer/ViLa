# Vehicle management

## Vehicle profiles

The Autos navigation entry opens the catalog at `/vila/vehicles`. Each profile has a stable identifier, a display name, a maximum AC charging power in kW, an automatic assignment setting, and an availability setting. Names may contain up to 100 characters. Power must be finite, greater than zero, and at most 1000 kW. The German form accepts decimal commas and decimal points without thousands separators.

The power setting describes the maximum charging power that the vehicle can actually reach at this installation. For example, a car with an 11 kW onboard charger connected to a 7.4 kW supply should use 7.4 kW. Motor output and battery capacity do not describe this value. Vehicles with the same effective charging power cannot be distinguished by the power profile alone.

The availability setting supports deactivation and later reactivation. Inactive vehicles are excluded from recognition and new manual assignments. Existing assignments retain their vehicle identifier and the name captured at assignment time. The catalog deliberately retains inactive profiles instead of deleting historical references. Renaming or deactivating a profile does not rewrite past measurements.

## Measurement assignments

The measurement detail page contains an assignment form. Select an available vehicle to save a manual assignment, select the explicit no vehicle option to leave the session manually unassigned, or select automatic recognition to evaluate the completed measurement again. A currently assigned inactive vehicle remains available in that measurement's selector. The dashboard detail link and the first history column open the same detail page.

The automatic path runs when a new charging session ends, before its measurement log is saved. Existing measurements are not reclassified when a profile is created or edited. Their detail form can explicitly request recognition. Manual choices, including the explicit no vehicle choice, are preserved until the user submits a different selection. The detail page displays the saved vehicle and whether its assignment was automatic or manual.

## Recognition rules

The recognition algorithm converts each stored minute energy sample from kWh to an average power in kW by multiplying it by 60. It requires at least three samples and rejects negative or nonfinite values. The highest observed minute power must match exactly one active vehicle within five percent of its configured maximum, with a minimum tolerance of 0.3 kW. That vehicle must allow automatic assignment. Active profiles with recognition disabled still participate in ambiguity detection, so disabling one of two equivalent profiles does not silently identify the other.

The matching vehicle must additionally have three consecutive minute samples within its power tolerance. Neighboring sample timestamps must differ by 60 seconds with a five second tolerance. Short sessions, missing minute intervals, isolated spikes, unavailable profiles, and ambiguous power matches remain unassigned. Inactive profiles do not participate in matching.

The recognition result is a power based estimate. Charging limits, load balancing, reduced supply power, temperature, and battery state can make a vehicle appear to have another vehicle's lower maximum. Configure profiles for the actual installation and correct uncertain results manually. The application does not receive a vehicle identifier from the S0 electricity meter.

## Storage and compatibility

The vehicle catalog is stored in `data/vehicles.json` beside the executable. Catalog writes use a temporary file and atomic replacement before publishing the updated in-memory snapshot. Catalog edits persist across restarts. Invalid existing catalog data is rejected instead of being silently replaced.

The measurement XML adds the optional attributes `vehicleId`, `vehicleName`, and `vehicleAssignment`. Assignment values are `Unassigned`, `Automatic`, and `Manual`. Older files without these attributes continue to load as unassigned. Existing consumption, tariff, currency, and cost fields retain their stored values. Back up the catalog together with the measurement directory.

## Verification

The automated tests cover localized power input, profile persistence, deactivation, unique and ambiguous matches, disabled recognition, insufficient samples, timestamp gaps, manual override preservation, and XML compatibility. Run `dotnet test src/ViLa.Test/ViLa.Test.csproj` from the repository root.

The browser checks use an isolated host and synthetic measurements. They cover profile creation and editing, validation, deactivation and reactivation, special characters, manual assignment and removal, automatic recognition, ambiguous profiles, history navigation, responsive layout, and persistence after a host restart. These checks do not exercise GPIO hardware or establish recognition accuracy for a real vehicle fleet.
