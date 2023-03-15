using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    /// <summary>
    /// Bill payment advice request payload
    /// </summary>
    public class BillPaymentAdviceRequest : BaseRequest
    {
        /// <summary>
        /// Quickteller issued Termianl ID to the bank
        /// </summary>
        public string TerminalId { get; set; }
        /// <summary>
        /// Billers payment item code
        /// </summary>
        public string paymentCode { get; set; }
        /// <summary>
        /// Customer unique code from biller e.g DSTV smart card number
        /// </summary>
        public string customerId
        {
            get
            {
                return CustomerUniqueCode;
            }
        }

        /// <summary>
        /// Customer phone number
        /// </summary>
        public string customerMobile { get; set; }
        /// <summary>
        /// Customer email address
        /// </summary>
        public string customerEmail { get; set; }
        /// <summary>
        /// Bill payment Amount
        /// </summary>
        public string amount { get; set; }

        /// <summary>
        /// Transaction unique reference 
        /// </summary>
        public string requestReference { get; set; }

        /// <summary>
        /// Customer account number
        /// </summary>
        public string AccountNo { get; set; }

        /// <summary>
        /// Banks GL accout for bill payment
        /// </summary>
        public string SuspenseAccount { get; set; }

        /// <summary>
        /// Customer account names
        /// </summary>
        public string CustomerAccountName { get; set; }

        /// <summary>
        /// Customer unique code from biller e.g DSTV smart card number
        /// </summary>
        public string CustomerUniqueCode { get; set; }

        /// <summary>
        /// The currency of the customer account
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Item fee i.e extra amouhnt charged on bill payment by interswitch
        /// </summary>
        public string itemFee { get; set; }

        /// <summary>
        /// explanation code to indicate type of transaction i.e bill payment
        /// </summary>
        public int ExplCode { get; set; }
    }
}
