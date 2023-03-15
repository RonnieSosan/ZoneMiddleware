using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    /// <summary>
    /// Encapsulates data needed to invoke the RegisterUser REST API.
    /// </summary>
    [JsonObject]
    [Serializable]
    public class MPassRegisterUserRequestJSON : sPayBaseRequest
    {
        public string BVN { get; set; }
        public string NUBAN { get; set; }
        public string BusinessName { get; set; }
        public string MccCode { get; set; }
        public string Password { get; set; }
    }
}
