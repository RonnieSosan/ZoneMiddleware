using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.AuthenticationProxy
{
    public class HealthCheckResponse
    {
        public bool TimeAssertionSuccess { get; set; }
        public bool ResponseAssertionSuccess { get; set; }
        public int TimeElapsed { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
