using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAReportExtractor.App.Configuration
{
    public class DateConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("StartDate", IsRequired = true)]
        public string StartDate
        {
            get
            {
                return this["StartDate"] as string;
            }
        }

        [ConfigurationProperty("EndDate", IsRequired = true)]
        public string EndDate
        {
            get
            {
                return this["EndDate"] as string;
            }
        }
        [ConfigurationProperty("NumberOfDays", IsRequired = true)]
        public string NumberOfDays
        {
            get
            {
                return this["NumberOfDays"] as string;
            }
        }
    }
}
