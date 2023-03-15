using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class GetQuickTellerBillerItemsRequest : BaseRequest
    {
        public string Translocation { get; set; }
        public string BillerID { get; set; }
    }
}
