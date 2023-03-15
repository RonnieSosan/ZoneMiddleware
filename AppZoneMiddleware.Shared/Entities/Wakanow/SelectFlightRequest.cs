using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public class SelectFlightRequest
    {
        [JsonProperty("TargetCurrency")]
        public string TargetCurrency { get; set; }

        [JsonProperty("SelectData")]
        public string SelectData { get; set; }
    }
}
