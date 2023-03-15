using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class BVNInquiryRequest
    {
        public string BVN { get; set; }
    }

    public class BVNInquiryResponse : BaseResponse
    {
        public IList<BVNAccounts> AccountList { get; set; }
    }

    public class BVNAccounts
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountStatus { get; set; }
        public string AccountType { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
    }
}
