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
    public class PayLoadDataHeaders
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
