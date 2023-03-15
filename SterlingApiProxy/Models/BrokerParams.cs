using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SterlingApiProxy.Models
{
    public class BrokerParams
    {
        public string ApiUrl { get; set; }
        public string PayLoad { get; set; }
        public string Result { get; set; }
    }

    public class PayLoadData
    {
        public bool IsSoap { get; set; }
        public string Payload { get; set; }
        public PayLoadDataHeaders[] Headers { get; set; }
    }

    public class PayLoadDataHeaders
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
