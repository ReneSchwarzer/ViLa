using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Represents a form control with a comment input text field.
    /// </summary>
    public class ControlFormComment : ControlForm
    {
        /// <summary>  
        /// Gets the input field for comments.  
        /// </summary>
        public ControlFormItemInputText Comment { get; } = new ControlFormItemInputText("comment");

        /// <summary>
        /// Initializes a new instance of the  class.
        /// </summary>
        /// <param name="id">
        /// The unique identifier for the control.
        /// </param>
        public ControlFormComment(string id = null)
            : base(id)
        {
            Add(Comment);
        }
    }
}
