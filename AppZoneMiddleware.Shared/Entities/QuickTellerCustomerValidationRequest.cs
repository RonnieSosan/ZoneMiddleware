using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    public class QuickTellerCustomerValidationRequest : BaseRequest
    {
        public string Translocation { get; set; }
        public string BillerPaymentCode { get; set; }
        public string BillerCustomerId { get; set; }
    }
}
