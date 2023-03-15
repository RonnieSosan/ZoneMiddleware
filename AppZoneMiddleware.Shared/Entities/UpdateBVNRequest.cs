using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class UpdateBVNRequest : BaseRequest
    {
        public string BVN { get; set; }
        public string AccountNumber { get; set; }
    }

    public class UpdateBvnResponse : BaseResponse { }
}
