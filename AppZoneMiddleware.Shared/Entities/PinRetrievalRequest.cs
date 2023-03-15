using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class PinRetrievalRequest:BaseRequest
    {
        public string AccountNumber { get; set; }
        public string Last4Pan { get; set; }
        public string Bin { get; set; }
        public string ExpDate { get; set; }
    }
}
