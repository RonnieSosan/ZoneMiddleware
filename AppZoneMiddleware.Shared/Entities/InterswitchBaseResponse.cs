using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class InterswitchBaseResponse : BaseResponse
    {
        public error error { get; set; }
    }

    public class error
    {
        public string code { get; set; }
        public string message { get; set; }
    }
}
