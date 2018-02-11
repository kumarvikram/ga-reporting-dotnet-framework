using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace GAReportExtractor.App
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                #region Prepare Report Request object 
                // Create the DateRange object. Here we want data from last week.
                var dateRange = new DateRange
                {
                    StartDate = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd"),
                    EndDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
                };
                // Create the Metrics and dimensions object.
                var metrics = new List<Metric> { new Metric { Expression = "ga:sessions", Alias = "Sessions" } };
                var dimensions = new List<Dimension> { new Dimension { Name = "ga:pageTitle" } };

                //Get required View Id from configuration
                var ViewId = ConfigurationManager.AppSettings["ViewId"];

                // Create the Request object.
                var reportRequest = new ReportRequest
                {
                    DateRanges = new List<DateRange> { dateRange },
                    Metrics = metrics,
                    Dimensions = dimensions,
                    ViewId = ViewId
                };
                var getReportsRequest = new GetReportsRequest();
                getReportsRequest.ReportRequests = new List<ReportRequest> { reportRequest };

                var response = ReportManager.GetReport(getReportsRequest);
                #endregion

                //Print report data to console
                PrintReport(response);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }
        private static void PrintReport(GetReportsResponse response)
        {
            foreach (var report in response.Reports)
            {
                var rows = report.Data.Rows;
                ColumnHeader header = report.ColumnHeader;
                var dimensionHeaders = header.Dimensions;
                var metricHeaders = header.MetricHeader.MetricHeaderEntries;
                if (!rows.Any())
                {
                    Console.WriteLine("No data found!");
                    return;
                }
                else
                {
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
}
