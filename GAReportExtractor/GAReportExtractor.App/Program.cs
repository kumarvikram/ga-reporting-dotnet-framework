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
                var views = ConfigurationManager.AppSettings["Views"].Split(',');
                var reportingApi = new ReportingApi();
                var reportingService = new ReportingService();
                foreach (var viewId in views)
                {
                    var reportResponse = reportingApi.GetReport(viewId.Trim());
                    reportingService.SaveReportToDisk(reportResponse, viewId.Trim());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                Console.WriteLine("\nClosing App...");
                //Thread.Sleep(3000);
                Console.ReadLine();
            }
        }


    }
}
