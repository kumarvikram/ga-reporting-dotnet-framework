using System.Configuration;

namespace GAReportExtractor.Library.Configuration
{
    public class ReportConfiguration : ConfigurationSection
    {
        public static ReportConfiguration GetConfig()
        {
            return (ReportConfiguration)ConfigurationManager.GetSection("reportConfiguration") ?? new ReportConfiguration();
        }

        [ConfigurationProperty("reports")]
        [ConfigurationCollection(typeof(Reports), AddItemName = "report")]
        public Reports Reports
        {
            get
            {
                object o = this["reports"];
                return o as Reports;
            }
        }

        [ConfigurationProperty("dateConfiguration")]
        public DateConfigurationElement DateConfiguration
        {
            get
            {
                return base["dateConfiguration"] as DateConfigurationElement;
            }
        }

    }
}
