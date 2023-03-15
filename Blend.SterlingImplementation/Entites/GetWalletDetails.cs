using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    [JsonObject]
    public class GetWalletDetails : sPayBaseRequest
    {
        public string nuban { get; set; }
    }
}
