using System.Collections.Generic;
using System.Linq;
using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebControl
{
    /// <summary>Edits the measurement description under a fixed localized title.</summary>
    internal sealed class ControlDescriptionEditor : ControlDataModalEditor
    {
        /// <summary>Initializes the description dialog.</summary>
        /// <param name="id">The dialog identifier.</param>
        public ControlDescriptionEditor(string id) : base(id)
        {
        }

        /// <summary>Marks the title as read only independently of the form loading state.</summary>
        /// <param name="renderContext">The current form context.</param>
        /// <param name="visualTree">The page containing the dialog.</param>
        /// <param name="items">The items to render.</param>
        /// <returns>The rendered description editor.</returns>
        public override IHtmlNode Render(IRenderControlFormContext renderContext, IVisualTreeControl visualTree, IEnumerable<IControlFormItem> items)
        {
            var dialog = base.Render(renderContext, visualTree, items);
            var title = dialog.Find(node => node is HtmlElementFieldInput input && input.Name == "Title")
                .OfType<HtmlElementFieldInput>().FirstOrDefault();
            if (title != null)
            {
                title.Readonly = "readonly";
            }
            return dialog;
        }
    }
}
