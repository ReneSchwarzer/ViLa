using System.IO;
using ViLa.Model;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebResource;

namespace ViLa.WebResource
{
    /// <summary>
    /// Serves XML measurements files.
    /// </summary>
    [Title("Measurements")]
    [Segment("measurements")]
    [ContextPath("/")]
    [IncludeSubPaths(true)]
    [Application<Application>]
    [Cache]
    public sealed class ResourceMeasurements : WebExpress.WebCore.WebResource.ResourceFile
    {
        public ResourceMeasurements(IResourceContext resourceContext)
            : base(resourceContext)
        {
            RootDirectory = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "measurements");
        }
    }
}
