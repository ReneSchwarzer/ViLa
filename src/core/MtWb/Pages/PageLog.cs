using MtWb.Controls;
using MtWb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebExpress.UI.Controls;

namespace MtWb.Pages
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

            var menu = new ControlMenu(this);
            ToolBar.Add(menu);
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        public override void Process()
        {
            base.Process();

            Main.Content.Add(new ControlText(this)
            {
                Text = "Ereignisse",
                Format = TypesTextFormat.Center,
                Color = TypesTextColor.Primary,
                Class = "m-3"
            });

            var table = new ControlTable(this);
            table.AddColumn("Level", Icon.Hashtag, TypesLayoutTableRow.Info);
            table.AddColumn("Instanz", Icon.Code, TypesLayoutTableRow.Warning);
            table.AddColumn("Nachricht", Icon.CommentAlt, TypesLayoutTableRow.Danger);
            table.AddColumn("Zeit", Icon.Clock, TypesLayoutTableRow.Warning);

            Func<LogItem.LogLevel, Icon> func = (level) =>
            {
                switch (level)
                {
                    case LogItem.LogLevel.Info:
                        return Icon.Info;
                    case LogItem.LogLevel.Debug:
                        return Icon.Bug;
                    case LogItem.LogLevel.Warning:
                        return Icon.ExclamationTriangle;
                    case LogItem.LogLevel.Error:
                        return Icon.Times;
                    case LogItem.LogLevel.Exception:
                        return Icon.Bomb;
                }

                return Icon.None;
            };

            var log = ViewModel.Instance.Logging;

            if (!ViewModel.Instance.Settings.DebugMode)
            {
                log = log.Where(x => !(x.Level == LogItem.LogLevel.Debug || x.Level == LogItem.LogLevel.Exception)).ToList();
            }

            foreach (var v in log.OrderByDescending(x => x.Time))
            {
                var row = new ControlTableRow(this) { };
                row.Cells.Add(new ControlText(this) { Class = func(v.Level).ToClass() });
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0}", v.Instance) });
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0}", v.Massage) });
                row.Cells.Add(new ControlText(this) { Text = string.Format("{0}", v.Time.ToString("dd.MM.yyyy HH.mm.ss.f")) });

                table.Rows.Add(row);
            }

            Main.Content.Add(table);
            Main.Content.Add(new ControlPanelCenter(this, new ControlButtonLink(this)
            {
                Text = ViewModel.Instance.Settings.DebugMode ? "Debug-Ausgaben ausblenden" : "Debug-Ausgaben einblenden",
                Icon = Icon.Bug,
                Color = TypesTextColor.Warning,
                Url = "/debug",
                Class = "m-3"
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
