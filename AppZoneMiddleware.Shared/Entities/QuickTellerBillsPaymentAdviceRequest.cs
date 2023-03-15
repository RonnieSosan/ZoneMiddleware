using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class QuickTellerBillsPaymentAdviceRequest : BaseRequest
    {
        public string Translocation { get; set; }
        public string Amount { get; set; }
        public string PaymentCode { get; set; }
        public string Email { get; set; }
        public string SubscriberInfo1 { get; set; }
        public string ActionType { get; set; }
        public string NUBAN { get; set; }
    }
}
