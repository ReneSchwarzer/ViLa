using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ViLa.Model;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPlugin;

namespace ViLa
{
    /// <summary>  
    /// Represents a plugin that initializes a ViewModel and updates it in a 
    /// continuous loop.  
    /// </summary>
    [Name("vila:plugin.name")]
    [Description("vila:plugin.description")]
    [Icon("/assets/img/vila.svg")]
    [Dependency("webexpress.webui")]
    [Dependency("webexpress.webapp")]
    [Application<Application>()]
    public sealed class Plugin : IPlugin
    {
        /// <summary>  
        /// Initializes a new instance of the class.  
        /// </summary>
        /// <param name="context">
        /// The context that applies to the execution of the plugin.
        /// </param>
        public Plugin(IPluginContext context)
        {
            ViewModel.Instance.Context = context;
            ViewModel.Instance.Init();

            // increase priority
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
        }

        /// <summary>  
        /// Is invoked when the plugin begins its work. The call to Run is made concurrently.  
        /// </summary>
        public void Run()
        {
            Task.Run(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;

                // Loop
                while (true)
                {
                    try
                    {
                        ViewModel.Instance.Update();
                    }
                    finally
                    {
                        Thread.Sleep(10);
                    }
                }
            });
        }

        /// <summary>  
        /// Releases unmanaged resources that were reserved during use.  
        /// </summary>
        public void Dispose()
        {

        }
    }
}
