using System.Configuration;

namespace GAReportExtractor.Library.Configuration
{
    public class ReportConfiguration : ConfigurationSection
    {
        public static ReportConfiguration GetConfig()
        {
            return (ReportConfiguration)ConfigurationManager.GetSection("ReportConfiguration") ?? new ReportConfiguration();
        }

        [ConfigurationProperty("Reports")]
        [ConfigurationCollection(typeof(Reports), AddItemName = "Report")]
        public Reports Reports
        {
            get
            {
                object o = this["Reports"];
                return o as Reports;
            }
        }

        [ConfigurationProperty("DateConfiguration")]
        public DateConfigurationElement DateConfiguration
        {
            get
            {
                return base["DateConfiguration"] as DateConfigurationElement;
            }
        }

    }
}
