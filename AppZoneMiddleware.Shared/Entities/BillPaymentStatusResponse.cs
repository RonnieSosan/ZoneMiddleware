using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    /// <summary>
    /// Bill payment status response paylaod
    /// </summary>
    public class BillPaymentStatusResponse : InterswitchBaseResponse
    {
        /// <summary>
        /// Transaction status response code
        /// </summary>
        public string transactionResponseCode { get; set; }
        /// <summary>
        /// Transaction status
        /// </summary>
        public string status { get; set; }
    }

    /// <summary>
    /// Bill payment processing status request payload
    /// </summary>
    public class BillPaymentStatusRequest : BaseRequest
    {
        /// <summary>
        /// Bill payment unique transaction reference
        /// </summary>
        public string TransactiontReference { get; set; }
    }
}
