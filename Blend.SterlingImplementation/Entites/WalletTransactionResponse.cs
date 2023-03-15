using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    [JsonObject]
    public class WalletTransactionResponse : sPayBaseResponse
    {
        public string message { get; set; }
        public string response { get; set; }
        public WalletTransactionResponseData data { get; set; }
    }

    [JsonObject]
    public class WalletTransactionResponseData
    {
        public string status { get; set; }
    }

}
