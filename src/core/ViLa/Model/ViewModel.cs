using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using WebExpress.Plugin;

namespace ViLa.Model
{
    public class ViewModel
    {
        /// <summary>
        /// Die Größe des Autobuffers in minuten
        /// </summary>
        public const int AutoBufferSize = 5;
        
        /// <summary>
        /// Der Schwellwert in Impulsen
        /// </summary>
        public const int AutoThreshold = 500;

        /// <summary>
        /// Impulsdauer im ms 
        /// </summary>
        public const int ImpulseDuration = 30;

        /// <summary>
        /// Der GPIO-Pin, welcher die S0-Schnittstelle des Strommeßgerät ausließt
        /// </summary>
        private const int _powerMeterPin = 3;

        /// <summary>
        /// Der GPIO-Pin, welcher den Schütz steuert
        /// </summary>
        private const int _electricContactorPin = 13;

        /// <summary>
        /// Instanz des einzigen Modells
        /// </summary>
        private static ViewModel _this = null;

        /// <summary>
        /// Lifert die einzige Instanz der Modell-Klasse
        /// </summary>
        public static ViewModel Instance
        {
            get
            {
                if (_this == null)
                {
                    _this = new ViewModel();
                }

                return _this;
            }
        }

        /// <summary>
        /// Liefert die aktuelle Zeit
        /// </summary>
        public static string Now => DateTime.Now.ToString("dd.MM.yyyy<br>HH:mm:ss");

        /// <summary>
        /// Liefert oder setzt den Verweis auf den Kontext des Plugins
        /// </summary>
        public IPluginContext Context { get; set; }

        /// <summary>
        /// Liefert oder setzt den Staustext
        /// </summary>
        [XmlIgnore]
        public List<LogItem> Logging { get; set; } = new List<LogItem>();

        /// <summary>
        /// Der GPIO-Controller
        /// </summary>
        private GpioController GPIO { get; set; }

        /// <summary>
        /// Liefert oder setzt die Zeit des letzen auslesen der Temperatur
        /// </summary>
        private DateTime _lastMetering = DateTime.MinValue;

        /// <summary>
        /// Der Zustand des GPIO-Pins, welcher den Schütz steuert
        /// </summary>
        private bool _electricContactorStatus;

        /// <summary>
        /// Liefert oder setzt ob der Schütz angeschaltet ist
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
                            GPIO.Write(_electricContactorPin, PinValue.High);
                            Log(new LogItem(LogItem.LogLevel.Debug, "Status des Schütz wurde auf HIGH geändert"));
                        }
                        else
                        {
                            GPIO.Write(_electricContactorPin, PinValue.Low);
                            Log(new LogItem(LogItem.LogLevel.Debug, "Status des Schütz wurde auf LOW geändert"));
                        }

