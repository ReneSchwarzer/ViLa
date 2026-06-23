using System;
using System.Collections.Generic;
using System.Device.Gpio;
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
        public List<LogItem> Logging { get; set; } = new List<LogItem>();

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
        /// Gets or sets the settings.
        /// </summary>
        public Settings Settings { get; private set; } = new Settings() { Currency = "€" };

        /// <summary>
        /// Gets the culture.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets all assigned labels.
        /// </summary>
        public IEnumerable<string> Tags => HistoryMeasurementLog
            .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
            .SelectMany(x => x.Tag.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
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
                // Initialize the controller
                GPIO = new GpioController(PinNumberingScheme.Logical);
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
                    HistoryMeasurementLog.Add(serializer.Deserialize(reader) as MeasurementLog);
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
                    foreach (var his in HistoryMeasurementLog.Where(x => x.Till < DateTime.Now.AddYears(-10)).ToList())
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
                    Log(new LogItem(LogItem.LogLevel.Warning, string.Format(System.Globalization.CultureInfo.CurrentCulture, "vila:vila.log.update.exceeding", delta - ViewModel.ImpulseDuration)));
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
            Logging.Add(logItem);

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

            // Save settings
            var serializer = new XmlSerializer(typeof(Settings));

            using var memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, Settings);

            var utf = new UTF8Encoding();

            File.WriteAllText
            (
                ResolveConfigPath("vila.settings.xml"),
                utf.GetString(memoryStream.ToArray())
            );
        }

        /// <summary>
        /// Invoked when settings should be reset.
        /// </summary>
        public void ResetSettings()
        {
            Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.setting.load"));

            // Load settings
            var serializer = new XmlSerializer(typeof(Settings));

            // 0.0.11 migration: csproj copies vila.settings.xml to <BaseDirectory>/Config/,
            // but the code looked in <BaseDirectory>/. Fallback: Config/ first, then BaseDirectory.
            var settingsPath = ResolveConfigPath("vila.settings.xml");

            try
            {
                using var reader = File.OpenText(settingsPath);
                Settings = serializer.Deserialize(reader) as Settings;
            }
            catch
            {
                Log(new LogItem(LogItem.LogLevel.Warning, "vila:vila.setting.warning"));
            }

            Log(new LogItem(LogItem.LogLevel.Debug, "ImpulsePerkWh = " + Settings?.ImpulsePerkWh));
        }

        /// <summary>
        /// Looks up a config file first in the Config/ subfolder (0.0.11 default),
        /// then falls back to BaseDirectory (1.4.7 default).
        /// </summary>
        private static string ResolveConfigPath(string fileName)
        {
            var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(baseDir, "Config", fileName);
            if (File.Exists(configPath))
            {
                return configPath;
            }

            var rootPath = Path.Combine(baseDir, fileName);
            return rootPath;
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
        /// Startet den Ladevorgang.
        /// </summary>
        public void StartCharging()
        {
            Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.charging.begin"));

            ActiveMeasurementLog = new MeasurementLog()
            {
                ID = Guid.NewGuid().ToString(),
                Measurements = new List<MeasurementItem>()
            };

            // Initialer Messwert
            //ActiveMeasurementLog.Measurements.Add(new MeasurementItem()
            //{
            //    MeasurementTimePoint = DateTime.Now
            //});

            ElectricContactorStatus = true;
        }

        /// <summary>
        /// Beendet den Ladevorgang.
        /// </summary>
        public void StopCharging()
        {
            Log(new LogItem(LogItem.LogLevel.Info, "vila:vila.charging.stop"));

            ActiveMeasurementLog.FinalPower = ActiveMeasurementLog.Power;
            ActiveMeasurementLog.FinalCost = ActiveMeasurementLog.Cost;
            ActiveMeasurementLog.FinalFrom = ActiveMeasurementLog.From;
            ActiveMeasurementLog.FinalTill = DateTime.Now;
            ActiveMeasurementLog.ElectricityPricePerkWh = Settings.ElectricityPricePerkWh;
            ActiveMeasurementLog.ImpulsePerkWh = Settings.ImpulsePerkWh;
            ActiveMeasurementLog.Currency = Settings.Currency;

            // Messung speichern
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
            return HistoryMeasurementLog.Where(x => x.Till >= from && x.Till <= till).OrderByDescending(x => x.Till);
        }

        /// <summary>
        /// Gets all completed measurement logs
        /// </summary>
        /// <return>Alle gespeicherten Messprotokoll</return>
        public IEnumerable<MeasurementLog> GetHistoryMeasurementLogs()
        {
            return HistoryMeasurementLog;
        }

        /// <summary>
        /// Gets a single completed measurement log
        /// </summary>
        /// <param name="id">Die ID des Messprotokolls</param>
        /// <return>Das Messprokoll oder null</return>
        public MeasurementLog GetHistoryMeasurementLog(string id)
        {
            return HistoryMeasurementLog.Where(x => x.ID.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Speichert ein abgeschlossenes Messprotokoll
        /// </summary>
        public void SaveMeasurementLog()
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

                HistoryMeasurementLog.Add(ActiveMeasurementLog);

                Log(new LogItem(LogItem.LogLevel.Info, string.Format("vila:vila.charging.save", fileName)));
            }
        }

        /// <summary>
        /// Modifies a completed measurement log
        /// </summary>
        /// <param name="measurement">Das Messprotokoll, welches upgedatet werden soll</param>
        public void UpdateMeasurementLog(MeasurementLog measurement)
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

                Log(new LogItem(LogItem.LogLevel.Info, string.Format("vila:vila.charging.save", fileName)));
            }
        }

        /// <summary>
        /// Deletes a completed measurement log
        /// </summary>
        /// <param name="id">Die ID des Messprotokolls</param>
        public void RemoveHistoryMeasurementLog(string id)
        {
            try
            {
                var measurementLog = GetHistoryMeasurementLog(id);
                if (measurementLog != null)
                {
                    File.Delete(Path.Combine(ResolveMeasurementsDirectory(), $"{measurementLog.ID}.xml"));
                    ViewModel.Instance.Logging.Add(new LogItem(LogItem.LogLevel.Info, string.Format("vila:vila.delete.file", id)));

                    HistoryMeasurementLog.Remove(measurementLog);
                }
                else
                {
                    Log(new LogItem(LogItem.LogLevel.Info, string.Format("vila:vila.delete.error", id)));
                }
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
            }
        }

        /// <summary>
        /// Archiviert ein abgeschlossenes Messprotokoll
        /// </summary>
        /// <param name="id">Die ID des Messprotokolls</param>
        public void ArchiveHistoryMeasurementLog(string id)
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

                    HistoryMeasurementLog.Remove(measurementLog);

                    Log(new LogItem(LogItem.LogLevel.Info, string.Format("vila:vila.archive.move", id)));
                }
                else
                {
                    Log(new LogItem(LogItem.LogLevel.Info, string.Format("vila:vila.archive.error", id)));
                }
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
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

            int hashCode = tag.GetHashCode();
            int hue = Math.Abs(hashCode) % 360;
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