using System;
using System.Collections.Generic;
using System.Linq;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.Attribute;
using WebExpress.Html;
using WebExpress.Internationalization;
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

            void item(IEnumerable<MeasurementLog> value, string yaer)
            {
                if (!value.Any()) return;

                Content.Secondary.Add(new ControlText()
                {
                    Text = $"{ yaer }",
                    Format = TypeFormatText.H3,
                    Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
                });

                var grid = new ControlPanelGrid() { Fluid = TypePanelContainer.Fluid };

                foreach (var measurementLog in value)
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

            Content.Primary.Add(new ControlCardCharging()
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.Four, PropertySpacing.Space.None)
            });

            item(ViewModel.Instance.GetHistoryMeasurementLogs(new DateTime(DateTime.Now.Year, 1, 1), DateTime.Now), DateTime.Now.Year.ToString());
            item(ViewModel.Instance.GetHistoryMeasurementLogs(new DateTime(DateTime.Now.Year - 1, 1, 1), new DateTime(DateTime.Now.Year - 1, 12, 31)), (DateTime.Now.Year - 1).ToString());
            item(ViewModel.Instance.GetHistoryMeasurementLogs(new DateTime(DateTime.Now.Year - 2, 1, 1), new DateTime(DateTime.Now.Year - 2, 12, 31)), (DateTime.Now.Year - 2).ToString());
        }
    }
}
