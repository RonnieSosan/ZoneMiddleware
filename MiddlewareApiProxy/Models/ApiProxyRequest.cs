using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiddlewareApiProxy.Models
{
    [JsonObject]
    [Serializable]
    public class ApiProxyRequest
    {
        public string EndPointURL { get; set; }
        public AuthenticationData AuthData { get; set; }
        public PayLoadData PayLoad { get; set; }
    }
}
