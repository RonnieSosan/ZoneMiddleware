using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class GetQuickTellerBillerItemsResponse : BaseResponse
    {
        public GetQuickTellerBillerItemsResponseData BillerItemsData { get; set; }
    }
    
    public class GetQuickTellerBillerItemsResponseData
    {
        public string Billers { get; set; }
        public string Status { get; set; }
    }
}
