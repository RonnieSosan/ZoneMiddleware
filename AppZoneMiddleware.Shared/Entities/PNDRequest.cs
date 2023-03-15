using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class PNDRequest : BaseRequest
    {
        public string AccountNumber { get; set; }
        public decimal BranchCode { get; set; }
        public decimal CurrencyCode { get; set; }
        public decimal LedgerCode { get; set; }
        public decimal SubAccountCode { get; set; }
        public decimal RestrictionCode
        {
            get
            {
                return 21;
            }
        }
        public string Restriction_message { get; set; }
        //public string RequestLangInd { get; set; }
        //public string ChannelID { get; set; }
        //public string Channel_Reference_Number { get; set; }
    }

    public class PNDResponse : BaseResponse
    {
    }
}
