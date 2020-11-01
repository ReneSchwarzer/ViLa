using WebExpress.Html;
using WebExpress.UI.Controls;

namespace ViLa.Pages
{
    public class PageHelp : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageHelp()
            : base("Hilfe")
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

            Main.Content.Add(new ControlImage()
            {
                Source = Uri.Root.Append("Assets/StoreLogo.png"),
                Width = 200,
                Height = 200,
                HorizontalAlignment = TypeHorizontalAlignment.Right
            });

            Main.Content.Add(new ControlText()
            {
                Text = "ViLa - Visuelles Laden",
                Format = TypeFormatText.H1
            });

            Main.Content.Add(new ControlText()
            {
                Text = "Mit ViLa verwalten Sie ihre Wallbox und behalten so den Überblick über die entstehenden Kosten.",
                Format = TypeFormatText.Paragraph
            });

            Main.Content.Add(new ControlText()
            {
                Text = "Datenschutzrichtlinie",
                Format = TypeFormatText.H4
            });

            Main.Content.Add(new ControlText()
            {
                Text = "Die während der Nutzung eingegebenen Daten werden lokal auf Ihrem Gerät gespeichert. Sie behalten jederzeit die Datenhoheit. Die Daten werden zu keiner Zeit an Dritte übermittelt. Persönliche Informationen und Standortinformationen werden nicht erhoben.",
                Format = TypeFormatText.Paragraph
            });

            Main.Content.Add(new ControlText()
            {
                Text = "Haftungsausschluss",
                Format = TypeFormatText.H4
            });

            Main.Content.Add(new ControlText()
            {
                Text = "Die Haftung für Schäden durch Sachmängel wird ausgeschlossen. Die Haftung auf Schadensersatz wegen Körperverletzung sowie bei grober Fahrlässigkeit oder Vorsatz bleibt unberührt.",
                Format = TypeFormatText.Paragraph
            });

            Main.Content.Add(new ControlText()
            {
                Text = "Informationen über ViLa",
                Format = TypeFormatText.H1
            });

            Main.Content.Add
            (
                new ControlPanelCenter
                (
                    new ControlText()
                    {
                        Text = string.Format("Version"),
                        TextColor = new PropertyColorText(TypeColorText.Primary)
                    },
                    new ControlText()
                    {
                        Text = string.Format("{0}", Context.Version),
                        TextColor = new PropertyColorText(TypeColorText.Dark)
                    },
                    new ControlText()
                    {
                        Text = string.Format("Kontakt"),
                        TextColor = new PropertyColorText(TypeColorText.Primary)
                    },
                    new ControlLink()
                    {
                        Text = string.Format("rene_schwarzer@hotmail.de"),
                        Uri = new UriAbsolute()
                        {
                            Scheme = UriScheme.Mailto,
                            Authority = new UriAuthority("rene_schwarzer@hotmail.de")
                        },
                        TextColor = new PropertyColorText(TypeColorText.Dark)
                    }
                )
            );
        }
    }
}
