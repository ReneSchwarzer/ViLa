using System;
using System.Collections.Generic;
using System.Linq;
using ViLa.Model;
using WebExpress.WebApp.WebRestApi;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebParameter;

namespace ViLa.WWW.Api._1_.History.Labels._id_
{
    /// <summary>
    /// Manages the labels of a completed measurement.
    /// </summary>
    [Application<Application>]
    [SegmentGuid<ParameterId>]
    [IncludeSubPaths(true)]
    public sealed class Index : RestApiTag
    {
        /// <summary>Returns the labels attached to the measurement.</summary>
        /// <param name="request">The request identifying the measurement.</param>
        /// <returns>The attached labels.</returns>
        protected override IEnumerable<RestApiTagItem> RetrieveTags(IRequest request)
        {
            var log = ViewModel.Instance.GetHistoryMeasurementLog(request.GetParameter<ParameterId>()?.Value);
            return log == null ? [] : BillingPeriod.GetTags(log).Select(tag => new RestApiTagItem { Value = tag });
        }

        /// <summary>Suggests known labels matching the entered text.</summary>
        /// <param name="term">The search text.</param>
        /// <param name="request">The request identifying the measurement.</param>
        /// <returns>The matching labels.</returns>
        protected override IEnumerable<RestApiTagItem> SuggestTags(string term, IRequest request)
        {
            return ViewModel.Instance.Tags.Where(tag => tag.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Take(10).Select(tag => new RestApiTagItem { Value = tag });
        }

        /// <summary>Attaches a label to the measurement.</summary>
        /// <param name="payload">The label to attach.</param>
        /// <param name="request">The request identifying the measurement.</param>
        /// <returns>The attached label or null for an invalid request.</returns>
        protected override RestApiTagItem CreateTag(RestApiTagPayload payload, IRequest request)
        {
            var log = ViewModel.Instance.GetHistoryMeasurementLog(request.GetParameter<ParameterId>()?.Value);
            var value = payload.Value.Trim();
            if (log == null || value.IndexOfAny([' ', ';']) >= 0)
            {
                return null;
            }
            lock (log)
            {
                var tags = BillingPeriod.GetTags(log).ToList();
                if (!tags.Contains(value))
                {
                    tags.Add(value);
                    Save(log, tags);
                }
            }
            return new RestApiTagItem { Value = value };
        }

        /// <summary>Removes a label from the measurement.</summary>
        /// <param name="value">The encoded label name.</param>
        /// <param name="request">The request identifying the measurement.</param>
        /// <returns>Whether the label was removed.</returns>
        protected override bool DeleteTag(string value, IRequest request)
        {
            var log = ViewModel.Instance.GetHistoryMeasurementLog(request.GetParameter<ParameterId>()?.Value);
            if (log == null)
            {
                return false;
            }
            lock (log)
            {
                var tags = BillingPeriod.GetTags(log).ToList();
                if (!tags.Remove(Uri.UnescapeDataString(value)))
                {
                    return false;
                }
                Save(log, tags);
                return true;
            }
        }

        /// <summary>Persists the updated label set.</summary>
        /// <param name="log">The selected measurement.</param>
        /// <param name="tags">The updated labels.</param>
        private static void Save(MeasurementLog log, List<string> tags)
        {
            var original = log.Tag;
            log.Tag = string.Join(";", tags);
            try
            {
                ViewModel.Instance.UpdateMeasurementLog(log);
            }
            catch
            {
                log.Tag = original;
                throw;
            }
        }
    }
}
