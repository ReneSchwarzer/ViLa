using System;
using System.Linq;
using System.Text;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.UI.Attribute;
using WebExpress.UI.WebComponent;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebComponent;
using WebExpress.WebPage;

namespace ViLa.WebComponent
{
    [Section(Section.ContentSecondary)]
    [Module("vila")]
    [Context("details")]
    public sealed class ComponentContentComment : ControlPanel, IComponent
    {
        /// <summary>
        /// Das Kommentierungsformular
        /// </summary>
        private ControlFormularComment Form { get; } = new ControlFormularComment("form_comment");

        /// <summary>
        /// Die Liste
        /// </summary>
        private ControlList List { get; } = new ControlList()
        {
            Layout = TypeLayoutList.Flush,
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two, PropertySpacing.Space.None, PropertySpacing.Space.Five, PropertySpacing.Space.None)
        };

        /// <summary>
        /// Konstruktor
        /// </summary>
        public ComponentContentComment()
        {
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);

            Content.Add(List);

            Form.ProcessFormular += OnProcessFormular;
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein nuer Kommentar gepeichert werden soll
        /// </summary>
        /// <param name="sender">Der Auslöser des Events</param>
        /// <param name="e">Das Eventargument</param>
        private void OnProcessFormular(object sender, FormularEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Form.Comment.Value))
            {
                var id = e.Context.Request.GetParameter("id");
                var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id?.Value);
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(Form.Comment.Value));

                measurementLog.Comments.Add(new CommentItem() { Guid = Guid.NewGuid().ToString(), Comment = base64, Created = DateTime.Now, Updated = DateTime.Now });

                ViewModel.Instance.UpdateMeasurementLog(measurementLog);
            }
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public void Initialization(IComponentContext context)
        {
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            var id = context.Request.GetParameter("id");
            var measurementLog = ViewModel.Instance.GetHistoryMeasurementLog(id?.Value);

            Form.RedirectUri = context.Uri;
            List.Items.Clear();

            foreach (var comment in measurementLog.Comments.OrderBy(x => x.Created))
            {
                List.Add(new ControlListItem(new ControlTimelineComment()
                {
                    Post = Encoding.UTF8.GetString(Convert.FromBase64String(comment.Comment)),
                    Timestamp = comment.Created,
                    Likes = -1
                }));
            }

            List.Add(new ControlListItem(Form));

            return base.Render(context);
        }
    }
}
