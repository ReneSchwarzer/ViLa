using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ViLa.Model
{
    /// <summary>
    /// Persists the vehicle catalog using atomic file replacement and immutable entries.
    /// </summary>
    public sealed class VehicleStore
    {
        private readonly object _sync = new object();
        private readonly string _path;
        private List<Vehicle> _vehicles;

        /// <summary>
        /// Opens a catalog without creating or replacing an existing file.
        /// </summary>
        /// <param name="path">The absolute catalog file path.</param>
        public VehicleStore(string path)
        {
            _path = Path.GetFullPath(path);
            _vehicles = File.Exists(_path)
                ? JsonSerializer.Deserialize<List<Vehicle>>(File.ReadAllText(_path)) ?? throw new InvalidDataException("The vehicle catalog is empty.")
                : new List<Vehicle>();
            if (_vehicles.Any(x => x == null || !Guid.TryParse(x.Id, out _) || !IsValid(x.Name, x.MaxPowerKw))
                || _vehicles.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count() != _vehicles.Count)
            {
                throw new InvalidDataException("The vehicle catalog contains invalid or duplicate entries.");
            }
        }

        /// <summary>
        /// Returns a stable snapshot including inactive vehicles.
        /// </summary>
        /// <returns>The catalog sorted by display name.</returns>
        public Vehicle[] GetAll()
        {
            lock (_sync)
            {
                return _vehicles.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToArray();
            }
        }

        /// <summary>
        /// Creates or updates a vehicle and commits the catalog before publishing the change.
        /// </summary>
        /// <param name="id">The existing identifier, or null for a new vehicle.</param>
        /// <param name="name">The vehicle display name.</param>
        /// <param name="maxPowerKw">The expected maximum charging power in kilowatts.</param>
        /// <param name="automaticAssignment">Whether automatic assignment is enabled.</param>
        /// <param name="active">Whether the vehicle is available for new assignments.</param>
        /// <returns>The saved vehicle.</returns>
        public Vehicle Save(string id, string name, double maxPowerKw, bool automaticAssignment, bool active)
        {
            if (!IsValid(name, maxPowerKw))
            {
                throw new ArgumentException("A vehicle requires a name of at most 100 characters and a power between zero and 1000 kW.");
            }
            lock (_sync)
            {
                if (id != null && !_vehicles.Any(x => x.Id == id))
                {
                    throw new ArgumentException("The vehicle does not exist.", nameof(id));
                }
                var vehicle = new Vehicle
                {
                    Id = id ?? Guid.NewGuid().ToString(), Name = name.Trim(), MaxPowerKw = maxPowerKw,
                    AutomaticAssignment = automaticAssignment, Active = active
                };
                var updated = _vehicles.Where(x => x.Id != vehicle.Id).Append(vehicle).ToList();
                Directory.CreateDirectory(Path.GetDirectoryName(_path));
                var temporary = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
                try
                {
                    File.WriteAllText(temporary, JsonSerializer.Serialize(updated, new JsonSerializerOptions { WriteIndented = true }));
                    File.Move(temporary, _path, true);
                    _vehicles = updated;
                }
                finally
                {
                    if (File.Exists(temporary))
                    {
                        File.Delete(temporary);
                    }
                }
                return vehicle;
            }
        }

        /// <summary>
        /// Checks the persisted name and power constraints.
        /// </summary>
        /// <param name="name">The vehicle name.</param>
        /// <param name="power">The maximum charging power.</param>
        /// <returns>Whether both fields are valid.</returns>
        private static bool IsValid(string name, double power)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Trim().Length <= 100
                && double.IsFinite(power) && power > 0 && power <= 1000;
        }
    }
}
