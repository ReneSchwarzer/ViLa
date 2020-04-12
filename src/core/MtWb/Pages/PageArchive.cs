using MtWb.Controls;
using MtWb.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebExpress.UI.Controls;

namespace MtWb.Pages
{
    public class PageArchive : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageArchive()
            : base("Archivieren")
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Init()
        {
            base.Init();
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();
            var id = GetParam("id");

            try
            {
                var archive = System.IO.Path.Combine(ViewModel.Instance.Context.AssetBaseFolder, "archive");

                if (!Directory.Exists(archive))
                {
                    Directory.CreateDirectory(archive);
                }

                var year = System.IO.Path.Combine(archive, DateTime.Now.Year.ToString());
                if (!Directory.Exists(year))
                {
                    Directory.CreateDirectory(year);
                }

                var month = System.IO.Path.Combine(year, DateTime.Now.ToString("MM"));
                if (!Directory.Exists(month))
                {
                    Directory.CreateDirectory(month);
                }

                var source = System.IO.Path.Combine(ViewModel.Instance.Context.AssetBaseFolder, "measurements", id + ".xml");
                var destination = System.IO.Path.Combine(month, id + ".xml");

                File.Move(source, destination);
                ViewModel.Instance.Logging.Add(new LogItem(LogItem.LogLevel.Info, string.Format("Datei {0}.xml wurde in das Achiv verschoben!", id)));
            }
            catch (Exception ex)
            {
                ViewModel.Instance.Logging.Add(new LogItem(LogItem.LogLevel.Exception, ex.ToString()));
            }

            Redirecting(GetPath(0));
        }

        /// <summary>
        /// In String konvertieren
        /// </summary>
        /// <returns>Das Objekt als String</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
