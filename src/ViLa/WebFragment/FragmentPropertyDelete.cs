using WebExpress.WebApp.WebSection;
using WebExpress.WebCore;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebUri;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Adds the DELETE action to the measurement detail menu.
    /// </summary>
    [Section<SectionHeadlineMoreSecondary>]
    [Application<Application>]
    [Scope<WWW.Details.Index>]
    [Order(3)]
    public sealed class FragmentPropertyDelete : FragmentControlDropdownItemLink
    {
        /// <summary>
        /// Initializes the menu item for the current measurement.
        /// </summary>
        /// <param name="fragmentContext">The fragment registration context.</param>
        public FragmentPropertyDelete(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Text = _ => "vila:vila.details.delete";
            Color = _ => TypeColorText.Danger;
            Icon = _ => new IconTrash();
            PrimaryAction = context => new ActionModal("delete-measurement-" + context.Request.GetParameter<ParameterId>()?.Value);
        }
    }
}
