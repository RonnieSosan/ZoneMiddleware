using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    [Serializable]
    public class MPassRegisterUserRequest : BaseRequest
    {
        public string Referenceid { get; set; }
        public string RequestType { get; set; }
        public string Translocation { get; set; }
        public string BVN { get; set; }
        public string NUBAN { get; set; }
        public string BusinessName { get; set; }
        public string MccCode { get; set; }
        public string Password { get; set; }
    }
}
