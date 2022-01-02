using System.Text;
using ViLa.Model;
using WebExpress.WebAttribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.WebAttribute;
using WebExpress.UI.WebComponent;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebComponent;
using WebExpress.WebPage;

namespace ViLa.WebComponent
{
    [Section(Section.PropertyPreferences)]
    [Application("ViLa")]
    [Context("dashboard")]
    [Context("history")]
    public sealed class ComponentPropertyCurrent : ComponentControlList
    {
        /// <summary>
        /// Liefert den Titel
        /// </summary>
        private ControlText Title = new ControlText()
        {
            Text = "vila:vila.charging.current",
            Format = TypeFormatText.H4,
            TextColor = new PropertyColorText(TypeColorText.Secondary)
        };

        /// <summary>
        /// Liefert die aktuelle Liestung
        /// </summary>
        private ControlAttribute CurrentPower = new ControlAttribute()
        {
            Icon = new PropertyIcon(TypeIcon.Bolt),
            Value = "",
            TextColor = new PropertyColorText(TypeColorText.Secondary)
        };

        private ControlText Description = new ControlText()
        {
            Text = "vila:vila.charging.current.description",
            Format = TypeFormatText.Small,
            TextColor = new PropertyColorText(TypeColorText.Secondary)
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentPropertyCurrent()
            : base("current")
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public override void Initialization(IComponentContext context)
        {
            base.Initialization(context);

            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
            Layout = TypeLayoutList.Flush;

            Items.Add(new ControlListItem(Title, CurrentPower, Description));
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            context.VisualTree.AddHeaderScriptLinks(context.Uri.Root.Append("/assets/js/vila.dashboard.js"));

            CurrentPower.Name = $"{ string.Format(context.Culture, "{0:F2}", ViewModel.Instance.CurrentPower) } kWh";

            var builder = new StringBuilder();
            builder.AppendLine($"var restUrl='{ context.Uri.Root.Append("api") }';");
            builder.AppendLine($"var currency='{ ViewModel.Instance.Settings.Currency }';");
            builder.AppendLine($"var vila_charging_current='{ context.Page.I18N("vila:vila.charging.current") }';");
            builder.AppendLine($"var vila_charging_begin='{ context.Page.I18N("vila:vila.charging.begin") }';");
            builder.AppendLine($"var vila_charging_stop='{ context.Page.I18N("vila:vila.charging.stop") }';");
            builder.AppendLine($"var vila_charging_duration='{ context.Page.I18N("vila:vila.charging.duration") }';");
            builder.AppendLine($"var vila_charging_cost='{ context.Page.I18N("vila:vila.charging.cost") }';");
            builder.AppendLine($"var vila_charging_consumption='{ context.Page.I18N("vila:vila.charging.consumption") }';");

            context.VisualTree.AddScript($"charging_i18n", builder.ToString());

            return base.Render(context);
        }
    }
}
