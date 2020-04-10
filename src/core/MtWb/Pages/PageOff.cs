using MtWb.Controls;
using MtWb.Model;
using System;
using System.Collections.Generic;
using System.Text;
using WebExpress.UI.Controls;

namespace MtWb.Pages
{
    public class PageOff : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageOff()
            : base("Off")
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

            ViewModel.Instance.StopsCharging();

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
