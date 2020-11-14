using System;
using System.Collections.Generic;
using System.Text;
using ViLa.Pages;
using WebExpress.Html;
using WebExpress.UI.Controls;
using WebExpress.UI.Plugin;

namespace ViLa.Controls
{
    public class ControlAppNavigationHistory : ControlLink, IPluginComponentAppNavigationPrimary
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlAppNavigationHistory()
            : base()
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            TextColor = new PropertyColorText(TypeColorText.Light);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = "Verlauf";
            Uri = context.Page.Uri.Root.Append("history");
            Active = context.Page is IPageHistory ? TypeActive.Active : TypeActive.None;
            Icon = new PropertyIcon(TypeIcon.ChartBar);

            return base.Render(context);
        }

    }
}
