using System;
using System.Linq;
using ViLa.Model;
using WebExpress.UI.Controls;

namespace ViLa.Pages
{
    public sealed class PageLog : PageBase
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageLog()
            : base("Logging")
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

            var table = new ControlTable(this);
            table.AddColumn("Level", new PropertyIcon(TypeIcon.Hashtag), TypesLayoutTableRow.Info);
            table.AddColumn("Instanz", new PropertyIcon(TypeIcon.Code), TypesLayoutTableRow.Warning);
            table.AddColumn("Nachricht", new PropertyIcon(TypeIcon.CommentAlt), TypesLayoutTableRow.Danger);
            table.AddColumn("Zeit", new PropertyIcon(TypeIcon.Clock), TypesLayoutTableRow.Warning);

            Func<LogItem.LogLevel, PropertyIcon> func = (level) =>
            {
                switch (level)
                {
                    case LogItem.LogLevel.Info:
                        return new PropertyIcon(TypeIcon.Info);
                    case LogItem.LogLevel.Debug:
                        return new PropertyIcon(TypeIcon.Bug);
                    case LogItem.LogLevel.Warning:
                        return new PropertyIcon(TypeIcon.ExclamationTriangle);
                    case LogItem.LogLevel.Error:
                        return new PropertyIcon(TypeIcon.Times);
                    case LogItem.LogLevel.Exception:
                        return new PropertyIcon(TypeIcon.Bomb);
                }

                return new PropertyIcon(TypeIcon.None);
            };

            var log = ViewModel.Instance.Logging;

            if (!ViewModel.Instance.Settings.DebugMode)
            {
                log = log.Where(x => !(x.Level == LogItem.LogLevel.Debug || x.Level == LogItem.LogLevel.Exception)).ToList();
            }

            foreach (var v in log.OrderByDescending(x => x.Time))
            {
                var row = new ControlTableRow(this) { };
                row.Cells.Add(new ControlIcon(this) { Icon = func(v.Level) });
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0}", v.Instance) });
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0}", v.Massage) });
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0}", v.Time.ToString("dd.MM.yyyy HH.mm.ss.f")) });

                table.Rows.Add(row);
            }

            Main.Content.Add(table);
            Main.Content.Add(new ControlPanelCenter(this, new ControlButtonLink(this)
            {
                Text = ViewModel.Instance.Settings.DebugMode ? "Debug-Ausgaben ausblenden" : "Debug-Ausgaben einblenden",
                Icon = new PropertyIcon(TypeIcon.Bug),
                TextColor = new PropertyColorText(TypeColorText.Warning),
                Uri = Uri.Root.Append("/debug"),
                Margin = new PropertySpacingMargin(PropertySpacing.Space.Three)
            }));
        }

        /// <summary>
        /// In String konvertieren
        /// </summary>
        /// <returns>Das Objekt als String</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
