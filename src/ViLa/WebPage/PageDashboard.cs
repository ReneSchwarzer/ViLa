using System;
using System.Linq;
using ViLa.Model;
using ViLa.WebControl;
using WebExpress.WebApp.WebPage;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebResource;
using WebExpress.WebCore.WebScope;
using WebExpress.WebUI.WebControl;

namespace ViLa.WebPage
{
    [Title("vila:vila.dashboard.label")]
    [Segment("", "vila:vila.dashboard.label")]
    [ContextPath("/")]
    [Module<Module>]
    public sealed class PageDashboard : PageWebApp, IPageDashbord, IScope
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
        /// <param name="context">Der Kontext</param>
        public override void Initialization(IResourceContext context)
        {
            base.Initialization(context);
        }

        /// <summary>
        /// Verarbeitung
        /// </summary>
        /// <param name="context">Der Kontext zum Rendern der Seite</param>
        public override void Process(RenderContextWebApp context)
        {
            base.Process(context);

            void item(int year)
            {
                if (ViewModel.Instance.GetHistoryMeasurementLogs(new DateTime(year, 1, 1), new DateTime(year, 12, 31)).Any())
                {
                    context.VisualTree.Content.Secondary.Add(new ControlText()
                    {
                        Text = $"{year}",
                        Format = TypeFormatText.H3,
                        Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
                    });

                    for (var i = 12; i > 0; i--)
                    {
                        var offset = ViewModel.Instance.Settings.BillingDayOffset == 0 ? 0 : ViewModel.Instance.Settings.BillingDayOffset - 1;
                        var m = ViewModel.Instance.GetHistoryMeasurementLogs
                        (
                            new DateTime(year, i, 1).AddDays(offset),
                            new DateTime(year, i, DateTime.DaysInMonth(year, i), 23, 59, 59).AddDays(offset)
                        );

                        if (m.Any())
                        {
                            context.VisualTree.Content.Secondary.Add(new ControlText()
                            {
                                Text = $"{Culture.DateTimeFormat.GetMonthName(i)} - {string.Format(context.Culture, "{0:F2}", m.Sum(x => x.FinalCost))} {ViewModel.Instance.Settings.Currency} / {string.Format(context.Culture, "{0:F2}", m.Sum(x => x.FinalPower))} kWh",
                                Format = TypeFormatText.H4,
                                Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None),
                                TextColor = new PropertyColorText(TypeColorText.Primary)
                            });

                            foreach (var tag in m
                                .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
                                .SelectMany(x => x.Tag.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                                .Distinct())
                            {
                                var mt = m.Where(x => !string.IsNullOrWhiteSpace(x.Tag))
                                          .Where(x => x.Tag.Contains(tag));

                                context.VisualTree.Content.Secondary.Add(new ControlPanel
                                (
                                    new ControlTag()
                                    {
                                        BackgroundColor = new PropertyColorBackground(ViewModel.Instance.GetColor(tag)),
                                        Text = tag
                                    },
                                    new ControlText()
                                    {
                                        Text = $"{string.Format(context.Culture, "{0:F2}", mt.Sum(x => x.FinalCost))} {ViewModel.Instance.Settings.Currency} / {string.Format(context.Culture, "{0:F2}", mt.Sum(x => x.FinalPower))} kWh",
                                        Format = TypeFormatText.Span,
                                        Margin = new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
                                    }
                                )
                                {
                                    Fluid = TypePanelContainer.Fluid
                                }
                                );
                            }

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

                            context.VisualTree.Content.Secondary.Add(grid);
                        }
                    }
                }
            }

            context.VisualTree.Content.Primary.Add(new ControlCardCharging()
            {
                Margin = new PropertySpacingMargin(PropertySpacing.Space.Four, PropertySpacing.Space.None)
            });

            item(DateTime.Now.Year);
            item(DateTime.Now.Year - 1);
            item(DateTime.Now.Year - 2);
        }
    }
}
