using ViLa.Model;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    /// <summary>
    /// Represents a settings control form for mode selection.
    /// </summary>
    public class ControlFormMode : ControlForm
    {
        /// <summary>
        /// Gets the mode input combo control.
        /// </summary>
        public ControlFormItemInputCombo Mode { get; } = new ControlFormItemInputCombo("mode");

        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary> 
        public ControlFormMode()
            : base("settings")
        {
            foreach (var mode in System.Enum.GetValues<Model.Mode>())
            {
                Mode.Add(new ControlFormItemInputComboItem
                {
                    Value = _ => mode.ToString(),
                    Text = _ => mode.ToText()
                });
            }
            Mode.Initialize(args => args.Value.Text = ViewModel.Instance.Settings.Mode.ToString());

            Add(Mode);
            AddPrimaryButton
            (
                new ControlFormItemButtonSubmit()
            );
        }
    }
}
