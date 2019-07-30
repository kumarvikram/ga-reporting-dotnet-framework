using Google.Apis.AnalyticsReporting.v4.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAReportExtractor.App.Configuration
{
    public class Reports : ConfigurationElementCollection
    {
        public Report this[int index]
        {
            get
            {
                return base.BaseGet(index) as Report;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new Report this[string responseString]
        {
            get { return (Report)BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Report();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Report)element).Name;
        }
    }
}
