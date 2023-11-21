using GAReportExtractor.Library;
using System;
using System.Configuration;
using System.Threading;

namespace GAReportExtractor.App
{
    class Program
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(Program));
        static void Main()
        {
            try
            {
                //Take View Id's from config
                var propertyId = ConfigurationManager.AppSettings["PropertyId"];
                var reportingApi = new ReportingApi();
                var reportingService = new ReportingService();
                var reportResponse = reportingApi.GetReport(propertyId);
                reportingService.SaveReportToDisk(reportResponse, propertyId);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                Console.WriteLine("\nClosing App...");
                Console.ReadLine();
            }
        }


    }
}
