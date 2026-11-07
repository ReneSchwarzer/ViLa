using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WebExpress.WebCore.WebPlugin;

namespace ViLa.Model
{
    public class ViewModel
    {
        /// <summary>
        /// Size of the continuous-log buffer in minutes.
        /// </summary>
        public const int ContinuousLogSize = 5;

        /// <summary>
        /// Threshold in impulses.
        /// </summary>
        public const int ContinuousThreshold = 50;

        /// <summary>
        /// Impulse duration in milliseconds.
        /// </summary>
        public const int ImpulseDuration = 30;

        /// <summary>
        /// GPIO pin that reads the S0 interface of the electricity meter.
        /// </summary>
        private const int PowerMeterPin = 3;

        /// <summary>
        /// GPIO pin that controls the contactor.
        /// </summary>
        private const int ElectricContactorPin = 13;

        /// <summary>
        /// Gets the singleton instance of the model class.
        /// </summary>
        public static ViewModel Instance { get; } = new ViewModel();

        /// <summary>
        /// Gets the persisted vehicle catalog.
        /// </summary>
        public VehicleStore Vehicles { get; } = new VehicleStore(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "vehicles.json"));

        /// <summary>
        /// Gets the current time.
        /// </summary>
        public static string Now => DateTime.Now.ToString("dd.MM.yyyy<br>HH:mm:ss");

        /// <summary>
        /// Gets or sets the reference to the plugin context.
        /// </summary>
        public IPluginContext Context { get; set; }

        /// <summary>
        /// Gets or sets the log entries.
        /// </summary>
        [XmlIgnore]
        public List<LogItem> Logging { get; } = new List<LogItem>();

        /// <summary>
        /// The maximum number of entries kept in <see cref="Logging"/>; older entries are discarded.
        /// </summary>
        public const int MaxLogSize = 10000;

        /// <summary>
        /// The GPIO controller.
        /// </summary>
        private GpioController GPIO { get; set; }

        /// <summary>
        /// Gets or sets the time of the last read.
        /// </summary>
        private Stopwatch Stopwatch { get; } = new Stopwatch();

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        private DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the elapsed minutes.
        /// </summary>
        private long PastMinutes { get; set; } = 0;

        /// <summary>
        /// State of the GPIO pin that controls the contactor.
        /// </summary>
        private bool _electricContactorStatus;

        /// <summary>
        /// Gets or sets whether the contactor is switched on.
        /// </summary>
        protected virtual bool ElectricContactorStatus
        {
            get => _electricContactorStatus;
            set
            {
                try
                {
                    if (value != _electricContactorStatus)
                    {
                        if (!value)
                        {
                            GPIO.Write(ElectricContactorPin, PinValue.High);
                            Log(new LogItem(LogItem.LogLevel.Debug, "vila:vila.log.electriccontactorstatus.high"));
                        }
                        else
                        {
                            GPIO.Write(ElectricContactorPin, PinValue.Low);
                            Log(new LogItem(LogItem.LogLevel.Debug, "vila:vila.log.electriccontactorstatus.low"));
                        }

                        _electricContactorStatus = value;
                    }
                }
                catch (Exception ex)
                {
                    Log(new LogItem(LogItem.LogLevel.Error, "vila:vila.log.electriccontactorstatus.error"));
                    Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }
            }
        }

