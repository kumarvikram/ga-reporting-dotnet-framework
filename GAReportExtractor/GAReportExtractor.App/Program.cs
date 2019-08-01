using System;
using System.Configuration;
using System.Threading;

namespace GAReportExtractor.App
{
    class Program
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            try
            {
                //Take View Id's from config
                var views = ConfigurationManager.AppSettings["Views"].Split(',');                
                foreach (var viewId in views)
                {                    
                    ReportManager.GetReport(viewId.Trim());
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                Console.WriteLine("\nClosing App...");
                Thread.Sleep(3000);
            }
        }
    }
}
