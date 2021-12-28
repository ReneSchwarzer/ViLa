using WebExpress.Attribute;
using WebExpress.WebResource;

namespace ViLa.WebResource
{
    /// <summary>
    /// Lieferung einer im Assamby eingebetteten Ressource
    /// </summary>
    [ID("Asset")]
    [Title("Assets")]
    [Segment("assets", "")]
    [Path("/")]
    [IncludeSubPaths(true)]
    [Module("ViLa")]
    public sealed class ResourceAsset : WebExpress.WebResource.ResourceAsset
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ResourceAsset()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="context">Der Kontext</param>
        public override void Initialization(IResourceContext context)
        {
            base.Initialization(context);
        }
    }
}
