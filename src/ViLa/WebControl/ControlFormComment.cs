using WebExpress.WebUI.WebControl;
using WebExpress.WebHtml;
using WebExpress.WebPage;

namespace ViLa.WebControl
{
    public class ControlFormComment : ControlForm
    {
        /// <summary>
        /// Liefert oder setzt den Kommentar
        /// </summary>
        public ControlFormItemInputTextBox Comment { get; } = new ControlFormItemInputTextBox("comment")
        {
            Name = "comment",
            Label = "vila:vila.comment.label",
            Help = "vila:vila.comment.description",
            Icon = new PropertyIcon(TypeIcon.Comment),
            Format = TypesEditTextFormat.Wysiwyg
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="id">Die ID</param>
        public ControlFormComment(string id = null)
            : base(id)
        {
            Add(Comment);

            Name = "form_comment";
            Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Three, PropertySpacing.Space.Five, PropertySpacing.Space.None);
            Layout = TypeLayoutForm.Vertical;
            SubmitButton.Icon = new PropertyIcon(TypeIcon.PaperPlane);
            SubmitButton.Text = "vila:vila.comment.submit";
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
