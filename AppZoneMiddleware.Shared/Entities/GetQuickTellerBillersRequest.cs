using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class GetQuickTellerBillersRequest : BaseRequest
    {
        public string Translocation { get; set; }
    }
}
