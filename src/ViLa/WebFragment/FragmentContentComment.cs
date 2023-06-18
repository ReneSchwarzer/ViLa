using System;
using System.Linq;
using System.Text;
using ViLa.Model;
using ViLa.WebControl;
using ViLa.WebPage;
using WebExpress.UI.WebAttribute;
using WebExpress.UI.WebControl;
using WebExpress.UI.WebFragment;
using WebExpress.WebApp.WebFragment;
using WebExpress.WebAttribute;
using WebExpress.WebHtml;
using WebExpress.WebPage;

namespace ViLa.WebFragment
{
    [Section(Section.ContentSecondary)]
    [Module<Module>]
    [Scope<PageDetails>]
    public sealed class FragmentContentComment : FragmentControlPanel
    {
        /// <summary>
        /// Das Kommentierungsformular
        /// </summary>
        private ControlFormComment Form { get; } = new ControlFormComment("form_comment");

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
        public FragmentContentComment()
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
        /// <param name="page">Die Seite, indem die Komonente aktiv ist</param>
        public override void Initialization(IFragmentContext context, IPage page)
        {
            base.Initialization(context, page);
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
