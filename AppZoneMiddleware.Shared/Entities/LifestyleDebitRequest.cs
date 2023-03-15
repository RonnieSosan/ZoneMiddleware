using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class LifestyleDebitRequest : BaseRequest
    {
        public string RefId { get; set; }
        public string AccountNumber { get; set; }
        public string MerchantId { get; set; }
        public string Narration { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
    }
}
