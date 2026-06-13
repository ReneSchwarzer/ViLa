using System;
using System.Globalization;
using System.Linq;
using ViLa.Model;
using WebExpress.WebApp.WebScope;
using WebExpress.WebApp.WebSection;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebFragment;
using WebExpress.WebCore.WebHtml;
using WebExpress.WebUI.WebControl;
using WebExpress.WebUI.WebFragment;
using WebExpress.WebUI.WebIcon;
using WebExpress.WebUI.WebPage;

namespace ViLa.WebFragment
{
    /// <summary>
    /// Represents a fragment control for displaying property cost information.
    /// </summary>
    [Section<SectionPropertyPrimary>]
    [Application<Application>]
    [Scope<ViLa.WWW.Index>]
    [Scope<ViLa.WWW.History.Index>]
    [Scope<IScopeGeneral>]
    [Cache]
    public sealed class FragmentPropertyCost : FragmentControlList
    {
        private bool ContentBuilt { get; set; }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context for the fragment.</param>
        public FragmentPropertyCost(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
            Layout = _ => TypeLayoutList.Flush;
        }

        /// <summary>
        /// Adds one yearly cost and consumption summary to the list.
        /// </summary>
        /// <param name="year">The year to display.</param>
        /// <param name="from">The inclusive start of the period.</param>
        /// <param name="till">The inclusive end of the period.</param>
        /// <param name="culture">The culture used for number formatting.</param>
        private void AddYearSummary(int year, DateTime from, DateTime till, CultureInfo culture)
        {
            var values = ViewModel.Instance.GetHistoryMeasurementLogs(from, till).ToList();

            if (!values.Any())
            {
                return;
            }

            var item = new ControlListItem();
            item.Add
            (
                new ControlText()
                {
                    Text = _ => year.ToString(culture),
                    Format = _ => TypeFormatText.H4,
                    TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
                }
            );
            item.Add
            (
                new ControlAttribute()
                {
                    Icon = _ => new IconEuroSign(),
                    Value = _ => string.Format(culture, "{0:F2} {1}", values.Sum(x => x.Cost), ViewModel.Instance.Settings.Currency),
                    TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
                }
            );
            item.Add
            (
                new ControlAttribute()
                {
                    Icon = _ => new IconBolt(),
                    Value = _ => string.Format(culture, "{0:F2} kWh", values.Sum(x => x.Power)),
                    TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
                }
            );

            foreach (var tag in values
                .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
                .SelectMany(x => x.Tag.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .Distinct())
            {
                var taggedValues = values
                    .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
                    .Where(x => x.Tag.Contains(tag))
                    .ToList();

                var panel = new ControlPanel();
                panel.Add
                (
                    new ControlTag(tag)
                    {
                        BackgroundColor = _ => new PropertyColorBackground(ViewModel.Instance.GetColor(tag))
                    }
                );
                panel.Add
                (
                    new ControlText()
                    {
                        Text = _ => string.Format
                        (
                            culture,
                            "{0:F2} {1} / {2:F2} kWh",
                            taggedValues.Sum(x => x.FinalCost),
                            ViewModel.Instance.Settings.Currency,
                            taggedValues.Sum(x => x.FinalPower)
                        ),
                        Format = _ => TypeFormatText.Span,
                        TextColor = _ => new PropertyColorText(TypeColorText.Secondary),
                        Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.None, PropertySpacing.Space.None, PropertySpacing.Space.Two, PropertySpacing.Space.None)
                    }
                );

                item.Add(panel);
            }

            Add(item);
        }

        /// <summary>  
        /// Renders the control into an HTML node.  
        /// </summary>  
        /// <param name="renderContext">The render context.</param>  
        /// <param name="visualTree">The visual tree control.</param>  
        /// <returns>The rendered HTML node.</returns>
        public override IHtmlNode Render(IRenderControlContext renderContext, IVisualTreeControl visualTree)
        {
            if (ContentBuilt)
            {
                return base.Render(renderContext, visualTree);
            }

            var culture = renderContext.Request.Culture ?? CultureInfo.CurrentCulture;
            var now = DateTime.Now;

            AddYearSummary(now.Year, new DateTime(now.Year, 1, 1), now, culture);
            AddYearSummary(now.Year - 1, new DateTime(now.Year - 1, 1, 1), new DateTime(now.Year - 1, 12, 31), culture);
            AddYearSummary(now.Year - 2, new DateTime(now.Year - 2, 1, 1), new DateTime(now.Year - 2, 12, 31), culture);

            ContentBuilt = true;

            return base.Render(renderContext, visualTree);
        }
    }
}
