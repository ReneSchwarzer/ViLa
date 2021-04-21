using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WebExpress.Internationalization;
using WebExpress.Plugin;

namespace ViLa.Model
{
    public class ViewModel : II18N
    {
        /// <summary>
        /// Die Größe des Autobuffers in Minuten
        /// </summary>
        public const int ContinuousLogSize = 5;

        /// <summary>
        /// Der Schwellwert in Impulsen
        /// </summary>
        public const int ContinuousThreshold = 50;

        /// <summary>
        /// Impulsdauer im ms 
        /// </summary>
        public const int ImpulseDuration = 30;

        /// <summary>
        /// Der GPIO-Pin, welcher die S0-Schnittstelle des Strommeßgerät ausließt
        /// </summary>
        private const int PowerMeterPin = 3;

        /// <summary>
        /// Der GPIO-Pin, welcher den Schütz steuert
        /// </summary>
        private const int ElectricContactorPin = 13;

        /// <summary>
        /// Lifert die einzige Instanz der Modell-Klasse
        /// </summary>
        public static ViewModel Instance { get; } = new ViewModel();

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
        /// Liefert oder setzt die Zeit des letzen Auslesens
        /// </summary>
        private Stopwatch Stopwatch { get; } = new Stopwatch();

        /// <summary>
        /// Liefert oder setzt Startzeitpunkt
        /// </summary>
        private DateTime StartTime { get; set; }

        /// <summary>
        /// Liefert oder setzt die vergangenen Minuten
        /// </summary>
        private long PastMinutes { get; set; } = 0;

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
                            GPIO.Write(ElectricContactorPin, PinValue.High);
                            Log(new LogItem(LogItem.LogLevel.Debug, this.I18N("vila.log.electriccontactorstatus.high")));
                        }
                        else
                        {
                            GPIO.Write(ElectricContactorPin, PinValue.Low);
                            Log(new LogItem(LogItem.LogLevel.Debug, "vila.log.electriccontactorstatus.low"));
                        }

