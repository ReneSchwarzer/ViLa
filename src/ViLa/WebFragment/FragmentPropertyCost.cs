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
    [Scope<IScopeGeneral>]
    public sealed class FragmentPropertyCost : FragmentControlList
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="fragmentContext">The context for the fragment.</param>
        public FragmentPropertyCost(IFragmentContext fragmentContext)
            : base(fragmentContext)
        {
            Margin = _ => new PropertySpacingMargin(PropertySpacing.Space.Two);
        }

        /// <summary>
        /// Adds one yearly cost and consumption summary to the list.
        /// </summary>
        /// <param name="year">The year to display.</param>
        /// <param name="from">The inclusive start of the period.</param>
        /// <param name="till">The exclusive end of the period.</param>
        /// <param name="culture">The culture used for number formatting.</param>
        private void AddYearSummary(int year, DateTime from, DateTime till, CultureInfo culture)
        {
            var values = ViewModel.Instance.GetHistoryMeasurementLogs()
                .Where(x => x.Till >= from && x.Till < till).ToList();

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
                    Value = _ => BillingPeriod.FormatCosts(values, culture, ViewModel.Instance.Settings.Currency),
                    TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
                }
            );
            item.Add
            (
                new ControlAttribute()
                {
                    Icon = _ => new IconBolt(),
                    Value = _ => string.Format(culture, "{0:F2} kWh", values.Sum(x => x.FinalPower)),
                    TextColor = _ => new PropertyColorText(TypeColorText.Secondary)
                }
            );

            foreach (var tag in values
                .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
                .SelectMany(BillingPeriod.GetTags)
                .Distinct())
            {
                var taggedValues = values
                    .Where(x => !string.IsNullOrWhiteSpace(x.Tag))
                    .Where(x => BillingPeriod.GetTags(x).Contains(tag))
                    .ToList();

                var panel = new ControlPanel();
                panel.Add
                (
                    new ControlTag()
                    {
                        Value = _ => tag,
                        Color = _ => new PropertyColorTag(ViewModel.Instance.GetColor(tag))
                    }
                );
                panel.Add
                (
                    new ControlText()
                    {
                        Text = _ => string.Format
                        (
                            culture,
                            "{0} / {1:F2} kWh",
                            BillingPeriod.FormatCosts(taggedValues, culture, ViewModel.Instance.Settings.Currency),
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
            foreach (var item in Items.ToArray())
            {
                Remove(item);
            }

            var culture = renderContext.Request.Culture ?? CultureInfo.CurrentCulture;
            var now = DateTime.Now;

            AddYearSummary(now.Year, new DateTime(now.Year, 1, 1), now, culture);
            AddYearSummary(now.Year - 1, new DateTime(now.Year - 1, 1, 1), new DateTime(now.Year, 1, 1), culture);
            AddYearSummary(now.Year - 2, new DateTime(now.Year - 2, 1, 1), new DateTime(now.Year - 1, 1, 1), culture);

            return base.Render(renderContext, visualTree);
        }
    }
}
