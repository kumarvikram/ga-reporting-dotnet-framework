using Google.Analytics.Data.V1Beta;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GAReportExtractor.Library
{
    public class ReportingService
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void SaveReportToDisk(RunReportResponse reportsResponse, string propertyId)
        {
            try
            {
                if (reportsResponse != null)
                {
                    Logger.Info("Generating extract file...");

                    var outputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];
                    Directory.CreateDirectory(outputDirectory); //Create directory if it doesn't exist

                    var delimiter = ConfigurationManager.AppSettings["Delimiter"];
                  
                    var fileName = string.Format("GA4Report_{1}_{2}.json", propertyId.Trim(), DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.CurrentCulture));
                    
                    File.WriteAllText(string.Format(@"{0}\{1}", outputDirectory, fileName), JsonConvert.SerializeObject(reportsResponse));
                    Logger.Info("Finished geneating extract file...");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error in geneating extract file: " + ex);
            }
        }
       
    }
}
