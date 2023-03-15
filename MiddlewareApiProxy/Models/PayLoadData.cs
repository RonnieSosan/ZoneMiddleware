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
    public class PayLoadData
    {
        public bool IsSoap { get; set; }
        public string Payload { get; set; }
        public PayLoadDataHeaders[] Headers { get; set; }
    }
}
