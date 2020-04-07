using MtWb.Controls;
using MtWb.Model;
using System;
using System.Collections.Generic;
using System.Text;
using WebExpress.UI.Controls;

namespace MtWb.Pages
{
    public class PageOn : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageOn()
            : base("On")
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

            ViewModel.Instance.StartsTheChargingProcess();

            Redirecting("/");
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
