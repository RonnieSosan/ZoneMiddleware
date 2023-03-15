using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    [JsonObject]
    public class MPassPaymentResponseJSON
    {
        public string message { get; set; }
        public string response { get; set; }
        public MPassPaymentResponseJSONDetails data { get; set; }
    }

    [JsonObject]
    public class MPassPaymentResponseJSONDetails
    {
        public string status { get; set; }
        public string RequestId { get; set; }
        public string TransactionReference { get; set; }
        public string TransferType { get; set; }
        public string SystemTraceAuditNumber { get; set; }
        public string NetworkReferenceNumber { get; set; }
        public string SettlementDate { get; set; }
        public string ResponseCode { get; set; }
        public string Response_Description { get; set; }
        public string SubmitDateTime { get; set; }
    }
}
