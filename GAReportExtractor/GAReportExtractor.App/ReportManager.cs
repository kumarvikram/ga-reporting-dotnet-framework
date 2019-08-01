using GAReportExtractor.App.Configuration;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using GAReport = Google.Apis.AnalyticsReporting.v4.Data.Report;
using Report = GAReportExtractor.App.Configuration.Report;

namespace GAReportExtractor.App
{
    public class ReportManager
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Intializes and returns Analytics Reporting Service Instance using the parameters stored in key file
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Create date range based on entries in configuration
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private static List<DateRange> GetDateRange(ReportConfiguration config)
        {
            var strStartDateFromConfig = config.DateConfiguration.StartDate;
            var strEndDateFromConfig = config.DateConfiguration.EndDate;
            var strNumberOfDaysFromConfig = config.DateConfiguration.NumberOfDays;

            DateTime.TryParse(strStartDateFromConfig, out DateTime reportStartDate);

            DateTime.TryParse(strEndDateFromConfig, out DateTime reportEndDate);

            int.TryParse(strNumberOfDaysFromConfig, out int numberOfDays);

            //Set start and end date for report using number of days
            var startDate = DateTime.Now.AddDays(-numberOfDays);
            var endDate = numberOfDays == 0 ? DateTime.Now : DateTime.Now.AddDays(-1);

            //Use start and end date from config if specified else keep the existing values
            if (reportStartDate != DateTime.MinValue && reportEndDate != DateTime.MinValue &&
                reportStartDate <= reportEndDate)
            {
                startDate = reportStartDate;
                endDate = reportEndDate;
            }

            return new List<DateRange>
                {
                    new DateRange
                    {
                        StartDate = startDate.ToString("yyyy-MM-dd"),
                        EndDate = endDate.ToString("yyyy-MM-dd")
                    }
                };
        }
        
        /// <summary>
        /// Get all reports configured in App.config
        /// </summary>
        /// <returns></returns>
        public static void GetReport(string viewId)
        {
            try
            {
                Logger.Info("Processing View Id: " + viewId);
                var config = ReportConfiguration.GetConfig();

                foreach (var item in config.Reports)
                {
                    Report report = item as Report;
                    if (report != null)
                    {                        
                        var stopwatch = new Stopwatch();
                        Logger.Info("Started fetching report: " + report.Name);
                        // Create the Metrics and dimensions object based on configuration.
                        var metrics = report.Metrics.Split(',').Select(m => new Metric { Expression = m }).ToList();
                        var dimensions = report.Dimensions.Split(',').Select(d => new Dimension { Name = d }).ToList();
                        var reportRequest = new ReportRequest
                        {
                            DateRanges = GetDateRange(config),
                            Metrics = metrics,
                            Dimensions = dimensions,
                            ViewId = viewId,
                            SamplingLevel = "LARGE", //https://developers.google.com/analytics/devguides/reporting/core/v4/basics#sampling
                            PageSize = 10000 //The Analytics Core Reporting API returns a maximum of 10,000 rows per request, no matter how many you ask for. https://developers.google.com/analytics/devguides/reporting/core/v4/rest/v4/reports/batchGet
                        };

                        var combinedReportResponse = new GetReportsResponse { Reports = new List<GAReport>() };
                        stopwatch.Start();
                        var reportsResponse = GetAnalyticsReportingServiceInstance().Reports.BatchGet(new GetReportsRequest { ReportRequests = new List<ReportRequest> { reportRequest } }).Execute();
                        
                        if (reportsResponse != null)
                        {
                            ((List<GAReport>)combinedReportResponse.Reports).AddRange(reportsResponse.Reports);
                            while (reportsResponse.Reports[0].NextPageToken != null)
                            {
                                reportRequest.PageToken = reportsResponse.Reports[0].NextPageToken;
                                reportsResponse = GetAnalyticsReportingServiceInstance().Reports.BatchGet(new GetReportsRequest { ReportRequests = new List<ReportRequest> { reportRequest } }).Execute();
                                ((List<GAReport>)combinedReportResponse.Reports).AddRange(reportsResponse.Reports);
                            }
                            stopwatch.Stop();
                            SaveReportToDisk(combinedReportResponse, viewId);
                        }
                        Logger.Info("Finished fetching report: " + report.Name);
                        Logger.Info(string.Format("Time elapsed: {0:hh\\:mm\\:ss}", stopwatch.Elapsed));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error in fetching reports: " + ex);
            }
        }

        private static void SaveReportToDisk(GetReportsResponse reportsResponse, string viewId)
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

                    var outputList=new List<object>();
                    foreach(var row in reportsResponse.Reports.SelectMany(r=>r.Data.Rows))
                    {
                      outputList.Add(new {Dimensions=row.Dimensions, Metrics=row.Metrics.SelectMany(m => m.Values)});
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
        private static string GetReportNameByMetric(string metric)
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

        private static void PrintReport(GetReportsResponse response)
        {
            var reports = response.Reports;
            foreach (var report in reports)
            {
                var header = report.ColumnHeader;
                var dimensionHeaders = header.Dimensions;
                var metricHeaders = header.MetricHeader.MetricHeaderEntries;
                var rows = report.Data.Rows;
                if (!rows.Any())
                {
                    Logger.Info("No data found!");
                    return;
                }
                foreach (var row in rows)
                {
                    var dimensions = row.Dimensions;
                    var metrics = row.Metrics;

                    for (int i = 0; i < dimensionHeaders.Count && i < dimensions.Count; i++)
                    {
                        Console.WriteLine(dimensionHeaders[i] + ": " + dimensions[i]);
                    }
                    for (int j = 0; j < metrics.Count; j++)
                    {
                        DateRangeValues values = metrics[j];
                        for (int k = 0; k < values.Values.Count && k < metricHeaders.Count; k++)
                        {
                            Console.WriteLine(metricHeaders[k].Name + ": " + values.Values[k]);
                        }
                    }

                }
            }
        }
    }
}
