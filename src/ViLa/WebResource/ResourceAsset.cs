using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebResource;

namespace ViLa.WebResource
{
    // TODO MIGRATION: New ResourceAsset base requires IResourceContext via ctor; Initialization override is gone.
    [Title("Assets")]
    [Segment("assets")]
    [ContextPath("/")]
    [IncludeSubPaths(true)]
    [Application<Application>]
    [Cache]
    public sealed class ResourceAsset : WebExpress.WebCore.WebResource.ResourceAsset
    {
        public ResourceAsset(IResourceContext resourceContext)
            : base(resourceContext)
        {
        }
    }
}