                        _electricContactorStatus = value;
                    }
                }
                catch (Exception ex)
                {
                    Log(new LogItem(LogItem.LogLevel.Error, this.I18N("vila.log.electriccontactorstatus.error")));
                    Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }
            }
        }

        /// <summary>
        /// Liefert oder setzt ob ein GPIO-Impuls anliegt 
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
                    Log(new LogItem(LogItem.LogLevel.Error, this.I18N("vila.log.powermeterstatus.error")));
                    Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
                }

                return false;
            }
        }

        /// <summary>
        /// Liefert oder setzt den letzten Status der GPIO-Schnittstelle
        /// </summary>
        private bool LastPowerMeterStatus { get; set; }

        /// <summary>
        /// Bestimmt, ob der Ladevorgang aktiv ist
        /// </summary>
        public bool ActiveCharging => ActiveMeasurementLog != null;

        /// <summary>
        /// Messprotokoll der ständigen Messung
        /// </summary>
        private MeasurementLog ContinuousMeasurementLog { get; } = new MeasurementLog()
        {
            ID = Guid.NewGuid().ToString(),
            Measurements = new List<MeasurementItem>() { new MeasurementItem() { MeasurementTimePoint = DateTime.Now } }
        };

        /// <summary>
        /// Aktuelles Messprotokoll
        /// </summary>
        public MeasurementLog CurrentMeasurementLog => ActiveCharging ? ActiveMeasurementLog : ContinuousMeasurementLog;

        /// <summary>
        /// Ermittelt die aktuell ermittelte Leistung der letzen Minute in kWh
        /// </summary>
        public float CurrentPower => CurrentMeasurementLog.Measurements.Count > 0 ? CurrentMeasurementLog.CurrentPower : 0;

        /// <summary>
        /// Liefert oder setzt das aktive Messprotokoll
        /// </summary>
        private MeasurementLog ActiveMeasurementLog { get; set; }

        /// <summary>
        /// Liefert die bereits abgeschlossene Messprotokolle
        /// </summary>
        private List<MeasurementLog> HistoryMeasurementLog { get; } = new List<MeasurementLog>();

        /// <summary>
        /// Liefert oder setzt die Settings
        /// </summary>
        public Settings Settings { get; private set; } = new Settings() { Currency = "€" };

        /// <summary>
        /// Liefert die Kultur
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Liefert den I18N-Key
        /// </summary>
        public string I18N_PluginID => Context.PluginID;

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
                GPIO.OpenPin(PowerMeterPin, PinMode.InputPullUp);
                GPIO.OpenPin(ElectricContactorPin, PinMode.Output);

                GPIO.Write(ElectricContactorPin, PinValue.High);
                _electricContactorStatus = false;

                Log(new LogItem(LogItem.LogLevel.Info, this.I18N("vila.log.init.gpio")));
                Log(new LogItem(LogItem.LogLevel.Debug, "ElectricContactorPin " + ElectricContactorPin));
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
            }


            // Alte Messprotokolle laden
            var directoryName = Path.Combine(Context.Host.AssetPath, "measurements");

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

            // ganz alte Messprotokolle archivieren (zyklisch)
            Task.Run(() =>
            {
                while (true)
                {
                    foreach (var his in HistoryMeasurementLog.Where(x => x.Till < DateTime.Now.AddYears(-1)).ToList())
                    {
                        ArchiveHistoryMeasurementLog(his.ID);
                    }

                    Thread.Sleep(1000 * 60 * 60 * 24);
                }
            });

            Culture = Context.Host.Culture;

            ResetSettings();

            StartTime = DateTime.Now;
        }

        /// <summary>
        /// Updatefunktion
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
                    Log(new LogItem(LogItem.LogLevel.Warning, string.Format(Context.Host.Culture, this.I18N("vila.log.update.exceeding"), delta - ViewModel.ImpulseDuration)));
                }

                if (Stopwatch.IsRunning)
                {
                    var minutes = (DateTime.Now - StartTime).Minutes;

                    if (pulse)
                    {
                        ContinuousMeasurementLog.CurrentMeasurement.Impulse++;
                        ContinuousMeasurementLog.CurrentMeasurement.Power = (float)ContinuousMeasurementLog?.CurrentMeasurement?.Impulse / Settings.ImpulsePerkWh;
                    }
                                                   
                    // Neuer Messwert
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
                        if (impulse > ContinuousThreshold && !ActiveCharging && Settings.Auto)
                        {
                            StartCharging();

                            // Bestimme den ersten Meßwert mit Werten
                            var skip = 0;
                            for (var i = 0; i < ContinuousMeasurementLog.Measurements.Count; i++)
                            {
                                if (ContinuousMeasurementLog.Measurements[i].Impulse > 0)
                                {
                                    skip = i;
                                    break;
                                }
                            }

                            // Bereits verbrauchte Energie welche zur Dedektierung der Autofunktion gemessen wurde, dem neuen Messprotokoll zuschreiben
                            var measurements = ContinuousMeasurementLog.Measurements.Skip(skip);
                            ActiveMeasurementLog.Measurements.Clear();
                            ActiveMeasurementLog.Measurements.AddRange(measurements.SkipLast(1));

                            return;
                        }
                        else if (impulse <= ContinuousThreshold && ActiveCharging && Settings.Auto)
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
                            Log(new LogItem(LogItem.LogLevel.Info, this.I18N("vila.charging.min")));

                            StopCharging();
                            return;
                        }
                    }
                }

                if (ActiveCharging && Settings.MaxChargingTime > 0 && (DateTime.Now - ActiveMeasurementLog.From).TotalSeconds > Settings.MaxChargingTime * 60 * 60)
                {
                    Log(new LogItem(LogItem.LogLevel.Info, this.I18N("vila.charging.time.max")));

                    StopCharging();
                    return;
                }

                if (ActiveCharging && Settings.MaxWattage > 0 && ActiveMeasurementLog.Power > Settings.MaxWattage)
                {
                    Log(new LogItem(LogItem.LogLevel.Info, this.I18N("vila.charging.consumption.max")));

                    StopCharging();
                    return;
                }
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Error, this.I18N("vila.charging.error")));
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
        /// <param name="logItem">Der Logeintrag</param>
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

            switch (logItem.Level)
            {
                case LogItem.LogLevel.Info:
                    Context.Log.Info(logItem.Massage, logItem.Instance);
                    break;
                case LogItem.LogLevel.Debug:
                    Context.Log.Debug(logItem.Massage, logItem.Instance);
                    break;
                case LogItem.LogLevel.Warning:
                    Context.Log.Warning(logItem.Massage, logItem.Instance);
                    break;
                case LogItem.LogLevel.Error:
                    Context.Log.Error(logItem.Massage, logItem.Instance);
                    break;
                case LogItem.LogLevel.Exception:
                    Context.Log.Error(logItem.Massage, logItem.Instance);
                    break;
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Speichern der Einstellungen erfolgen soll
        /// </summary>
        public void SaveSettings()
        {
            Log(new LogItem(LogItem.LogLevel.Info, this.I18N("vila.setting.save")));

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
            Log(new LogItem(LogItem.LogLevel.Info, this.I18N("vila.setting.load")));

            // Konfiguration laden
            var serializer = new XmlSerializer(typeof(Settings));

            try
            {
                using var reader = File.OpenText(Path.Combine(Context.Host.ConfigPath, "vila.settings.xml"));
                Settings = serializer.Deserialize(reader) as Settings;
            }
            catch
            {
                Log(new LogItem(LogItem.LogLevel.Warning, this.I18N("vila.setting.warning")));
            }

            Log(new LogItem(LogItem.LogLevel.Debug, "ImpulsePerkWh = " + Settings.ImpulsePerkWh));
        }

        /// <summary>
        /// Startet den Ladevorgang.
        /// </summary>
        public void StartCharging()
        {
            Log(new LogItem(LogItem.LogLevel.Info, this.I18N("vila.charging.begin")));

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
            Log(new LogItem(LogItem.LogLevel.Info, this.I18N("vila.charging.stop")));

            ActiveMeasurementLog.FinalPower = ActiveMeasurementLog.Power;
            ActiveMeasurementLog.FinalCost = ActiveMeasurementLog.Cost;
            ActiveMeasurementLog.FinalFrom = ActiveMeasurementLog.From;
            ActiveMeasurementLog.FinalTill = DateTime.Now;
            ActiveMeasurementLog.ElectricityPricePerkWh = Settings.ElectricityPricePerkWh;
            ActiveMeasurementLog.ImpulsePerkWh = Settings.ImpulsePerkWh;
            ActiveMeasurementLog.Currency = Settings.Currency;

            // Messung speichern
            var serializer = new XmlSerializer(typeof(MeasurementLog));
            var xmlns = new XmlSerializerNamespaces();
            xmlns.Add(string.Empty, string.Empty);

            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(memoryStream, ActiveMeasurementLog, xmlns);

                var utf = new UTF8Encoding();
                var fileName = Path.Combine(Context.Host.AssetPath, "measurements", string.Format("{0}.xml", ActiveMeasurementLog.ID));

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

                Log(new LogItem(LogItem.LogLevel.Info, string.Format(this.I18N("vila.charging.save"), fileName)));
            }

            ActiveMeasurementLog = null;
            ElectricContactorStatus = false;

            Stopwatch.Restart();
        }

        /// <summary>
        /// Liefert die abgeschlossenen Messprotokolle
        /// </summary>
        /// <param name="from">Die Anfang, in welcher die Messprotokolle geliefert werden sollen</param>
        /// <param name="till">Das Ende, in welcher die Messprotokolle geliefert werden sollen</param>
        /// <return>Messprotokolle, welche sich innerhalb der gegebenen Zeitspanne befinden</return>
        public IEnumerable<MeasurementLog> GetHistoryMeasurementLogs(DateTime from, DateTime till)
        {
            return HistoryMeasurementLog.Where(x => x.Till >= from && x.Till <= till).OrderByDescending(x => x.Till);
        }

        /// <summary>
        /// Liefert alle abgeschlossenen Messprotokolle
        /// </summary>
        /// <return>Alle gespeicherten Messprotokoll</return>
        public IEnumerable<MeasurementLog> GetHistoryMeasurementLogs()
        {
            return HistoryMeasurementLog;
        }

        /// <summary>
        /// Liefert ein abgeschlossenes Messprotokoll
        /// </summary>
        /// <param name="id">Die ID des Messprotokolls</param>
        /// <return>Das Messprokoll oder null</return>
        public MeasurementLog GetHistoryMeasurementLog(string id)
        {
            return HistoryMeasurementLog.Where(x => x.ID.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Löscht ein abgeschlossenes Messprotokoll
        /// </summary>
        /// <param name="id">Die ID des Messprotokolls</param>
        public void RemoveHistoryMeasurementLog(string id)
        {
            try
            {
                var measurementLog = GetHistoryMeasurementLog(id);
                if (measurementLog != null)
                {
                    File.Delete(System.IO.Path.Combine(Context.Host.AssetPath, "measurements", $"{measurementLog.ID}.xml"));
                    ViewModel.Instance.Logging.Add(new LogItem(LogItem.LogLevel.Info, string.Format(this.I18N("vila.delete.file"), id)));

                    HistoryMeasurementLog.Remove(measurementLog);
                }
                else
                {
                    Log(new LogItem(LogItem.LogLevel.Info, string.Format(this.I18N("vila.delete.error"), id)));
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
                    var archive = Path.Combine(ViewModel.Instance.Context.Host.AssetPath, "archive");

                    if (!Directory.Exists(archive))
                    {
                        Directory.CreateDirectory(archive);
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

                    var source = Path.Combine(Context.Host.AssetPath, "measurements", id + ".xml");
                    var destination = Path.Combine(month, id + ".xml");

                    File.Move(source, destination);

                    HistoryMeasurementLog.Remove(measurementLog);

                    Log(new LogItem(LogItem.LogLevel.Info, string.Format(this.I18N("vila.archive.move"), id)));
                }
                else
                {
                    Log(new LogItem(LogItem.LogLevel.Info, string.Format(this.I18N("vila.archive.error"), id)));
                }
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
            }
        }
    }
}