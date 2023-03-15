using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class QuickTellerBillsPaymentAdviceResponse : BaseResponse
    {
        public QuickTellerBillsPaymentAdviceResponseData ResponseDetails { get; set; }
    }
    
    public class QuickTellerBillsPaymentAdviceResponseData
    {
        public string BillerResponse { get; set; }
        public string Status { get; set; }
    }
}
