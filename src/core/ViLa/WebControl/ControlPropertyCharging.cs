using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.Attribute;
using WebExpress.UI.Component;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Components;

namespace ViLa.WebControl
{
    [Section(Section.PropertyPreferences)]
    [Application("ViLa")]
    [Context("dashboard")]
    [Context("history")]
    public sealed class ControlPropertyCharging : ControlFormularInline, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlPropertyCharging()
            : base("charging")
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            RedirectUri = context.Uri;

            if (!ViewModel.Instance.ActiveCharging)
            {
                SubmitButton.Text = context.Page.I18N("vila.charging.begin");
                SubmitButton.Icon = new PropertyIcon(TypeIcon.PlayCircle);
                SubmitButton.Color = new PropertyColorButton(TypeColorButton.Success);
                Items.Add(new ControlFormularItemInputHidden("state") { Value = "off" });
            }
            else
            {
                SubmitButton.Text = context.I18N("vila.charging.stop");
                SubmitButton.Color = new PropertyColorButton(TypeColorButton.Danger);
                SubmitButton.Icon = new PropertyIcon(TypeIcon.StopCircle);
                Items.Add(new ControlFormularItemInputHidden("state") { Value = "on" });
            }

            ProcessFormular += (s, e) =>
            {
                if (!ViewModel.Instance.ActiveCharging)
                {
                    ViewModel.Instance.StartCharging();
                }
                else
                {
                    ViewModel.Instance.StopCharging();
                }

                context.Page.Redirecting(context.Uri);
            };

            return base.Render(context);
        }
    }
}
