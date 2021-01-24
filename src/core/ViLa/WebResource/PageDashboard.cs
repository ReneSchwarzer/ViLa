using System;
using System.Linq;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.UI.WebControl;
using WebExpress.WebApp.WebResource;

namespace ViLa.WebResource
{
    [ID("Home")]
    [Title("vila.dashboard.label")]
    [Segment("", "vila.dashboard.label")]
    [Path("")]
    [Module("ViLa")]
    [Context("general")]
    [Context("dashboard")]
    public sealed class PageDashboard : PageTemplateWebApp, IPageDashbord
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        public PageDashboard()
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

            void item(int year)
            {
                if (ViewModel.Instance.GetHistoryMeasurementLogs(new DateTime(year, 1, 1), new DateTime(year, 12, 31)).Any())
                {
                    Content.Secondary.Add(new ControlText()
                    {
                        Text = $"{ year }",
                        Format = TypeFormatText.H3,
                        Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
                    });

                    for (var i = 12; i > 0; i--)
                    {
                        var m = ViewModel.Instance.GetHistoryMeasurementLogs(new DateTime(year, i, 1), new DateTime(year, i, DateTime.DaysInMonth(year, i)));
                        if (m.Any())
                        {
                            Content.Secondary.Add(new ControlText()
                            {
                                Text = $"{ Culture.DateTimeFormat.GetMonthName(i) }",
                                Format = TypeFormatText.H4,
                                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
                            });

                            var grid = new ControlPanelGrid() { Fluid = TypePanelContainer.Fluid };

                            foreach (var measurementLog in m)
                            {
                                var card = new ControlCardMeasurementLog()
                                {
                                    MeasurementLog = measurementLog,
                                    GridColumn = new PropertyGrid(TypeDevice.Medium, 3)
                                };

                                grid.Content.Add(card);
                            }

                            Content.Secondary.Add(grid);
                        }
                    }
                }
            }

            Content.Primary.Add(new ControlCardCharging()
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.Four, PropertySpacing.Space.None)
            });

            item(DateTime.Now.Year);
            item(DateTime.Now.Year - 1);
            item(DateTime.Now.Year - 2);
        }
    }
}
