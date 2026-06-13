using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Represents a form control for tag input containing a single text input field.
    /// </summary>
    public class ControlFormTag : ControlForm
    {
        /// <summary>  
        /// Gets the input‑field control for tags.  
        /// </summary>
        public ControlFormItemInputText Tag { get; } = new ControlFormItemInputText("tag");

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary> 
        /// <param name="id">
        /// The identifier for the control.
        /// </param>
        public ControlFormTag(string id = null)
            : base(id)
        {
            Add(Tag);
        }
    }
}
