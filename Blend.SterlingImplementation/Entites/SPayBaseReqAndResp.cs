using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    [JsonObject]
    public class sPayBaseRequest
    {
        public string RequestType { get; set; }
        public string Referenceid { get; set; }
        public string Translocation { get; set; }
    }

    [JsonObject]
    public class sPayBaseResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
    }
}
