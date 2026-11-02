using ViLa.Model;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;

namespace ViLa.WWW.Api._1_.Vehicles
{
    /// <summary>
    /// Provides WQL analysis and field suggestions for vehicle profiles.
    /// </summary>
    [Application<Application>]
    public sealed class Wql : RestApiWqlPrompt<Vehicle>
    {
    }
}
