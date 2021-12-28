using System.IO;
using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.WebResource;

namespace ViLa.WebResource
{
    /// <summary>
    /// Lieferung einer im Assamby eingebetteten Ressource
    /// </summary>
    [ID("Measurements")]
    [Title("Measurements")]
    [Segment("measurements", "")]
    [Path("/")]
    [IncludeSubPaths(true)]
    [Module("ViLa")]
    public sealed class ResourceMeasurements : WebExpress.WebResource.ResourceFile
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ResourceMeasurements()
        {
            RootDirectory = ViewModel.Instance.Context.Host.AssetPath;
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
