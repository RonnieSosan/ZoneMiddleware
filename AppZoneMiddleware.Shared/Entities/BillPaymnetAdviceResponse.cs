using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class BillPaymnetAdviceResponse : InterswitchBaseResponse
    {
        public string Amount { get; set; }
        public string transactionRef { get; set; }
        public string requestReference { get; set; }
    }
}
