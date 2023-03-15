using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    /// <summary>
    /// Quickteller payment item details
    /// </summary>
    public class Paymentitem
    {
        public string categoryid { get; set; }
        public string billerid { get; set; }
        public string paymentitemid { get; set; }
        public string paymentitemname { get; set; }
        public string amount { get; set; }
        public string code
        {
            get
            {
                return paymentCode;
            }
        }
        public string paymentCode { get; set; }
        public string currencyCode { get; set; }
        public string itemFee { get; set; }
    }

    /// <summary>
    /// list of quickteller biller payment items
    /// </summary>
    public class QucktellerPaymentItems : InterswitchBaseResponse
    {
        public List<Paymentitem> paymentitems { get; set; }
    }
}
