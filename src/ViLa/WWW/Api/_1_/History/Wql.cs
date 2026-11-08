using System.Collections.Generic;
using System.Linq;
using ViLa.Model;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;

namespace ViLa.WWW.Api._1_.History
{
    /// <summary>
    /// REST endpoint backing the WQL autocomplete for the history search control.
    /// Inherits the GET-based routing from RestApiWqlPrompt, dispatching on the
    /// last URL segment (history returns past WQL strings, analyze returns lookahead).
    /// </summary>
    [Application<Application>]
    public sealed class Wql : RestApiWqlPrompt<IndexMeasurementLog>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Wql()
        {
        }

        /// <summary>
        /// Returns the recent WQL strings the user has executed.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>Recent WQL strings.</returns>
        protected override IEnumerable<string> GetHistory(IRequest request)
        {
            return Enumerable.Empty<string>();
        }
    }
}
