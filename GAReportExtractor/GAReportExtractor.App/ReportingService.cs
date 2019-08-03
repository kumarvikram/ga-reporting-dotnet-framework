using GAReportExtractor.App.Configuration;
using Google.Apis.AnalyticsReporting.v4.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using Report = GAReportExtractor.App.Configuration.Report;

namespace GAReportExtractor.App
{
    public class ReportingService
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void SaveReportToDisk(GetReportsResponse reportsResponse, string viewId)
        {
            try
            {
                if (reportsResponse != null && reportsResponse.Reports.Any(report => report.Data.Rows != null))
                {
                    Logger.Info("Generating extract file...");

                    var outputDirectory = ConfigurationManager.AppSettings["OutputDirectory"];
                    Directory.CreateDirectory(outputDirectory); //Create directory if it doesn't exist

                    var reportName = GetReportNameByMetric(reportsResponse.Reports[0].ColumnHeader.MetricHeader.MetricHeaderEntries[0].Name);
                    var fileName = string.Format("{0}_{1}_{2}.json", reportName, viewId, DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.CurrentCulture));

                    var outputList = new List<object>();
                    foreach (var row in reportsResponse.Reports.SelectMany(r => r.Data.Rows))
                    {
                        outputList.Add(new { Dimensions = row.Dimensions, Metrics = row.Metrics.SelectMany(m => m.Values) });
                    }

                    File.WriteAllText(string.Format(@"{0}\{1}", outputDirectory, fileName), JsonConvert.SerializeObject(outputList));
                    Logger.Info("Finished geneating extract file...");
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error in geneating extract file: " + ex);
            }
        }

        /// <summary>
        /// Get report name from config file by any metric
        /// </summary>
        /// <param name="metric"></param>
        /// <returns></returns>
        private string GetReportNameByMetric(string metric)
        {
            var config = ReportConfiguration.GetConfig();
            foreach (var item in config.Reports)
            {
                var report = item as Report;
                if (report != null && report.Metrics.Contains(metric))
                {
                    return report.Name;
                }
            }
            return string.Empty;
        }
       
    }
}
