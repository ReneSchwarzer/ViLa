using System;
using System.Linq;
using System.Net;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebCore;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebUI.WebControl;
using WebExpress.WebApp.WebScope;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebPage;

namespace ViLa.WWW.Log
{
    /// <summary>
    /// Represents a log index page that processes visual trees for the web application.
    /// </summary>
    [Title("vila:vila.log.label")]
    [Application<Application>]
    [Scope<IScopeGeneral>]
    public sealed class Index : IPage<VisualTreeWebApp>, IScopeGeneral
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Index()
        {
        }

        /// <summary>  
        /// Processes the visual tree using the specified render context.  
        /// </summary>  
        /// <param name="renderContext">The render context used for processing.</param>  
        /// <param name="visualTree">The visual tree of the web application to be processed.</param>
        public void Process(IRenderContext renderContext, VisualTreeWebApp visualTree)
        {
            var request = renderContext.Request;
            var debug = request.GetParameter("debug")?.Value == "1";
            // copy under the lock and filter outside it, so the GPIO loop's Log() calls are not blocked
            LogItem[] snapshot;
            var log = ViewModel.Instance.Logging;
            lock (log)
            {
                snapshot = log.ToArray();
            }
            var entries = snapshot.Where(item => debug || item.Level != LogItem.LogLevel.Debug).Reverse().ToArray();
            if (entries.Length == 0)
            {
                visualTree.Content.MainPanel.AddPrimary(new ControlEmptyState { Message = _ => "vila:vila.log.empty" });
                return;
            }

            const int pageSize = 50;
            var pages = (entries.Length + pageSize - 1) / pageSize;
            int.TryParse(request.GetParameter("page")?.Value, out var requested);
            var page = Math.Clamp(requested, 1, pages);
            var table = new ControlTable("vila-log");
            foreach (var column in new[] { "time", "level", "source", "message" })
            {
                table.AddColumn("vila:vila.log." + column);
            }
            foreach (var item in entries.Skip((page - 1) * pageSize).Take(pageSize))
            {
                var message = item.Massage ?? string.Empty;
                if (message.StartsWith("vila:", StringComparison.Ordinal))
                {
                    message = I18N.Translate(request, message, item.Argument ?? string.Empty);
                }
                table.AddRow(
                    new ControlTableCell { Text = _ => item.Time.ToString("G", request.Culture) },
                    new ControlTableCell { Text = _ => I18N.Translate(request, "vila:vila.log.level." + item.Level.ToString().ToLowerInvariant()) },
                    new ControlTableCell { Text = _ => WebUtility.HtmlEncode(item.Instance) },
                    new ControlTableCell { Text = _ => WebUtility.HtmlEncode(message) });
            }
            visualTree.Content.MainPanel.AddPrimary(table);
            var uri = WebEx.ComponentHub.SitemapManager.GetUri<Index>(request.ApplicationContext);
            if (ControlPager.Create(request, page, pages, target => uri + $"?debug={(debug ? 1 : 0)}&page={target}") is { } pager)
            {
                visualTree.Content.MainPanel.AddPrimary(pager);
            }
        }
    }
}
