using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Represents a form control for time settings with weekday selection.
    /// </summary>
    public class ControlFormTimeControl : ControlForm
    {
        /// <summary>  
        /// Gets the form‑input checkbox control for Monday.  
        /// </summary>
        public ControlFormItemInputCheck Monday { get; } = new ControlFormItemInputCheck("monday");

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary> 
        public ControlFormTimeControl()
            : base("settings")
        {
            Add(Monday);
            AddPrimaryButton
            (
                new ControlFormItemButtonSubmit()
            );
        }
    }
}
