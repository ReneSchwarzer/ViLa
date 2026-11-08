using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ViLa.Model;
using WebExpress.WebCore;
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
    [Icon("assets/img/vila.svg")]
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
        /// <param name="serverContext">The host supplying the shared settings directory and configuration.</param>
        public Plugin(IPluginContext context, IHttpServerContext serverContext)
        {
            ViewModel.Instance.Context = context;
            ViewModel.Instance.SettingsStorage = new SettingsStore(System.AppDomain.CurrentDomain.BaseDirectory,
                serverContext.SettingsPath, serverContext.Configuration);
            ViewModel.Instance.Init();

            WebEx.Favicon = "assets/img/vila.svg";

            // increase priority
            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception || ex is PlatformNotSupportedException || ex is UnauthorizedAccessException)
            {
                // insufficient privileges (e.g. in a container); continue with the default priority
            }
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
