using ViLa.Model;
using ViLa.WebPage;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;

namespace ViLa.WebFragment
{
    [Section(Section.PropertyPreferences)]
    [Module<Module>]
    [Scope<PageDashboard>]
    [Scope<PageHistory>]
    public sealed class FragmentPropertyCharging : FragmentControlFormularInline
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentPropertyCharging()
            : base("charging")
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

            ProcessFormular += OnProcessFormular;
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            RedirectUri = context.Uri;
            Items.Clear();

            if (!ViewModel.Instance.ActiveCharging)
            {
                SubmitButton.Text = "vila:vila.charging.begin";
                SubmitButton.Icon = new PropertyIcon(TypeIcon.PlayCircle);
                SubmitButton.Color = new PropertyColorButton(TypeColorButton.Success);
                Items.Add(new ControlFormItemInputHidden("state") { Value = "off" });
            }
            else
            {
                SubmitButton.Text = "vila:vila.charging.stop";
                SubmitButton.Color = new PropertyColorButton(TypeColorButton.Danger);
                SubmitButton.Icon = new PropertyIcon(TypeIcon.StopCircle);
                Items.Add(new ControlFormItemInputHidden("state") { Value = "on" });
            }

            return base.Render(context);
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Formular abgeschlossen werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnProcessFormular(object sender, FormularEventArgs e)
        {
            if (!ViewModel.Instance.ActiveCharging)
            {
                ViewModel.Instance.StartCharging();
            }
            else
            {
                ViewModel.Instance.StopCharging();
            }

            e.Context.Page.Redirecting(e.Context.Uri);
        }
    }
}
