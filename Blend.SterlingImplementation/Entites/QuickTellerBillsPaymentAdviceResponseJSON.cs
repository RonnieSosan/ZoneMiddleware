using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    public class QuickTellerBillsPaymentAdviceResponseJSON
    {
        public string message { get; set; }
        public string response { get; set; }
        public QuickTellerBillsPaymentAdviceResponseJSONData data { get; set; }
    }
    
    public class QuickTellerBillsPaymentAdviceResponseJSONData
    {
        public string billerResponse { get; set; }
        public string status { get; set; }
    }

}
