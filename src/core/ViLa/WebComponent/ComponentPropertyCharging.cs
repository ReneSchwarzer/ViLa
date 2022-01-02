using ViLa.Model;
using WebExpress.WebAttribute;
using WebExpress.Html;
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
    public sealed class ComponentPropertyCharging : ComponentControlFormularInline
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentPropertyCharging()
            : base("charging")
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
                Items.Add(new ControlFormularItemInputHidden("state") { Value = "off" });
            }
            else
            {
                SubmitButton.Text = "vila:vila.charging.stop";
                SubmitButton.Color = new PropertyColorButton(TypeColorButton.Danger);
                SubmitButton.Icon = new PropertyIcon(TypeIcon.StopCircle);
                Items.Add(new ControlFormularItemInputHidden("state") { Value = "on" });
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
