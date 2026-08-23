using System;
using System.Globalization;
using ViLa.Model;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebIcon;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebControl
{
    /// <summary>
    /// Represents a card with its associated measurement log.
    /// Layout matches ControlCardCounter, but the header is a formatted string
    /// (e.g. "13.70 kWh / 3.42 €") instead of a single int.
    /// </summary>
    public class ControlCardMeasurementLog : Control
    {
        /// <summary>
        /// Gets or sets the measurement log.
        /// </summary>
        public MeasurementLog MeasurementLog { get; set; }

        /// <summary>
        /// Optional icon override. Defaults to a tachometer.
        /// </summary>
        public Func<IRenderControlContext, IIcon> Icon { get; set; }

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
        }

        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var icon = Icon?.Invoke(renderContext);
            var ml = MeasurementLog;

            // 0.0.11 migration: ImpulsePerkWh=0 makes Power=Infinity/NaN; clamp to 0..100% (x86: (uint?)Infinity -> uint.MaxValue).
            uint? progressValue = null;
            if (ml?.Power is float power && !float.IsNaN(power) && !float.IsInfinity(power) && power >= 0f)
            {
                progressValue = (uint)Math.Min(power, 100f);
            }

            // Header text: "13.70 kWh / 3.42 €" (or null if no log)
            string headerText = null;
            if (ml != null)
            {
                var culture = renderContext?.Request?.Culture ?? CultureInfo.CurrentCulture;
                var finalCost = float.IsNaN(ml.FinalCost) || float.IsInfinity(ml.FinalCost) ? 0f : ml.FinalCost;
                var finalPower = float.IsNaN(ml.FinalPower) || float.IsInfinity(ml.FinalPower) ? 0f : ml.FinalPower;
                headerText = string.Format(culture, "{0:F2} kWh / {1:F2} {2}", finalPower, finalCost, ml.Currency);
            }

            // Body text: date, time range, tag badge, details link.
            string bodyText = null;
            if (ml != null)
            {
                var culture = renderContext.Request.Culture ?? CultureInfo.CurrentCulture;
                var shortDate = ml.From.ToString(culture.DateTimeFormat.ShortDatePattern);
                var finalFrom = ml.FinalFrom.ToString(culture.DateTimeFormat.LongTimePattern);
                var finalTill = ml.FinalTill.ToString(culture.DateTimeFormat.LongTimePattern);
                var labelTime = WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(culture, "vila:vila.charging.time");
                var labelDetails = WebExpress.WebCore.WebEx.ComponentHub.InternationalizationManager.Translate(culture, "vila:vila.charging.details");
                var detailsUri = WebExpress.WebCore.WebEx.ComponentHub.SitemapManager.GetUri<WWW.Details.Index>(renderContext.Request.ApplicationContext, new WebExpress.WebCore.WebParameter.ParameterId(ml.ID));

                var tagHtml = "";
                if (!string.IsNullOrWhiteSpace(ml.Tag))
                {
                    var color = ViewModel.Instance.GetColor(ml.Tag);
                    tagHtml = $"<br/><span class=\"badge\" style=\"background-color: {color}; color: #fff;\">{ml.Tag}</span>";
                }

                bodyText = $"{shortDate} <small class=\"text-muted\">({finalFrom} - {finalTill} {labelTime})</small>{tagHtml}<br/><a href=\"{detailsUri}\">{labelDetails}</a>";
            }

            var html = new HtmlElementTextSemanticsSpan()
            {
                Id = Id,
                Class = Css.Concatenate("card-counter", GetClasses(renderContext)),
                Style = GetStyles(renderContext),
                Role = Role?.Invoke(renderContext)
            };

            if (icon != null)
            {
                html.Add(new ControlIcon()
                {
                    Icon = _ => icon,
                    TextColor = TextColor,
                    HorizontalAlignment = _ => TypeHorizontalAlignment.Right
                }.Render(renderContext, visualTree));
            }

            var header = new ControlText(string.IsNullOrWhiteSpace(Id) ? null : Id + "_header")
            {
                Text = _ => headerText,
                Format = _ => TypeFormatText.H4
            };

            var info = new ControlText()
            {
                Text = _ => bodyText,
                Format = _ => TypeFormatText.Span,
                TextColor = _ => new PropertyColorText(TypeColorText.Muted)
            };

            html.Add(new ControlPanel(null, header, info) { }.Render(renderContext, visualTree));

            if (progressValue.HasValue)
            {
                html.Add(new ControlProgress()
                {
                    Value = _ => progressValue.Value,
                    Format = _ => TypeFormatProgress.Striped,
                    BackgroundColor = BackgroundColor,
                    Size = _ => TypeSizeProgress.Small
                }.Render(renderContext, visualTree));
            }

            return html;
        }
    }
}
