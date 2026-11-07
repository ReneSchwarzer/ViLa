using ViLa.Model;
using System;
using System.IO;
using System.Text.Json;
using WebExpress.WebCore.Internationalization;
using WebExpress.WebCore.WebParameter;
using WebExpress.WebCore.WebAttribute;
using WebExpress.WebCore.WebMessage;
using WebExpress.WebCore.WebRestApi;

namespace ViLa.WWW.Api._1_
{
    /// <summary>
    /// Downloads the complete persisted ViLa data backup.
    /// </summary>
    [Application<Application>]
    public sealed class Backup : IRestApi
    {
        /// <summary>
        /// Imports a ZIP submitted by the standard WebExpress upload control.
        /// </summary>
        /// <param name="request">The multipart request containing one backup file.</param>
        /// <returns>The import counts or a localized error with an unsuccessful HTTP status.</returns>
        [Method(RequestMethod.POST)]
        public IResponse Import(IRequest request)
        {
            try
            {
                var file = request?.GetParameter("file") as ParameterFile;
                var result = ViewModel.Instance.ImportData(file?.Data);
                return new ResponseOK
                {
                    Content = JsonSerializer.SerializeToUtf8Bytes(new
                    {
                        result.MeasurementsAdded, result.VehiclesAdded, result.SettingsAdded, result.Skipped
                    })
                }.AddHeaderContentType("application/json");
            }
            catch (Exception exception) when (exception is InvalidDataException || exception is InvalidOperationException
                || exception is JsonException || exception is ArgumentException)
            {
                return Error(new ResponseBadRequest(), request, "vila:vila.backup.invalid");
            }
            catch (Exception exception) when (exception is IOException || exception is UnauthorizedAccessException)
            {
                return Error(new ResponseInternalServerError(), request, "vila:vila.backup.failed");
            }
        }

        /// <summary>
        /// Produces the localized error consumed by the upload transport.
        /// </summary>
        /// <param name="response">The response defining the failure status.</param>
        /// <param name="request">The request supplying the language.</param>
        /// <param name="key">The error translation key.</param>
        /// <returns>The JSON error response.</returns>
        private static IResponse Error(Response response, IRequest request, string key)
        {
            response.Content = JsonSerializer.SerializeToUtf8Bytes(new { message = I18N.Translate(request, key) });
            return response.AddHeaderContentType("application/json");
        }

        /// <summary>
        /// Returns a portable ZIP attachment.
        /// </summary>
        /// <param name="request">The download request.</param>
        /// <returns>The backup response.</returns>
        [Method(RequestMethod.GET)]
        public IResponse Retrieve(IRequest request)
        {
            var includeArchive = string.Equals(request?.GetParameter("includeArchive")?.Value, "true", System.StringComparison.OrdinalIgnoreCase);
            var response = new ResponseOK { Content = ViewModel.Instance.ExportData(includeArchive) };
            response.Header.ContentType = "application/zip";
            response.Header.ContentDisposition = "attachment; filename=\"ViLa-backup.zip\"";
            return response;
        }
    }
}
