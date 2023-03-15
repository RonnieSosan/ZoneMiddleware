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
    public class PayLoadDataHeader
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
