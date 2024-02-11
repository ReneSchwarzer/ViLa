using WebExpress.WebHtml;
using WebExpress.WebPage;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebControl
{
    public class ControlFormTag : ControlForm
    {
        /// <summary>
        /// Liefert oder setzt das Label
        /// </summary>
        public ControlFormItemInputTextBox Tag { get; } = new ControlFormItemInputTextBox("tag")
        {
            Name = "tag",
            Label = "vila:vila.tag.label",
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="id">Die ID</param>
        public ControlFormTag(string id = null)
            : base(id)
        {
            Add(Tag);

            Name = "form_tag";
            Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Three, PropertySpacing.Space.Five, PropertySpacing.Space.None);
            Layout = TypeLayoutForm.Vertical;
            SubmitButton.Icon = new PropertyIcon(TypeIcon.Save);
            SubmitButton.Text = "vila:vila.tag.submit";
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
