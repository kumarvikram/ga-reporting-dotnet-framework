using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace GAReportExtractor.Library.Configuration
{
    public class Report : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("metrics", IsRequired = true)]
        public string Metrics
        {
            get
            {
                return this["metrics"] as string;
            }
        }
        [ConfigurationProperty("dimensions", IsRequired = true)]
        public string Dimensions
        {
            get
            {
                return this["dimensions"] as string;
            }
        }
        [ConfigurationProperty("recordCount", IsRequired = true)]
        public int RecordCount
        {
            get
            {
                int.TryParse(this["recordCount"] as string, out int recordCount);
                return recordCount;
            }
        }
        [ConfigurationProperty("orderBy", IsRequired = true)]
        public string OrderBy
        {
            get
            {
                return this["orderBy"] as string;
            }
        }
    }
}