        /// <summary>
        /// Gets whether a GPIO impulse is present.
        /// </summary>
        protected virtual bool PowerMeterStatus
        {
            get
            {
                try
                {
                    var value = GPIO?.Read(PowerMeterPin);

                    return value == PinValue.High;

                }
                catch (Exception ex)
                {
                    Log(new LogItem(LogItem.LogLevel.Error, "vila:vila.log.powermeterstatus.error"));
                    Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the last status of the GPIO interface.
        /// </summary>
        private bool LastPowerMeterStatus { get; set; }

        /// <summary>
        /// Determines whether the charging process is active.
        /// </summary>
        public bool ActiveCharging => ActiveMeasurementLog != null;

        /// <summary>
        /// Measurement log of the continuous measurement.
        /// </summary>
        private MeasurementLog ContinuousMeasurementLog { get; } = new MeasurementLog()
        {
            ID = Guid.NewGuid().ToString(),
            Measurements = new List<MeasurementItem>() { new MeasurementItem() { MeasurementTimePoint = DateTime.Now } }
        };

        /// <summary>
        /// Current measurement log.
        /// </summary>
        public MeasurementLog CurrentMeasurementLog => ActiveCharging ? ActiveMeasurementLog : ContinuousMeasurementLog;

        /// <summary>
        /// Gets the power measured in the last minute, in kWh.
        /// </summary>
        public float CurrentPower => CurrentMeasurementLog.Measurements.Count > 0 ? CurrentMeasurementLog.CurrentPower : 0;

        /// <summary>
        /// Gets or sets the active measurement log.
        /// </summary>
        private MeasurementLog ActiveMeasurementLog { get; set; }

        /// <summary>
        /// Gets the already-completed measurement logs.
        /// </summary>
        private List<MeasurementLog> HistoryMeasurementLog { get; } = new List<MeasurementLog>();

        /// <summary>
        /// Guards every read and write of <see cref="HistoryMeasurementLog"/>, which is shared between requests.
        /// </summary>
        private readonly object _historyLock = new object();

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        public Settings Settings { get; private set; } = new Settings() { Currency = "€" };

        /// <summary>
        /// Gets or sets the settings storage selected by the host.
        /// </summary>
        public SettingsStore SettingsStorage { get; set; } = new SettingsStore(AppDomain.CurrentDomain.BaseDirectory);

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets all assigned labels.
        /// </summary>
        public IEnumerable<string> Tags => GetHistoryMeasurementLogs()
            .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
            .SelectMany(x => x.Tag.Split(new[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct();

        /// <summary>
        /// Constructor.
        /// </summary>
        private ViewModel()
        {
        }

        /// <summary>
        /// Initialization.
        /// </summary>
        public void Init()
        {
            try
            {
                // initialize the controller
                var chipNumber = Environment.GetEnvironmentVariable("VILA_GPIO_CHIP");
                GPIO = chipNumber == null
                    ? new GpioController()
                    : new GpioController(new LibGpiodDriver(int.Parse(chipNumber, CultureInfo.InvariantCulture)));
                GPIO.OpenPin(PowerMeterPin, PinMode.InputPullUp);
                GPIO.OpenPin(ElectricContactorPin, PinMode.Output);

                GPIO.Write(ElectricContactorPin, PinValue.High);
                _electricContactorStatus = false;

                Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.log.init.gpio"));
                Log(new LogItem(LogItem.LogLevel.Debug, "ElectricContactorPin " + ElectricContactorPin));
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
            }


            // Load existing measurement logs
            var directoryName = ResolveMeasurementsDirectory();

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var files = Directory.GetFiles(directoryName, "*.xml");
            var serializer = new XmlSerializer(typeof(MeasurementLog));
            foreach (var file in files)
            {
                try
                {
                    using var reader = File.OpenText(file);
                    var measurementLog = serializer.Deserialize(reader) as MeasurementLog;
                    lock (_historyLock)
                    {
                        HistoryMeasurementLog.Add(measurementLog);
                    }
                }
                catch (Exception ex)
                {
                    Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }
            }

            // Archive very old measurement logs (cyclically)
            Task.Run(() =>
            {
                while (true)
                {
                    foreach (var his in GetHistoryMeasurementLogs().Where(x => x.Till < DateTime.Now.AddYears(-10)))
                    {
                        ArchiveHistoryMeasurementLog(his.ID);
                    }

                    Thread.Sleep(1000 * 60 * 60 * 24);
                }
            });

            Culture = System.Globalization.CultureInfo.CurrentCulture;

            ResetSettings();

            StartTime = DateTime.Now;
        }

        /// <summary>
        /// Update function.
        /// </summary>
        public virtual void Update()
        {
            try
            {
                var delta = Stopwatch.ElapsedMilliseconds;
                var newValue = PowerMeterStatus;
                var pulse = newValue != LastPowerMeterStatus && newValue == true;
                LastPowerMeterStatus = newValue;

                if (delta > ImpulseDuration)
                {
                    Log(new LogItem(LogItem.LogLevel.Warning, "vila:vila.log.update.exceeding") { Argument = Convert.ToString(delta - ViewModel.ImpulseDuration, System.Globalization.CultureInfo.InvariantCulture) });
                }

                if (Stopwatch.IsRunning)
                {
                    var minutes = (long)Math.Floor((DateTime.Now - StartTime).TotalMinutes);

                    if (pulse)
                    {
                        ContinuousMeasurementLog.CurrentMeasurement.Impulse++;
                        ContinuousMeasurementLog.CurrentMeasurement.Power = (float)ContinuousMeasurementLog?.CurrentMeasurement?.Impulse / Settings.ImpulsePerkWh;
                    }

                    // New measurement value
                    if (minutes > PastMinutes)
                    {
                        PastMinutes = minutes;

                        if (ActiveCharging)
                        {
                            ActiveMeasurementLog.Measurements.Add(ContinuousMeasurementLog.CurrentMeasurement);
                        }

                        ContinuousMeasurementLog.Measurements.Add(new MeasurementItem() { MeasurementTimePoint = DateTime.Now });

                        while (ContinuousMeasurementLog.Measurements.Count > ContinuousLogSize)
                        {
                            ContinuousMeasurementLog.Measurements.RemoveAt(0);
                        }

                        var impulse = ContinuousMeasurementLog.Impulse;
                        if (impulse > ContinuousThreshold && !ActiveCharging && Settings.Mode == Mode.AutomaticControlled)
                        {
                            StartCharging();

                            // Find the first measurement value with non-zero data
                            var skip = 0;
                            for (var i = 0; i < ContinuousMeasurementLog.Measurements.Count; i++)
                            {
                                if (ContinuousMeasurementLog.Measurements[i].Impulse > 0)
                                {
                                    skip = i;
                                    break;
                                }
                            }

                            // Attribute the already-consumed energy measured for auto-detection to the new log
                            var measurements = ContinuousMeasurementLog.Measurements.Skip(skip);
                            ActiveMeasurementLog.Measurements.Clear();
                            ActiveMeasurementLog.Measurements.AddRange(measurements.SkipLast(1));

                            return;
                        }
                        else if (impulse <= ContinuousThreshold && ActiveCharging && Settings.Mode == Mode.AutomaticControlled)
                        {
                            StopCharging();
                            return;
                        }
                        else if
                        (
                            ActiveCharging &&
                            Settings.MinWattage >= 0 &&
                            ActiveMeasurementLog?.Power >= 0.5 &&
                            ActiveMeasurementLog?.CurrentMeasurement?.Power <= Settings.MinWattage
                        )
                        {
                            Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.charging.min"));

                            StopCharging();
                            return;
                        }
                    }
                }

                if (ActiveCharging && Settings.MaxChargingTime > 0 && (DateTime.Now - ActiveMeasurementLog.From).TotalSeconds > Settings.MaxChargingTime * 60 * 60)
                {
                    Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.charging.time.max"));

                    StopCharging();
                    return;
                }

                if (ActiveCharging && Settings.MaxWattage > 0 && ActiveMeasurementLog.Power > Settings.MaxWattage)
                {
                    Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.charging.consumption.max"));

                    StopCharging();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Error, "vila:vila.charging.error"));
                Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
            }
            finally
            {
                Stopwatch.Restart();
            }
        }

        /// <summary>
        /// Loggt ein Event
        /// </summary>
        /// <param name="logItem">The log entry.</param>
        public void Log(LogItem logItem)
        {
            lock (Logging)
            {
                Logging.Add(logItem);
                if (Logging.Count > MaxLogSize)
                {
                    Logging.RemoveRange(0, Logging.Count - MaxLogSize);
                }
            }

            if (ActiveCharging &&
                logItem.Level != LogItem.LogLevel.Info &&
                logItem.Level != LogItem.LogLevel.Debug)
            {
                var current = ActiveMeasurementLog?.CurrentMeasurement;
                current?.Logitems.Add(logItem);
            }

            // TODO MIGRATION: IPluginContext no longer exposes a Host with a Log facade in 0.0.11.
            // Resolve the LogManager via ComponentHub or wire the log routing differently when refactoring.
            System.Diagnostics.Debug.WriteLine($"[{logItem.Level}] {logItem.Instance}: {logItem.Massage}");
        }

        /// <summary>
        /// Invoked when settings should be saved.
        /// </summary>
        public void SaveSettings()
        {
            Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.setting.save"));

            SettingsStorage.Save(Settings);
            ResetSettings();
        }

        /// <summary>
        /// Exports the persisted application data and current settings.
        /// </summary>
        /// <param name="includeArchive">Whether archived measurements are included.</param>
        /// <returns>The portable ZIP backup.</returns>
        public byte[] ExportData(bool includeArchive = false)
        {
            lock (DataBackup.StorageLock)
            {
                lock (_historyLock)
                {
                    return new DataBackup(AppDomain.CurrentDomain.BaseDirectory, Vehicles, SettingsStorage).Export(Settings, includeArchive);
                }
            }
        }

        /// <summary>
        /// Adds missing backup records and makes imported history available immediately.
        /// </summary>
        /// <param name="content">The uploaded ZIP backup.</param>
        /// <returns>The counts of imported and preserved records.</returns>
        public BackupImportResult ImportData(byte[] content)
        {
            lock (DataBackup.StorageLock)
            {
                lock (_historyLock)
                {
                    var reserved = HistoryMeasurementLog.Select(x => x.ID).ToList();
                    if (ActiveMeasurementLog != null)
                    {
                        reserved.Add(ActiveMeasurementLog.ID);
                    }
                    return new DataBackup(AppDomain.CurrentDomain.BaseDirectory, Vehicles, SettingsStorage).Import(content, reserved, result =>
                    {
                        HistoryMeasurementLog.AddRange(result.Measurements);
                        if (result.SettingsAdded)
                        {
                            ResetSettings();
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Invoked when settings should be reset.
        /// </summary>
        public void ResetSettings()
        {
            Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.setting.load"));

            try
            {
                Settings = SettingsStorage.Load();
            }
            catch
            {
                Log(new LogItem(LogItem.LogLevel.Warning, "vila:vila.setting.warning"));
            }

            Log(new LogItem(LogItem.LogLevel.Debug, "ImpulsePerkWh = " + Settings?.ImpulsePerkWh));
        }

        /// <summary>
        /// Gets the data directory for measurement logs; creates it if necessary.
        /// Reihenfolge: <BaseDirectory>/data/measurements, sonst <BaseDirectory>/measurements.
        /// </summary>
        private static string ResolveMeasurementsDirectory()
        {
            var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            var dataDir = Path.Combine(baseDir, "data", "measurements");
            if (Directory.Exists(dataDir))
            {
                return dataDir;
            }

            var legacyDir = Path.Combine(baseDir, "measurements");
            Directory.CreateDirectory(legacyDir);
            return legacyDir;
        }

        /// <summary>
        /// Starts a charging session unless a session is already active.
        /// </summary>
        public void StartCharging()
        {
            if (ActiveCharging)
            {
                return;
            }

            Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.charging.begin"));

            ActiveMeasurementLog = new MeasurementLog()
            {
                ID = Guid.NewGuid().ToString(),
                FinalFrom = DateTime.Now,
                Measurements = new List<MeasurementItem>()
            };

            ElectricContactorStatus = true;
        }

        /// <summary>
        /// Completes and stores the active charging session.
        /// </summary>
        public void StopCharging()
        {
            if (!ActiveCharging)
            {
                return;
            }

            Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.charging.stop"));

            ActiveMeasurementLog.FinalPower = ActiveMeasurementLog.Power;
            ActiveMeasurementLog.FinalCost = ActiveMeasurementLog.Cost;
            ActiveMeasurementLog.FinalFrom = ActiveMeasurementLog.From;
            ActiveMeasurementLog.FinalTill = DateTime.Now;
            ActiveMeasurementLog.ElectricityPricePerkWh = Settings.ElectricityPricePerkWh;
            ActiveMeasurementLog.ImpulsePerkWh = Settings.ImpulsePerkWh;
            ActiveMeasurementLog.Currency = Settings.Currency;

            VehicleRecognition.Apply(ActiveMeasurementLog, Vehicles.GetAll());

            // persist the completed session
            SaveMeasurementLog();

            ActiveMeasurementLog = null;
            ElectricContactorStatus = false;

            Stopwatch.Restart();
        }

        /// <summary>
        /// Gets the completed measurement logs
        /// </summary>
        /// <param name="from">Die Anfang, in welcher die Messprotokolle geliefert werden sollen</param>
        /// <param name="till">Das Ende, in welcher die Messprotokolle geliefert werden sollen</param>
        /// <return>Messprotokolle, welche sich innerhalb der gegebenen Zeitspanne befinden</return>
        public IEnumerable<MeasurementLog> GetHistoryMeasurementLogs(DateTime from, DateTime till)
        {
            return GetHistoryMeasurementLogs().Where(x => x.Till >= from && x.Till <= till).OrderByDescending(x => x.Till);
        }

        /// <summary>
        /// Gets all completed measurement logs
        /// </summary>
        /// <return>A snapshot of all stored measurement logs that is safe to enumerate while other requests change the history.</return>
        public IEnumerable<MeasurementLog> GetHistoryMeasurementLogs()
        {
            lock (_historyLock)
            {
                return HistoryMeasurementLog.ToArray();
            }
        }

        /// <summary>
        /// Gets a single completed measurement log
        /// </summary>
        /// <param name="id">The measurement log identifier.</param>
        /// <return>Das Messprokoll oder null</return>
        public MeasurementLog GetHistoryMeasurementLog(string id)
        {
            lock (_historyLock)
            {
                return HistoryMeasurementLog.FirstOrDefault(x => x.ID.Equals(id));
            }
        }

        /// <summary>
        /// Moves a completed measurement if its persisted assignment still matches the source column.
        /// </summary>
        /// <param name="id">The completed measurement identifier.</param>
        /// <param name="source">The expected vehicle identifier or "none" for an unassigned measurement.</param>
        /// <param name="target">The destination vehicle identifier or "none" to clear the assignment.</param>
        /// <returns>Whether the assignment was saved.</returns>
        public bool TryMoveMeasurement(string id, string source, string target)
        {
            var log = GetHistoryMeasurementLog(id);
            if (log == null)
            {
                return false;
            }
            lock (log)
            {
                if ((string.IsNullOrEmpty(log.VehicleId) ? "none" : log.VehicleId) != source || source == target
                    || (target != "none" && !Vehicles.GetAll().Any(x => x.Active && x.Id == target)))
                {
                    return false;
                }
                AssignVehicle(id, target);
                return true;
            }
        }

        /// <summary>
        /// Persists a manual vehicle choice or explicitly reruns automatic recognition.
        /// </summary>
        /// <param name="id">The completed measurement log identifier.</param>
        /// <param name="selection">A vehicle identifier, "none", or "auto".</param>
        public void AssignVehicle(string id, string selection)
        {
            var log = GetHistoryMeasurementLog(id) ?? throw new ArgumentException("The measurement does not exist.", nameof(id));
            var vehicle = Vehicles.GetAll().FirstOrDefault(x => x.Id == selection && (x.Active || x.Id == log.VehicleId));
            if (selection != "auto" && selection != "none" && vehicle == null)
            {
                throw new ArgumentException("The selected vehicle is unavailable.", nameof(selection));
            }
            lock (log)
            {
                var originalId = log.VehicleId;
                var originalName = log.VehicleName;
                var originalAssignment = log.VehicleAssignment;
                try
                {
                    log.VehicleId = vehicle?.Id;
                    log.VehicleName = vehicle?.Name;
                    log.VehicleAssignment = selection == "auto" ? VehicleAssignment.Unassigned : VehicleAssignment.Manual;
                    if (selection == "auto")
                    {
                        VehicleRecognition.Apply(log, Vehicles.GetAll());
                    }
                    UpdateMeasurementLog(log);
                }
                catch
                {
                    log.VehicleId = originalId;
                    log.VehicleName = originalName;
                    log.VehicleAssignment = originalAssignment;
                    throw;
                }
            }
        }

        /// <summary>
        /// Persists a completed measurement log and publishes it in the history.
        /// </summary>
        public void SaveMeasurementLog()
        {
            lock (DataBackup.StorageLock)
            {
                var serializer = new XmlSerializer(typeof(MeasurementLog));
                var xmlns = new XmlSerializerNamespaces();
                xmlns.Add(string.Empty, string.Empty);

                using (var memoryStream = new MemoryStream())
                {
                    serializer.Serialize(memoryStream, ActiveMeasurementLog, xmlns);

                    var utf = new UTF8Encoding();
                    var fileName = Path.Combine(ResolveMeasurementsDirectory(), string.Format("{0}.xml", ActiveMeasurementLog.ID));

                    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    }

                    File.WriteAllText
                    (
                        fileName,
                        utf.GetString(memoryStream.ToArray())
                    );

                    lock (_historyLock)
                    {
                        HistoryMeasurementLog.Add(ActiveMeasurementLog);
                    }

                    Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.charging.save") { Argument = Convert.ToString(fileName, System.Globalization.CultureInfo.InvariantCulture) });
                }
            }
        }

        /// <summary>
        /// Persists changes to a completed measurement log.
        /// </summary>
        /// <param name="measurement">The measurement log to persist.</param>
        public void UpdateMeasurementLog(MeasurementLog measurement)
        {
            lock (DataBackup.StorageLock)
            {
                var serializer = new XmlSerializer(typeof(MeasurementLog));
                var xmlns = new XmlSerializerNamespaces();
                xmlns.Add(string.Empty, string.Empty);

                using (var memoryStream = new MemoryStream())
                {
                    serializer.Serialize(memoryStream, measurement, xmlns);

                    var utf = new UTF8Encoding();
                    var fileName = Path.Combine(ResolveMeasurementsDirectory(), string.Format("{0}.xml", measurement.ID));

                    if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                    }

                    File.WriteAllText
                    (
                        fileName,
                        utf.GetString(memoryStream.ToArray())
                    );

                    Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.charging.save") { Argument = Convert.ToString(fileName, System.Globalization.CultureInfo.InvariantCulture) });
                }
            }
        }

        /// <summary>
        /// Deletes a completed measurement log and removes it from the history.
        /// </summary>
        /// <param name="id">The measurement log identifier.</param>
        public void RemoveHistoryMeasurementLog(string id)
        {
            lock (DataBackup.StorageLock)
            {
                try
                {
                    var measurementLog = GetHistoryMeasurementLog(id);
                    if (measurementLog != null)
                    {
                        File.Delete(Path.Combine(ResolveMeasurementsDirectory(), $"{measurementLog.ID}.xml"));
                        Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.delete.file") { Argument = Convert.ToString(id, System.Globalization.CultureInfo.InvariantCulture) });

                        lock (_historyLock)
                        {
                            HistoryMeasurementLog.Remove(measurementLog);
                        }
                    }
                    else
                    {
                        Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.delete.error") { Argument = Convert.ToString(id, System.Globalization.CultureInfo.InvariantCulture) });
                    }
                }
                catch (Exception ex)
                {
                    Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }
            }
        }

        /// <summary>
        /// Moves a completed measurement log into the archive.
        /// </summary>
        /// <param name="id">The measurement log identifier.</param>
        public void ArchiveHistoryMeasurementLog(string id)
        {
            lock (DataBackup.StorageLock)
            {
                try
                {
                    var measurementLog = GetHistoryMeasurementLog(id);
                    if (measurementLog != null)
                    {
                        var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                        var archive = Path.Combine(baseDir, "data", "archive");
                        if (!Directory.Exists(archive))
                        {
                            archive = Path.Combine(baseDir, "archive");
                            if (!Directory.Exists(archive))
                            {
                                Directory.CreateDirectory(archive);
                            }
                        }

                        var year = Path.Combine(archive, DateTime.Now.Year.ToString());
                        if (!Directory.Exists(year))
                        {
                            Directory.CreateDirectory(year);
                        }

                        var month = Path.Combine(year, DateTime.Now.ToString("MM"));
                        if (!Directory.Exists(month))
                        {
                            Directory.CreateDirectory(month);
                        }

                        var source = Path.Combine(ResolveMeasurementsDirectory(), id + ".xml");
                        var destination = Path.Combine(month, id + ".xml");

                        File.Move(source, destination);

                        lock (_historyLock)
                        {
                            HistoryMeasurementLog.Remove(measurementLog);
                        }

                        Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.archive.move") { Argument = Convert.ToString(id, System.Globalization.CultureInfo.InvariantCulture) });
                    }
                    else
                    {
                        Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.archive.error") { Argument = Convert.ToString(id, System.Globalization.CultureInfo.InvariantCulture) });
                    }
                }
                catch (Exception ex)
                {
                    Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }
            }
        }

        /// <summary>
        /// Calculates a color for a tag.
        /// </summary>
        /// <param name="tag">Das Label.</param>
        /// <returns>Der Farbcode.</returns>
        public string GetColor(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return "#dddddd";
            }

            // FNV-1a over the UTF-8 bytes: string.GetHashCode() is randomized per process,
            // which would give every tag a new color after each restart.
            uint hashCode = 2166136261;
            foreach (var b in Encoding.UTF8.GetBytes(tag))
            {
                hashCode = unchecked((hashCode ^ b) * 16777619);
            }
            int hue = (int)(hashCode % 360);
            float saturation = 0.5f;
            float lightness = 0.6f;

            float temp2 = lightness < 0.5f ? lightness * (1.0f + saturation) : lightness + saturation - lightness * saturation;
            float temp1 = 2.0f * lightness - temp2;

            Func<float, float> getColorComponent = temp3 =>
            {
                temp3 = temp3 < 0 ? temp3 + 360 : temp3 > 360 ? temp3 - 360 : temp3;
                return temp3 < 60 ? temp1 + (temp2 - temp1) * temp3 / 60f : temp3 < 180 ? temp2 : temp3 < 240 ? temp1 + (temp2 - temp1) * (240 - temp3) / 60f : temp1;
            };

            Color color = Color.FromArgb((int)(255 * getColorComponent(hue + 120)), (int)(255 * getColorComponent(hue)), (int)(255 * getColorComponent(hue - 120)));
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
