using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class CreditCheckRequest : BaseRequest
    {
        public string dob { get; set; }//29-JUN-1964
        public string gender { get; set; }//001
        public string BVN { get; set; }//22268769282
    }

    public class CreditCheckResponse : BaseResponse { }
}
