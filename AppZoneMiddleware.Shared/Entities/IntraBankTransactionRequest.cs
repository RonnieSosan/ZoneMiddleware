using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class IntraBankTransactionRequest : BaseRequest
    {
        public string STAN { get; set; }
        public string RRN { get; set; }
        public string Remakcs { get; set; }
        public string Narration { get; set; }
        public decimal Amount { get; set; }
        public string DebitAccount { get; set; }
        public string FromAccountName { get; set; }
        public string CreditAccount1 { get; set; }
        public Dictionary<string, decimal> SplitAccount { get; set; }
        public string ToAccountName { get; set; }
        public TranType TranType { get; set; }
    }

    public enum TranType
    {
        CustomerPosting = 0,
        GLPosting = 1,
        Reversal = 2,
    }

    public class IntraBankTransactionResponse : BaseResponse
    {

    }

    public class FundsTransferRequest : BaseRequest
    {
        public string NameInqRef
        {
            get
            {
                return string.Format("{0}{1}", customer_id, System.DateTime.Now.ToString("mmyydd:HHMMss"));
            }
        }
        public string MyPaymentReference
        {
            get
            {
                return string.Format("{0}{1}", customer_id, System.DateTime.Now.ToString("mmyyddHHMMss"));
            }
        }
        public string ChannelCode { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string BeneficiaryBank { get; set; }
        public string BeneficiaryBankName { get; set; }
        public string BeneficiaryName { get; set; }
        public string Amount { get; set; }
        public string Remarks { get; set; }
        public string Narration { get; set; }
        public string MyAccountName { get; set; }
        public string OriginatorName { get; set; }
        public string BVN { get; set; }
        public decimal DailyLimit { get; set; }
    }

    public class FundsTransferResponse : BaseResponse
    {
        public string PaymentRef { get; set; }
    }
}
