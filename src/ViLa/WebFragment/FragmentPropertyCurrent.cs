using System.Text;
using ViLa.Model;
using ViLa.WebPage;
using WebExpress.Internationalization;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebAttribute;
using WebExpress.WebHtml;
using WebExpress.WebPage;

namespace ViLa.WebFragment
{
    [Section(Section.PropertyPreferences)]
    [Module<Module>]
    [Scope<PageDashboard>]
    [Scope<PageHistory>]
    public sealed class FragmentPropertyCurrent : FragmentControlList
    {
        /// <summary>
        /// Liefert den Titel
        /// </summary>
        private ControlText Title { get; } = new ControlText()
        {
            Text = "vila:vila.charging.current",
            Format = TypeFormatText.H4,
            TextColor = new PropertyColorText(TypeColorText.Secondary)
        };

        /// <summary>
        /// Liefert die aktuelle Liestung
        /// </summary>
        private ControlAttribute CurrentPower { get; } = new ControlAttribute()
        {
            Icon = new PropertyIcon(TypeIcon.Bolt),
            Value = "",
            TextColor = new PropertyColorText(TypeColorText.Secondary)
        };

        private ControlText Description { get; } = new ControlText()
        {
            Text = "vila:vila.charging.current.description",
            Format = TypeFormatText.Small,
            TextColor = new PropertyColorText(TypeColorText.Secondary)
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentPropertyCurrent()
            : base("current")
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="page">Die Seite, indem die Komonente aktiv ist</param>
        public override void Initialization(IFragmentContext context, IPage page)
        {
            base.Initialization(context, page);

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
            context.VisualTree.AddHeaderScriptLinks(context.ApplicationContext.ContextPath.Append("/assets/js/vila.dashboard.js"));

            CurrentPower.Name = $"{string.Format(context.Culture, "{0:F2}", ViewModel.Instance.CurrentPower)} kWh";

            var builder = new StringBuilder();
            builder.AppendLine($"var restUrl='{context.ApplicationContext.ContextPath.Append("api")}';");
            builder.AppendLine($"var currency='{ViewModel.Instance.Settings.Currency}';");
            builder.AppendLine($"var vila_charging_current='{context.Page.I18N("vila:vila.charging.current")}';");
            builder.AppendLine($"var vila_charging_begin='{context.Page.I18N("vila:vila.charging.begin")}';");
            builder.AppendLine($"var vila_charging_stop='{context.Page.I18N("vila:vila.charging.stop")}';");
            builder.AppendLine($"var vila_charging_duration='{context.Page.I18N("vila:vila.charging.duration")}';");
            builder.AppendLine($"var vila_charging_cost='{context.Page.I18N("vila:vila.charging.cost")}';");
            builder.AppendLine($"var vila_charging_consumption='{context.Page.I18N("vila:vila.charging.consumption")}';");

            context.VisualTree.AddScript($"charging_i18n", builder.ToString());

            return base.Render(context);
        }
    }
}
