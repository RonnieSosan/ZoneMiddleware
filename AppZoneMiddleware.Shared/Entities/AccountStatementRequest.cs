using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class AccountStatementRequest : BaseRequest
    {
        public string AccountNumber { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int count { get; set; }
    }

    public class AccountStatementDetails
    {
        public string TransactionDate { get; set; }
        public string ValueDate { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Remarks { get; set; }
        public string Balance { get; set; }
        public string TransactionReference { get; set; }
    }

    public class AccountStatementResponse : BaseResponse
    {
        public IList<AccountStatementDetails> AccountStatements { get; set; }
    }
}
