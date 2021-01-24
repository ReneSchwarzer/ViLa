using System;
using System.Reflection;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.Attribute;
using WebExpress.UI.Component;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Components;

namespace ViLa.WebControl
{
    [Section(Section.FooterSecondary)]
    [Application("ViLa")]
    public sealed class ControlFooterLicence : ControlText, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlFooterLicence()
            : base()
        {
            Init();
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        private void Init()
        {
            TextColor = new PropertyColorText(TypeColorText.Muted);
            Format = TypeFormatText.Center;
            Size = new PropertySizeText(TypeSizeText.Small);
        }

        /// <summary>
        /// In HTML konvertieren
        /// </summary>
        /// <param name="context">Der Kontext, indem das Steuerelement dargestellt wird</param>
        /// <returns>Das Control als HTML</returns>
        public override IHtmlNode Render(RenderContext context)
        {
            Text = string.Format("{0}: {1}", context.I18N("app.license.label"), context.I18N("app.license.value"));
            
            return base.Render(context);
        }
    }
}
