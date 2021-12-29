using WebExpress.Html;
using WebExpress.UI.WebControl;
using WebExpress.WebPage;

namespace ViLa.WebControl
{
    public class ControlFormularComment : ControlFormular
    {
        /// <summary>
        /// Liefert oder setzt den Kommentar
        /// </summary>
        public ControlFormularItemInputTextBox Comment { get; } = new ControlFormularItemInputTextBox("comment")
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
        public ControlFormularComment(string id = null)
            : base(id)
        {
            Add(Comment);

            Name = "form_comment";
            EnableCancelButton = true;
            Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.Three, PropertySpacing.Space.Five, PropertySpacing.Space.None);
            Layout = TypeLayoutFormular.Vertical;
            SubmitButton.Icon = new PropertyIcon(TypeIcon.PaperPlane);
            SubmitButton.Text = "vila:vila.comment.submit";
            EnableCancelButton = false;
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
