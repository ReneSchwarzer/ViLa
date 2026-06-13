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
    /// Represents the content fragment for operating mode settings.
    /// </summary>
    [Section<SectionContentPrimary>]
    [Application<Application>]
    [Scope<ViLa.WWW.Settings.Mode>]
    public sealed class FragmentContentSettingsMode : FragmentControlPanel
    {
        private ControlFormMode Form { get; } = new ControlFormMode();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context for the fragment.</param>
        public FragmentContentSettingsMode(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);

            Form.Mode.Label = _ => "vila:vila.setting.mode.label";
            Form.Mode.Help = _ => "vila:vila.setting.mode.description";

            Add(Form);

            Form.InitializeForm += OnInitializeForm;
            Form.ProcessForm += OnProcessForm;
        }

        /// <summary>
        /// Initializes the form state before rendering.
        /// </summary>
        /// <param name="e">Contains the form initialization context.</param>
        private static void OnInitializeForm(ControlFormEventFormInitialize e)
        {
        }

        /// <summary>
        /// Processes the submitted operating mode and persists settings.
        /// </summary>
        /// <param name="e">Contains the form processing context.</param>
        private static void OnProcessForm(ControlFormEventFormProcess e)
        {
            var modeValue = e.Context.Request.GetParameter("mode")?.Value;

            if (int.TryParse(modeValue, out var modeNumber) && System.Enum.IsDefined(typeof(Mode), modeNumber))
            {
                ViewModel.Instance.Settings.Mode = (Mode)modeNumber;
            }
            else if (System.Enum.TryParse<Mode>(modeValue, true, out var mode))
            {
                ViewModel.Instance.Settings.Mode = mode;
            }

            ViewModel.Instance.SaveSettings();
        }

        /// <summary>
        /// Renders the control into an HTML node.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="visualTree">The visual tree control.</param>
        /// <returns>Returns the rendered HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            Form.RedirectUri = ctx => ctx.Uri;

            return base.Render(renderContext, visualTree);
        }
    }
}
