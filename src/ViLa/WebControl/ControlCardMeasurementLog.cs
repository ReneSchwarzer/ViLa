using System;
using ViLa.Model;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;

namespace ViLa.WebControl
{
    /// <summary>  
    /// Represents a card with its associated measurement log.  
    /// </summary>
    public class ControlCardMeasurementLog : ControlCardCounter
    {
        /// <summary>
        /// Gets or sets the measurement log.
        /// </summary>
        public MeasurementLog MeasurementLog { get; set; }

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>  
        /// <param name="id">The ID of the measurement log. May be null.</param>
        public ControlCardMeasurementLog(string id = null)
            : base(id)
        {
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
            Icon = _ => new IconTachometerAlt();
            TextColor = _ => new PropertyColorText(TypeColorText.Default);
            BackgroundColor = _ => new PropertyColorBackground(TypeColorBackground.Light);
            // 0.0.11 MIGRATION: Wenn Settings.ImpulsePerkWh = 0 ist, ergibt Power = Infinity/NaN.
            // Cast (uint?)Infinity läuft auf x86 in uint.MaxValue (4294967295%) über.
            // Wir klammern ungültige Werte auf 0..100% ab.
            Progress = _ =>
            {
                var power = MeasurementLog?.Power;
                if (power is null || float.IsNaN(power.Value) || float.IsInfinity(power.Value) || power.Value < 0)
                {
                    return (uint?)0;
                }

                return (uint?)Math.Min(power.Value, 100f);
            };

            //Value = _ =>
            //{
            //    if (MeasurementLog == null) return "-";
            //    return $"{string.Format("{0:F2} kWh", MeasurementLog.FinalPower)} / {string.Format("{0:F2} {1}", MeasurementLog.FinalCost, MeasurementLog.Currency)}";
            //};

            Text = renderContext =>
            {
                if (MeasurementLog == null) return string.Empty;

                var culture = renderContext.Request.Culture ?? System.Globalization.CultureInfo.CurrentCulture;
                var shortDate = MeasurementLog.From.ToString(culture.DateTimeFormat.ShortDatePattern);
                var finalFrom = MeasurementLog.FinalFrom.ToString(culture.DateTimeFormat.LongTimePattern);
                var finalTill = MeasurementLog.FinalTill.ToString(culture.DateTimeFormat.LongTimePattern);
                var labelTime = WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(culture, "vila:vila.charging.time");
                var labelDetails = WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(culture, "vila:vila.charging.details");
                var detailsUri = WebExpress.WebCore.WebEx.ComponentHub.SitemapManager.GetUri<WWW.Details.Index>(renderContext.Request.ApplicationContext, new WebExpress.WebCore.WebParameter.ParameterId(MeasurementLog.ID));

                var tagHtml = "";
                if (!string.IsNullOrWhiteSpace(MeasurementLog.Tag))
                {
                    var color = ViewModel.Instance.GetColor(MeasurementLog.Tag);
                    tagHtml = $"<br/><span class=\"badge\" style=\"background-color: {color}; color: #fff;\">{MeasurementLog.Tag}</span>";
                }

                return $"{shortDate} <small class=\"text-muted\">({finalFrom} - {finalTill} {labelTime})</small>{tagHtml}<br/><a href=\"{detailsUri}\">{labelDetails}</a>";
            };
        }
    }
}
