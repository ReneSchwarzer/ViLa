using System.Reflection;

namespace ViLa.App
{
    /// <summary>
    /// The main entry point of the application. Initializes and runs the 
    /// web application using WebExpress.WebCore.WebEx.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main method that serves as the entry point of the application. It 
        /// creates an instance of the WebEx class, sets its name to the name of 
        /// the executing assembly, and then calls the Execution method with the 
        /// command-line arguments.
        /// </summary>
        /// <param name="args">
        /// The command-line arguments passed to the application.
        /// </param>
        private static void Main(string[] args)
        {
            var app = new WebExpress.WebCore.WebEx()
            {
                Name = Assembly.GetExecutingAssembly().GetName().Name
            };

            app.Execution(args);
        }
    }
}
