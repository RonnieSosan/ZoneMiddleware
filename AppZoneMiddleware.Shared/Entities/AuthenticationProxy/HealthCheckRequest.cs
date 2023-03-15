using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.AuthenticationProxy
{
    [JsonObject]
    [Serializable]
    public class HealthCheckRequest : ApiProxyRequest
    {
        public string ResponseAssertion { get; set; }
        public int TimeAssertion { get; set; }
    }
}
