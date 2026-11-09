using WebExpress.WebApp.WebControl;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebControl
{
    /// <summary>
    /// Renders a data board that includes the expected source assignment in card moves.
    /// </summary>
    internal sealed class ControlVehicleAssignmentBoard : ControlDataKanban
    {
        /// <summary>Initializes the vehicle assignment board.</summary>
        /// <param name="id">The board identifier.</param>
        public ControlVehicleAssignmentBoard(string id) : base(id)
        {
        }

        /// <summary>Renders the board with its assignment persistence controller.</summary>
        /// <param name="renderContext">The current rendering context.</param>
        /// <param name="visualTree">The containing visual tree.</param>
        /// <returns>The data board element.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var element = (HtmlElementTextContentDiv)base.Render(renderContext, visualTree);
            element.Class = element.Class.Replace("wx-webapp-kanban", "wx-vila-data-kanban");
            return element;
        }
    }
}
