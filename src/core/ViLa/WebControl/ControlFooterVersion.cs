using System;
using System.Reflection;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.UI.Attribute;
using WebExpress.UI.Component;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.Components;

namespace ViLa.WebControl
{
    [Section(Section.FooterPrimary)]
    [Application("ViLa")]
    public sealed class ControlFooterVersion : ControlText, IComponent
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public ControlFooterVersion()
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
            Text = string.Format("Version {0}", context.Page.Context.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);

            return base.Render(context);
        }
    }
}
