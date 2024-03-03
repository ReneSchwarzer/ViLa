using ViLa.Model;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebResource;

namespace ViLa.WebResource
{
    [Segment("tags")]
    [ContextPath("/api")]
    [Module<Module>]
    public sealed class ResourceApiTags : ResourceRest
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ResourceApiTags()
            : base()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext.</param>
        public override void Initialization(IResourceContext context)
        {
            base.Initialization(context);
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        /// <param name="request">Die Anfrage.</param>
        /// <returns>Ein Objekt welches mittels JsonSerializer serialisiert werden kann.</returns>
        public override object GetData(Request request)
        {
            return ViewModel.Instance.Tags;
        }
    }
}
