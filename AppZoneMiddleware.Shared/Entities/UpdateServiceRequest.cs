using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class UpdateServiceRequest
    {
        public string Solution { get; set; }
        public string Service { get; set; }
        public string Password { get; set; }
    }
}
