using ViLa.Model;
using ViLa.WebControl;
using ViLa.WebPage;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebAttribute;
using WebExpress.WebHtml;
using WebExpress.WebPage;
using WebExpress.WebUI.WebAttribute;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;

namespace ViLa.WebFragment
{
    [Section(Section.ContentPrimary)]
    [Module<Module>]
    [Scope<PageDetails>]
    public sealed class FragmentContentTag : FragmentControlPanel
    {
        /// <summary>
        /// Das Labelformular
        /// </summary>
        private ControlFormTag Form { get; } = new ControlFormTag("form_tag");

        /// <summary>
        /// Konstruktor
        /// </summary>
        public FragmentContentTag()
        {
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);

            Content.Add(Form);

            Form.FillFormular += OnFillFormular;
            Form.ProcessFormular += OnProcessFormular;
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Formular initialisiert wird.
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnFillFormular(object sender, FormularEventArgs e)
        {
            var id = e.Context.Request.GetParameter("id");
            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id?.Value);

            Form.Tag.Value = measurementLog.Tag;
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein nuer Kommentar gepeichert werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnProcessFormular(object sender, FormularEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Form.Tag.Value))
            {
                var id = e.Context.Request.GetParameter("id");
                var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id?.Value);

                measurementLog.Tag = Form.Tag.Value;

                ViewModel.Instance.UpdateMeasurementLog(measurementLog);
            }
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        /// <param name="page">Die Seite, indem die Komonente aktiv ist</param>
        public override void Initialization(IFragmentContext context, IPage page)
        {
            base.Initialization(context, page);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Form.RedirectUri = context.Uri;

            return base.Render(context);
        }
    }
}
