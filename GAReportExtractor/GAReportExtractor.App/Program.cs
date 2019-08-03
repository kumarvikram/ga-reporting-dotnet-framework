using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace GAReportExtractor.App
{
    class Program
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main()
        {
            try
            {
                //Take View Id's from config
                var views = ConfigurationManager.AppSettings["Views"].Split(',');
                using (var analyticsReportingServiceInstance = GetAnalyticsReportingServiceInstance())
                {
                    var reportingApi = new ReportingApi(analyticsReportingServiceInstance);
                    var reportingService = new ReportingService();
                    foreach (var viewId in views)
                    {                        
                        var reportResponse = reportingApi.GetReport(viewId.Trim());                        
                        reportingService.SaveReportToDisk(reportResponse, viewId.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                Console.WriteLine("\nClosing App...");
                Thread.Sleep(3000);
            }
        }

        /// <summary>
        /// Intializes and returns Analytics Reporting Service Instance using the parameters stored in key file
        /// </summary>
        /// <returns>AnalyticsReportingService</returns>   
        private static AnalyticsReportingService GetAnalyticsReportingServiceInstance()
        {
            string[] scopes = { AnalyticsReportingService.Scope.AnalyticsReadonly }; //Read-only access to Google Analytics
            GoogleCredential credential;
            using (var stream = new FileStream(ConfigurationManager.AppSettings["KeyFileName"], FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
            }
            // Create the  Analytics service.
            var baseClientInitializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GA Extractor",
            };
            return new AnalyticsReportingService(baseClientInitializer);
        }
    }
}
