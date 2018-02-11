using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Configuration;
using System.IO;

namespace GAReportExtractor.App
{
    public class ReportManager
    {
        /// <summary>
        /// Intializes and returns Analytics Reporting Service Instance using the parameters stored in key file
        /// </summary>
        /// <param name="keyFileName"></param>
        /// <returns></returns>
        private static AnalyticsReportingService GetAnalyticsReportingServiceInstance(string keyFileName)
        {
            string[] scopes = { AnalyticsReportingService.Scope.AnalyticsReadonly }; //Read-only access to Google Analytics
            GoogleCredential credential;
            using (var stream = new FileStream(keyFileName, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
            }
            // Create the  Analytics service.
            return new AnalyticsReportingService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GA Reporting data extraction example",
            });
        }

        /// <summary>
        /// Fetches all required reports from Google Analytics
        /// </summary>
        /// <param name="reportRequests"></param>
        /// <returns></returns>
        public static GetReportsResponse GetReport(GetReportsRequest getReportsRequest)
        {
            var analyticsService = GetAnalyticsReportingServiceInstance(ConfigurationManager.AppSettings["KeyFileName"]);
            return analyticsService.Reports.BatchGet(getReportsRequest).Execute();
        }
    }
}
