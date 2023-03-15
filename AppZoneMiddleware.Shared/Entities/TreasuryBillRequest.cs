using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class TreasuryBillRequest:BaseRequest
    {
        public string NUBAN { get; set; }
        public string tenor { get; set; }
        public string f_value { get; set; }
    }

    public class TreasuryBillResponse : BaseResponse
    { }
}
