using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class UpgradeRequest : BaseRequest
    {
        public string newproduct { get; set; }
    }

    public class UpgradeResponse : BaseResponse { }
}
