using System;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebControl
{
    /// <summary>
    /// Supplies the vehicle accent to the WebExpress Kanban column header.
    /// </summary>
    internal sealed class VehicleBoardColumn : IControlKanbanColumn
    {
        private readonly ControlKanbanColumn _column;
        private readonly string _color;

        /// <summary>
        /// Gets the assignment column identifier.
        /// </summary>
        public string Id => _column.Id;

        /// <summary>
        /// Gets the column label.
        /// </summary>
        public Func<IRenderControlContext, string> Title => _column.Title;

        /// <summary>
        /// Gets the column width.
        /// </summary>
        public Func<IRenderControlContext, string> Size => _column.Size;

        /// <summary>
        /// Initializes a column for a vehicle or the unassigned group.
        /// </summary>
        /// <param name="id">The assignment column identifier.</param>
        /// <param name="title">The display label.</param>
        /// <param name="color">The hexadecimal header color.</param>
        public VehicleBoardColumn(string id, string title, string color)
        {
            _column = new ControlKanbanColumn(id, title, "1fr");
            _color = color;
        }

        /// <summary>
        /// Renders the column with its configured header accent.
        /// </summary>
        /// <param name="renderContext">The current rendering context.</param>
        /// <param name="visualTree">The page tree containing the board.</param>
        /// <returns>The colored column declaration.</returns>
        public IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            var element = (HtmlElementTextContentDiv)_column.Render(renderContext, visualTree);
            return element.AddUserAttribute("data-color", _color);
        }
    }
}
