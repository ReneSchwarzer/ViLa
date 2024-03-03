using System.Collections.Generic;
using ViLa.Model;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebCore.WebPage;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    public class ControlFormTimeControl : ControlForm
    {
        /// <summary>
        /// Bestimmt, wie die Messungen erfolgen sollen
        /// </summary>
        public ControlFormItemInputCheckbox Monday { get; } = new ControlFormItemInputCheckbox("monday")
        {
            Name = "auto",
            Label = "vila:vila.setting.form.mode.label",
            Help = "vila:vila.setting.form.mode.description",
            Description = ""
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlFormTimeControl()
            : base("settings")
        {
            Name = "settings";
            Classes = new List<string>(new[] { "m-3" });

            Add(Monday);

            FillFormular += OnFillFormular;
            ProcessFormular += OnProcessFormular;
        }

        /// <summary>
        /// Initialisiert das Formular
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        public override void Initialize(RenderContextFormular context)
        {
            base.Initialize(context);
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Formular befüllt werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnFillFormular(object sender, FormularEventArgs e)
        {
            //Monday.Value = ViewModel.Instance.Settings.Mode.ToString();
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Formular bearbeitet werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnProcessFormular(object sender, FormularEventArgs e)
        {
            //ViewModel.Instance.Settings.Mode = (Mode)Enum.Parse(typeof(Mode), Mode.Value);
            ViewModel.Instance.SaveSettings();

            e.Context.Page.Redirecting(e.Context.Uri);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            return base.Render(context);
        }
    }
}
