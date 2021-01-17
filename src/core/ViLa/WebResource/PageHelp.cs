
using System.Reflection;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
using WebExpress.UI.WebControl;
using WebExpress.Uri;
using WebExpress.WebApp.WebResource;

namespace ViLa.WebResource
{
    [ID("Help")]
    [Title("vila.help.label")]
    [Segment("help", "vila.help.label")]
    [Path("/")]
    [Module("ViLa")]
    [Context("general")]
    [Context("help")]
    public sealed class PageHelp : PageTemplateWebApp
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageHelp()
        {
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();

            Favicons.Add(new Favicon(Uri.Root.Append("/assets/img/Favicon.png").ToString(), TypeFavicon.PNG));
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();

            Content.Primary.Add(new ControlImage()
            {
                Uri = Uri.Root.Append("assets/img/vila.svg"),
                Width = 200,
                Height = 200,
                HorizontalAlignment = TypeHorizontalAlignment.Right
            });

            var card = new ControlPanelCard();

            card.Add(new ControlText()
            {
                Text = this.I18N("app.label"),
                Format = TypeFormatText.H3
            });

            card.Add(new ControlText()
            {
                Text = this.I18N("app.description"),
                Format = TypeFormatText.Paragraph
            });

            card.Add(new ControlText()
            {
                Text = this.I18N("vila.help.privacypolicy.label"),
                Format = TypeFormatText.H3
            });

            card.Add(new ControlText()
            {
                Text = this.I18N("vila.help.privacypolicy.description"),
                Format = TypeFormatText.Paragraph
            });

            card.Add(new ControlText()
            {
                Text = this.I18N("vila.help.disclaimer.label"),
                Format = TypeFormatText.H3
            });

            card.Add(new ControlText()
            {
                Text = this.I18N("vila.help.disclaimer.description"),
                Format = TypeFormatText.Paragraph
            });

            card.Add(new ControlText()
            {
                Text = "Informationen über ViLa",
                Format = TypeFormatText.H3
            });

            card.Add(new ControlText()
            {
                Text = this.I18N("vila.help.version.label"),
                TextColor = new PropertyColorText(TypeColorText.Primary)
            });

            card.Add(new ControlText()
            {
                Text = string.Format("{0}", Context.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion),
                TextColor = new PropertyColorText(TypeColorText.Dark)
            });

            card.Add(new ControlText()
            {
                Text = this.I18N("vila.help.contact.label"),
                TextColor = new PropertyColorText(TypeColorText.Primary)
            });

            card.Add(new ControlLink()
            {
                Text = string.Format("rene_schwarzer@hotmail.de"),
                Uri = new UriAbsolute()
                {
                    Scheme = UriScheme.Mailto,
                    Authority = new UriAuthority("rene_schwarzer@hotmail.de")
                },
                TextColor = new PropertyColorText(TypeColorText.Dark)
            });

            Content.Primary.Add(card);
        }
    }
}
