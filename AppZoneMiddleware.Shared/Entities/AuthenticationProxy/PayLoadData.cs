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
    public class PayLoadData
    {
        public bool IsSoap { get; set; }
        public string Body { get; set; }
        public PayLoadDataHeader[] Headers { get; set; }
    }
}
