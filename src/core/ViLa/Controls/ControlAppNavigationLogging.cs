using System;
using System.Collections.Generic;
using System.Text;
using ViLa.Pages;
using WebExpress.Html;
using WebExpress.UI.Controls;
using WebExpress.UI.Plugin;

namespace ViLa.Controls
{
    public class ControlAppNavigationLogging : ControlLink, IPluginComponentAppNavigationPrimary
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlAppNavigationLogging()
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
            Text = "Logging";
            Uri = context.Page.Uri.Root.Append("log");
            Active = context.Page is IPageLogging ? TypeActive.Active : TypeActive.None;
            Icon = new PropertyIcon(TypeIcon.Book);

            return base.Render(context);
        }

    }
}
