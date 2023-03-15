using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    public class ExistingAccountHolderRequest : BaseRequest
    {
        public string BVN { get; set; }
    }
}
