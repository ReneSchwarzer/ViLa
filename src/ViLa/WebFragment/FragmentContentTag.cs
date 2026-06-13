using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a fragment control panel for tag content within the details index view.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Application<Application>]
    [Scope<ViLa.WWW.Details.Index>]
    public sealed class FragmentContentTag : FragmentControlPanel
    {
        /// <summary>
        /// Das Labelformular
        /// </summary>
        private ControlFormTag Form { get; } = new ControlFormTag("form_tag");

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>  
        /// <param name="fragmentContext">
        /// The context for the fragment.
        /// </param>
        public FragmentContentTag(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);

            Add(Form);

            Form.InitializeForm += OnInitializeForm;
            Form.ProcessForm += OnProcessForm;
        }

        private void OnInitializeForm(ControlFormEventFormInitialize e)
        {
            var id = e.Context.Request.GetParameter("id");
            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id?.Value);

            if (measurementLog != null)
            {
                // Form.Tag.Value = measurementLog.Tag;
            }
        }

        private void OnProcessForm(ControlFormEventFormProcess e)
        {
            if (Form.Tag != null)
            {
                var id = e.Context.Request.GetParameter("id");
                var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id?.Value);

                if (measurementLog != null)
                {
                    // measurementLog.Tag = Form.Tag.Value;
                    ViewModel.Instance.UpdateMeasurementLog(measurementLog);
                }
            }
        }

        /// <summary>  
        /// Renders the control into an HTML node.  
        /// </summary>  
        /// <param name="renderContext">The render context.</param>  
        /// <param name="visualTree">The visual tree control.</param>  
        /// <returns>The rendered HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            Form.RedirectUri = ctx => ctx.Uri;

            return base.Render(renderContext, visualTree);
        }
    }
}
