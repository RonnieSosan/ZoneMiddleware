using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    public class MPassPaymentResponse : BaseResponse
    {
        public MPassPaymentResponseDetails PaymentResponseDetails { get; set; }
    }
    
    [JsonObject]
    public class MPassPaymentResponseDetails
    {
        public string Status { get; set; }
        public string RequestID { get; set; }
        public string TransactionReference { get; set; }
        public string TransferType { get; set; }
        public string SystemTraceAuditNumber { get; set; }
        public string NetworkReferenceNumber { get; set; }
        public string SettlementDate { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
        public string SubmitDateTime { get; set; }
    }
}
