using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class SMSRequest:BaseRequest
    {
       public string Message { get; set; }
    }

    public class SMSResponse : BaseResponse { }
}
