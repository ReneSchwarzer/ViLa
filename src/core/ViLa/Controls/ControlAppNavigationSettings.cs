using System;
using System.Collections.Generic;
using System.Text;
using ViLa.Pages;
using WebExpress.Html;
using WebExpress.UI.Controls;
using WebExpress.UI.Plugin;

namespace ViLa.Controls
{
    public class ControlAppNavigationSettings : ControlLink, IPluginComponentSettingsPrimary
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlAppNavigationSettings()
            : base()
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            TextColor = new PropertyColorText(TypeColorText.Dark);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = "Einstellungen";
            Uri = context.Page.Uri.Root.Append("settings");
            Active = context.Page is IPageSettings ? TypeActive.Active : TypeActive.None;
            Icon = new PropertyIcon(TypeIcon.Cog);

            return base.Render(context);
        }

    }
}
