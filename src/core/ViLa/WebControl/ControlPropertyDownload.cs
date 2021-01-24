using System;
using System.Collections.Generic;
using System.Reflection;
using ViLa.Model;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.Attribute;
using WebExpress.UI.Component;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Components;

namespace ViLa.WebControl
{
    [Section(Section.PropertyPreferences)]
    [Application("ViLa")]
    [Context("details")]
    public sealed class ControlPropertyDownload : ControlButtonLink, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlPropertyDownload()
            : base("download_btn")
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            Margin = new PropertySpacingMargin(PropertySpacing.Space.Two);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            var id = context.Page.GetParamValue("id");

            Text = context.I18N("vila.download.label");
            BackgroundColor = new PropertyColorButton(TypeColorButton.Primary);
            Icon = new PropertyIcon(TypeIcon.Download);
            TextColor = new PropertyColorText(TypeColorText.Light);
            Uri = context.Uri.Root.Append("measurements/" + id + ".xml");

            return base.Render(context);
        }
    }
}
