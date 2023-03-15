using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Middleware.Main.Entities
    {
        public class InterswitchTokenGenRequest : BaseRequest
        {
            public string subscriberId { get; set; }
            public string ttid
            {
                get
                {
                    return new Random().Next(0000, 9999).ToString();
                }
            }
            public string paymentMethodTypeCode
            {
                get
                {
                    return "VMP";
                }
            }
            public string paymentMethodCode
            {
                get
                {
                    return "UMB";
                }
            }
            public string payWithMobileChannel
            {
                get
                {
                    return "ATM";
                }
            }
            public string tokenLifeTimeInMinutes
            {
                get
                {
                    return "60";
                }
            }
            public string amount { get; set; }
            public string oneTimePin { get; set; }
            public string codeGenerationChannel
            {
                get
                {
                    return "INTERNET_BANKING";
                }
            }
            public string codeGenerationChannelProvider
            {
                get
                {
                    return "UMB";
                }
            }
            public string accountNo { get; set; }
            public string accountType { get; set; }
            public string autoEnroll { get; set; }
            public string transactionRef
            {

                get
                {
                    return ttid;
                }
            }

            private static string GenerateUniqueReferenceId(string senderPhoneNumber)
            {
                return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString("F0") + senderPhoneNumber.Substring(3, 10);
            }
        }

        public class InterswitchTokenGenResponse : InterswitchBaseResponse
        {
            public string subscriberId { get; set; }
            public string payWithMobileToken { get; set; }
            public string tokenLifeTimeInMinutes { get; set; }
            public string transactionRef { get; set; }
        }

        public class InterswitchCancelTokenRequest:BaseRequest
        {
            public string transactionRef { get; set; }
            public string frontEndPartner { get; set; }

        }

        public class InterswitchCancelTokenResponse : InterswitchBaseResponse
        {
            public string code { get; set; }
            public string description { get; set; }
        }

        public class InterswitchTokenStatusRequest
        {
            public string paycode { get; set; }
            public string subscriberID { get; set; }
        }

        public class InterswitchTokenStatusResponse : InterswitchBaseResponse
        {
            public string channel { get; set; }
            public string token { get; set; }
            public string code { get; set; }
            public string description { get; set; }
            public string paymentMethodCode { get; set; }
            public string surcharge { get; set; }
            public string paymentMethodIdentifier { get; set; }
            public string paymentMethodTypeCode { get; set; }
            public string tokenLifeTimeInMinutes { get; set; }
            public string amount { get; set; }
            public string subscriberId { get; set; }
            public string settlementCode { get; set; }
            public string status { get; set; }
            public string frontEndPartner { get; set; }
        }
    }

}
