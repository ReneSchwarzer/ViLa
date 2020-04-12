using MtWb.Controls;
using MtWb.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebExpress.UI.Controls;

namespace MtWb.Pages
{
    public class PageDel : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageDel()
            : base("Löschen")
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
                File.Delete(System.IO.Path.Combine(ViewModel.Instance.Context.AssetBaseFolder, "measurements", id + ".xml"));
                ViewModel.Instance.Logging.Add(new LogItem(LogItem.LogLevel.Info, string.Format("Datei {0}.xml wurde gelöscht!", id)));
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
