using WebExpress.Pages;
using WebExpress.UI.Controls;

namespace MtWb.Pages
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

            Main.Content.Add(new ControlImage(this)
            {
                Source = GetPath(0, "Assets/StoreLogo.png"),
                Width = 200,
                Height = 200,
                HorizontalAlignment = TypesHorizontalAlignment.Right
            });

            Main.Content.Add(new ControlText(this)
            {
                Text = "Makes the Wallbox better",
                Format = TypesTextFormat.H1
            });

            Main.Content.Add(new ControlText(this)
            {
                Text = "Mit MtWb verwalten Sie ihre Wallbox und behalten so den Überblick über die entstehenden Kosten.",
                Format = TypesTextFormat.Paragraph
            });

            Main.Content.Add(new ControlText(this)
            {
                Text = "Datenschutzrichtlinie: Die während der Nutzung eingegebenen Daten werden lokal auf Ihrem Gerät als Dateien gespeichert und über die Cloud gesichert.Sie behalten jederzeit die Datenhoheit.Die Daten werden zu keiner Zeit an Dritte übermittelt.Persönliche Informationen und Standortinformationen werden nicht erhoben.",
                Format = TypesTextFormat.Paragraph
            });

            Main.Content.Add(new ControlText(this)
            {
                Text = "Haftungsausschluss: Die Haftung für Schäden durch Sachmängel wird ausgeschlossen.Die Haftung auf Schadensersatz wegen Körperverletzung sowie bei grober Fahrlässigkeit oder Vorsatz bleibt unberührt.",
                Format = TypesTextFormat.Paragraph
            });

            Main.Content.Add(new ControlText(this)
            {
                Text = "Informationen über Makes the Wallbox better",
                Format = TypesTextFormat.H1
            });

            Main.Content.Add
            (
                new ControlPanelCenter
                (
                    this,

                    new ControlText(this)
                    {
                        Text = string.Format("Version"),
                        Color = TypesTextColor.Primary
                    },
                    new ControlText(this)
                    {
                        Text = string.Format("{0}", Context.Version),
                        Color = TypesTextColor.Dark
                    },
                    new ControlText(this)
                    {
                        Text = string.Format("Kontakt"),
                        Color = TypesTextColor.Primary
                    },
                    new ControlLink(this)
                    {
                        Text = string.Format("rene_schwarzer@hotmail.de"),
                        Url = new PathExtern("mailto:rene_schwarzer@hotmail.de"),
                        Color = TypesTextColor.Dark
                    }
                )
            );
        }
    }
}
