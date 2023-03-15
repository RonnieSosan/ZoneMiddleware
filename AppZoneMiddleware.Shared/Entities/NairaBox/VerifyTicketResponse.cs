using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public class VerifyTicketResponse : BaseResponse
    {
        public string ticket { get; set; }
        public string reason { get; set; }
        public string Hash { get; set; }
        public string reference { get; set; }
    }
}
