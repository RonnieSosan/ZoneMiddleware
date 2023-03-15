using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class BlockFundRequest : BaseRequest
    {
        public string AccountNumber { get; set; }
        public string Amount { get; set; }
    }

    public class BlockFundResponse : BaseResponse
    {
        public string LockID { get; set; }
    }

    public class UnBlockFundRequest : BaseRequest
    {
        public string LockID { get; set; }
    }

    public class UnBlockFundResponse : BaseResponse
    {
        public string LockID { get; set; }
    }

}
