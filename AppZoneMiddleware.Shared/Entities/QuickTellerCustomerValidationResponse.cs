using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    public class QuickTellerCustomerValidationResponse : BaseResponse
    {
        public string QuickTellerResponseCode { get; set; }
        
        public string QuickTellerResponseStatus { get; set; }

        public QuickTellerCustomerValidationDetails CustomerDetails { get; set; }
    }

    [JsonObject]
    public class QuickTellerCustomerValidationDetails
    {        
        public string CustomerValidationField { get; set; }
        
        public string PaymentCode { get; set; }

        public string CustomerId { get; set; }

        public string WithDetails { get; set; }

        public string ResponseCode { get; set; }

        public string FullName { get; set; }

        public string Amount { get; set; }
    }
}
