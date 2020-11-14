using System;
using System.Collections.Generic;
using System.Text;
using ViLa.Pages;
using WebExpress.Html;
using WebExpress.UI.Controls;
using WebExpress.UI.Plugin;

namespace ViLa.Controls
{
    public class ControlAppNavigationHelp : ControlLink, IPluginComponentAppNavigationSecondary
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlAppNavigationHelp()
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
            Text = "Hilfe";
            Uri = context.Page.Uri.Root.Append("help");
            Active = context.Page is IPageHelp ? TypeActive.Active : TypeActive.None;
            Icon = new PropertyIcon(TypeIcon.InfoCircle);

            return base.Render(context);
        }

    }
}