                        _electricContactorStatus = value;
                    }
                }
                catch (Exception ex)
                {
                    Log(new LogItem(LogItem.LogLevel.Error, "Status des Schütz konnte nicht ermittelt werden"));
                    Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }
            }
        }

        /// <summary>
        /// Liefert oder setzt ob im S0-Impuls anliegt 
        /// </summary>
        protected virtual bool PowerMeterStatus
        {
            get
            {
                try
                {
                    var value = GPIO.Read(_powerMeterPin);

                    return value == PinValue.High;

                }
                catch (Exception ex)
                {
                    Log(new LogItem(LogItem.LogLevel.Error, "Status der S0-Schnittstelle konnte nicht ermittelt werden"));
                    Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }

                return false;
            }

        }

        /// <summary>
        /// Liefert oder setzt den letzten Status der S0-Schnittstelle
        /// </summary>
        private bool LastPowerMeterStatus { get; set; }

        /// <summary>
        /// Liefert oder setzt das aktuelle Messprotokoll
        /// </summary>
        public MeasurementLog CurrentMeasurementLog { get; private set; }

        /// <summary>
        /// Liefert oder setzt, ob der Ladevorgang aktiv ist
        /// </summary>
        public bool ActiveCharging => CurrentMeasurementLog != null;

        /// <summary>
        /// Puffer für automatische Messungen
        /// </summary>
        public MeasurementLog AutoMeasurementLog { get; } = new MeasurementLog()
        {
            ID = Guid.NewGuid().ToString(),
            From = DateTime.Now,
            Measurements = new List<MeasurementItem>() { new MeasurementItem() { MeasurementTimePoint = DateTime.Now } }
        };

        /// <summary>
        /// Liefert oder setzt die Settings
        /// </summary>
        public Settings Settings { get; private set; } = new Settings() { Currency = "€" };

        /// <summary>
        /// Konstruktor
        /// </summary>
        private ViewModel()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public void Init()
        {
            try
            {
                // Initialisierung des Controllers
                GPIO = new GpioController(PinNumberingScheme.Logical);
                GPIO.OpenPin(_powerMeterPin, PinMode.InputPullUp);
                GPIO.OpenPin(_electricContactorPin, PinMode.Output);

                GPIO.Write(_electricContactorPin, PinValue.High);
                _electricContactorStatus = false;

                Log(new LogItem(LogItem.LogLevel.Info, "GpioController gestartet"));
                Log(new LogItem(LogItem.LogLevel.Debug, "ElectricContactorPin " + _electricContactorPin));
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
            }

            ResetSettings();
        }

        /// <summary>
        /// Updatefunktion
        /// </summary>
        public virtual void Update()
        {
            try
            {
                var delta = (DateTime.Now - _lastMetering).TotalMilliseconds;

                if (delta > ImpulseDuration)
                {
                    Log(new LogItem(LogItem.LogLevel.Warning, string.Format("Zeitspanne der S0-Schnittstelle um {0}ms überschritten", delta - ViewModel.ImpulseDuration)));
                }

                var newValue = PowerMeterStatus;
                var pulse = newValue != LastPowerMeterStatus && newValue == true;

                LastPowerMeterStatus = PowerMeterStatus;

                if (pulse)
                {
                    AutoMeasurementLog.Impulse++;
                    AutoMeasurementLog.Power = (float)AutoMeasurementLog?.Impulse / Settings.ImpulsePerkWh;
                    AutoMeasurementLog.Cost = AutoMeasurementLog.Power * Settings.ElectricityPricePerkWh;
                    AutoMeasurementLog.CurrentMeasurement.Impulse++;
                    AutoMeasurementLog.CurrentMeasurement.Power = (float)AutoMeasurementLog?.CurrentMeasurement?.Impulse / Settings.ImpulsePerkWh;
                }

                // Neuer Messwert
                if ((DateTime.Now - AutoMeasurementLog.CurrentMeasurement.MeasurementTimePoint).TotalMilliseconds > 60000)
                {
                    AutoMeasurementLog.Measurements.Add(new MeasurementItem() { MeasurementTimePoint = DateTime.Now });

                    while (AutoMeasurementLog.Measurements.Count > AutoBufferSize)
                    {
                        AutoMeasurementLog.Measurements.RemoveAt(0);
                    }
                    var sum = AutoMeasurementLog.Measurements.Sum(x => x.Impulse);
                    if (sum > AutoThreshold && !ActiveCharging)
                    {
                        StartsTheChargingProcess();
                    }
                    else if (sum <= AutoThreshold && ActiveCharging)
                    {
                        StopsCharging();
                    }
                }

                if (CurrentMeasurementLog == null)
                {
                    return;
                }

                if (_lastMetering != DateTime.MinValue)
                {
                    if (pulse)
                    {
                        CurrentMeasurementLog.Impulse++;
                        CurrentMeasurementLog.Power = (float)CurrentMeasurementLog?.Impulse / Settings.ImpulsePerkWh;
                        CurrentMeasurementLog.Cost = CurrentMeasurementLog.Power * Settings.ElectricityPricePerkWh;
                        CurrentMeasurementLog.CurrentMeasurement.Impulse++;
                        CurrentMeasurementLog.CurrentMeasurement.Power = (float)CurrentMeasurementLog?.CurrentMeasurement?.Impulse / Settings.ImpulsePerkWh;
                    }

                    LastPowerMeterStatus = PowerMeterStatus;

                    // Neuer Messwert
                    if ((DateTime.Now - CurrentMeasurementLog.CurrentMeasurement.MeasurementTimePoint).TotalMilliseconds > 60000)
                    {
                        CurrentMeasurementLog.Measurements.Add(new MeasurementItem()
                        {
                            MeasurementTimePoint = DateTime.Now
                        });

                        if
                        (
                           Settings.MinWattage >= 0 &&
                           CurrentMeasurementLog?.Power >= 0.5 &&
                           CurrentMeasurementLog?.CurrentMeasurement?.Power <= Settings.MinWattage
                        )
                        {
                            Log(new LogItem(LogItem.LogLevel.Info, "Minimale Leistungsaufnahme wurde erreicht"));

                            StopsCharging();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Error, "Fehler bei Ermittlung des aktuellen Verbrauchs"));
                Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
            }

            _lastMetering = DateTime.Now;

            if (ActiveCharging && Settings.MaxChargingTime > 0 && (DateTime.Now - CurrentMeasurementLog.From).TotalSeconds > Settings.MaxChargingTime * 60 * 60)
            {
                Log(new LogItem(LogItem.LogLevel.Info, "Maximale Ladedauer wurde erreicht"));

                StopsCharging();
                return;
            }

            if (ActiveCharging && Settings.MaxWattage > 0 && CurrentMeasurementLog.Power > Settings.MaxWattage)
            {
                Log(new LogItem(LogItem.LogLevel.Info, "Maximaler Stromverbrauch wurde erreicht"));

                StopsCharging();
                return;
            }
        }

        /// <summary>
        /// Loggt ein Event
        /// </summary>
        /// <param name="logItem">Der Logeintrag</param>
        public void Log(LogItem logItem)
        {
            Logging.Add(logItem);

            if (ActiveCharging &&
                logItem.Level != LogItem.LogLevel.Info &&
                logItem.Level != LogItem.LogLevel.Debug)
            {
                var current = CurrentMeasurementLog?.CurrentMeasurement;
                current.Logitems.Add(logItem);
            }

            switch (logItem.Level)
            {
                case LogItem.LogLevel.Info:
                    Context.Log.Info(logItem.Instance, logItem.Massage);
                    break;
                case LogItem.LogLevel.Debug:
                    Context.Log.Debug(logItem.Instance, logItem.Massage);
                    break;
                case LogItem.LogLevel.Warning:
                    Context.Log.Warning(logItem.Instance, logItem.Massage);
                    break;
                case LogItem.LogLevel.Error:
                    Context.Log.Error(logItem.Instance, logItem.Massage);
                    break;
                case LogItem.LogLevel.Exception:
                    Context.Log.Error(logItem.Instance, logItem.Massage);
                    break;
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Speichern der Einstellungen erfolgen soll
        /// </summary>
        public void SaveSettings()
        {
            Log(new LogItem(LogItem.LogLevel.Info, "Einstellungen werden gespeichert"));

            // Konfiguration speichern
            var serializer = new XmlSerializer(typeof(Settings));

            using var memoryStream = new MemoryStream();
            serializer.Serialize(memoryStream, Settings);

            var utf = new UTF8Encoding();

            File.WriteAllText
            (
                Path.Combine(Context.Host.ConfigPath, "vila.settings.xml"),
                utf.GetString(memoryStream.ToArray())
            );
        }

        /// <summary>
        /// Wird aufgerufen, wenn die Einstellungen zurückgesetzt werden sollen
        /// </summary>
        public void ResetSettings()
        {
            Log(new LogItem(LogItem.LogLevel.Info, "Einstellungen werden geladen"));

            // Konfiguration laden
            var serializer = new XmlSerializer(typeof(Settings));

            try
            {
                using var reader = File.OpenText(Path.Combine(Context.Host.ConfigPath, "vila.settings.xml"));
                Settings = serializer.Deserialize(reader) as Settings;
            }
            catch
            {
                Log(new LogItem(LogItem.LogLevel.Warning, "Datei mit den Einstellungen wurde nicht gefunden!"));
            }

            Log(new LogItem(LogItem.LogLevel.Debug, "ImpulsePerkWh = " + Settings.ImpulsePerkWh));
        }

        /// <summary>
        /// Startet den Ladevorgang.
        /// </summary>
        public void StartsTheChargingProcess()
        {
            Log(new LogItem(LogItem.LogLevel.Info, "Startet den Ladevorgang"));

            CurrentMeasurementLog = new MeasurementLog()
            {
                ID = Guid.NewGuid().ToString(),
                From = DateTime.Now,
                Measurements = new List<MeasurementItem>()
            };

            // Initialer Messwert
            CurrentMeasurementLog.Measurements.Add(new MeasurementItem()
            {
                MeasurementTimePoint = DateTime.Now
            });

            ElectricContactorStatus = true;
        }

        /// <summary>
        /// Beendet den Ladevorgang.
        /// </summary>
        public void StopsCharging()
        {
            Log(new LogItem(LogItem.LogLevel.Info, "Beendet den Ladevorgang"));

            CurrentMeasurementLog.Till = DateTime.Now;

            // Messung speichern
            var serializer = new XmlSerializer(typeof(MeasurementLog));

            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(memoryStream, CurrentMeasurementLog);

                var utf = new UTF8Encoding();
                var fileName = Path.Combine(Context.Host.AssetPath, "measurements", string.Format("{0}.xml", CurrentMeasurementLog.ID));

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                File.WriteAllText
                (
                    fileName,
                    utf.GetString(memoryStream.ToArray())
                );

                Log(new LogItem(LogItem.LogLevel.Info, string.Format("Messprotokoll wurde unter {0} gespeichert", fileName)));
            }

            CurrentMeasurementLog = null;
            _lastMetering = DateTime.MinValue;

            ElectricContactorStatus = false;
        }

        /// <summary>
        /// Liefert die abgeschlossenen Messprotokolle
        /// </summary>
        /// <param name="from">Die Anfang, in welcher die Messprotokolle geliefert werden sollen</param>
        /// /// <param name="till">Das Ende, in welcher die Messprotokolle geliefert werden sollen</param>
        public List<MeasurementLog> GetHistoryMeasurementLogs(DateTime from, DateTime till)
        {
            var list = new List<MeasurementLog>();
            var directoryName = Path.Combine(Context.Host.AssetPath, "measurements");

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var files = Directory.GetFiles(directoryName, "*.xml");
            var serializer = new XmlSerializer(typeof(MeasurementLog));

            foreach (var file in files)
            {
                using var reader = File.OpenText(file);
                list.Add(serializer.Deserialize(reader) as MeasurementLog);
            }

            return list.Where(x => x.Till >= from && x.Till <= till).OrderByDescending(x => x.Till).ToList();
        }

        /// <summary>
        /// Liefert die abgeschlossenen Messprotokolle
        /// </summary>
        /// <param name="id">Die ID des Messprotokolls</param>
        public MeasurementLog GetHistoryMeasurementLogs(string id)
        {
            var list = new List<MeasurementLog>();
            var directoryName = Path.Combine(Context.Host.AssetPath, "measurements");
            var files = Directory.GetFiles(directoryName, id + ".xml");
            var serializer = new XmlSerializer(typeof(MeasurementLog));

            foreach (var file in files)
            {
                using var reader = File.OpenText(file);
                list.Add(serializer.Deserialize(reader) as MeasurementLog);
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// Liefert die abgeschlossenen Messprotokolle
        /// </summary>
        public List<MeasurementLog> GetHistoryMeasurementLogs()
        {
            var list = new List<MeasurementLog>();
            var directoryName = Path.Combine(Context.Host.AssetPath, "measurements");
            var files = Directory.GetFiles(directoryName, "*.xml");
            var serializer = new XmlSerializer(typeof(MeasurementLog));

            foreach (var file in files)
            {
                using var reader = File.OpenText(file);
                list.Add(serializer.Deserialize(reader) as MeasurementLog);
            }

            return list;
        }
    }
}