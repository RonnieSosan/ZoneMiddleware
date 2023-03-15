using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{

    public partial class WakanowAuthenticationData
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("Identifier")]
        public string Identifier { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("AgentType")]
        public string AgentType { get; set; }

        [JsonProperty("Market")]
        public string Market { get; set; }

        [JsonProperty("Services")]
        public string Services { get; set; }

        [JsonProperty("Currencies")]
        public string Currencies { get; set; }

        [JsonProperty(".issued")]
        public string Issued { get; set; }

        [JsonProperty(".expires")]
        public string Expires { get; set; }
    }
}
