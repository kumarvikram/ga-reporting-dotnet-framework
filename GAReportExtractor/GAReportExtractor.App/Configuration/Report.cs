using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAReportExtractor.App.Configuration
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
    }
}
