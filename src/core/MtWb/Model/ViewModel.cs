using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Xml.Serialization;
using WebExpress;

namespace MtWb.Model
{
    public class ViewModel
    {
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
        public string Now => DateTime.Now.ToString("dd.MM.yyyy<br>HH:mm:ss");

        /// <summary>
        /// Liefert oder setzt den Verweis auf dem Host
        /// </summary>
        public IHost Host { get; set; }

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
        private DateTime _lastMetering;

        /// <summary>
        /// Der Zustand des GPIO-Pins, welcher den Schütz steuert
        /// </summary>
        private bool _electricContactorStatus;

        /// <summary>
        /// Liefert oder setzt ob der Schütz angeschaltet ist
        /// </summary>
        public virtual bool ElectricContactorStatus
        {
            get => _electricContactorStatus;
            set
            {
                try
                {
                    if (value != _electricContactorStatus)
                    {
                        if (value)
                        {
                            GPIO.Write(_electricContactorPin, PinValue.High);
                            Log(new LogItem(LogItem.LogLevel.Error, "Status des Schütz wurde auf HIGH geändert"));
                        }
                        else
                        {
                            GPIO.Write(_electricContactorPin, PinValue.Low);
                            Log(new LogItem(LogItem.LogLevel.Error, "Status des Schütz wurde auf LOW"));
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
        public virtual bool PowerMeterStatus
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

                ElectricContactorStatus = true;

                Log(new LogItem(LogItem.LogLevel.Info, "GpioController gestartet"));
                Log(new LogItem(LogItem.LogLevel.Debug, "ElectricContactorPin " + _electricContactorPin));
            }
            catch
            {

            }
        }

        /// <summary>
        /// Updatefunktion
        /// </summary>
        public virtual void Update()
        {
            try
            {
                if (_lastMetering == null || (DateTime.Now - _lastMetering).TotalSeconds > 60)
                {
                    _lastMetering = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Log(new LogItem(LogItem.LogLevel.Error, "Fehler bei Ermittlung des aktuellen Verbrauchs"));
                Log(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
            }

        }

        /// <summary>
        /// Loggt ein Event
        /// </summary>
        /// <param name="logItem">Der Logeintrag</param>
        public void Log(LogItem logItem)
        {
            Logging.Add(logItem);

            switch (logItem.Level)
            {
                case LogItem.LogLevel.Info:
                    //Host.Context.Log.Info(logItem.Instance, logItem.Massage);
                    break;
                case LogItem.LogLevel.Debug:
                    //Host.Context.Log.Debug(logItem.Instance, logItem.Massage);
                    break;
                case LogItem.LogLevel.Warning:
                    Host.Context.Log.Warning(logItem.Instance, logItem.Massage);
                    break;
                case LogItem.LogLevel.Error:
                    Host.Context.Log.Error(logItem.Instance, logItem.Massage);
                    break;
                case LogItem.LogLevel.Exception:
                    Host.Context.Log.Exception(logItem.Instance, logItem.Massage);
                    break;
            }
        }
    }
}