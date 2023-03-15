using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    public class QuickTellerBillsPaymentAdviceRequestJSON : sPayBaseRequest
    {
        public string amt { get; set; }
        public string paymentcode { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string SubscriberInfo1 { get; set; }
        public string ActionType { get; set; }
        public string nuban { get; set; }
    }
}
