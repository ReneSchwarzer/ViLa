using WebExpress.WebCore.WebApplication;
using WebExpress.WebCore.WebAttribute;

namespace ViLa
{
    /// <summary>  
    /// Represents the Vila application.  
    /// </summary>
    [Name("vila:app.name")]
    [Description("vila:app.description")]
    [Icon("assets/img/vila.svg")]
    [AssetPath("/")]
    [ContextPath("/vila")]
    public sealed class Application : IApplication
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Application()
        {
        }

        /// <summary>  
        /// Is invoked when the application begins its work. The call is made concurrently.  
        /// </summary>
        public void Run()
        {

        }

        /// <summary>  
        /// Releases unmanaged resources that were reserved during use.  
        /// </summary>
        public void Dispose()
        {

        }
    }
}
