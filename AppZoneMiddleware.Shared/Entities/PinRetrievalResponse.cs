using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class PinRetrievalResponse : BaseResponse
    {
        public string Successful { get; set; }
        public string PIN { get; set; }
        public string NextToken { get; set; }
        public string ErrorMessage { get; set; }
    }
}
