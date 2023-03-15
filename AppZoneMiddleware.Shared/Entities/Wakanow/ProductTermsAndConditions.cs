using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class ProductTermsAndConditions
    {
        [JsonProperty("TermsAndConditions")]
        public string[] TermsAndConditions { get; set; }

        [JsonProperty("TermsAndConditionImportantNotice")]
        public string TermsAndConditionImportantNotice { get; set; }
    }
}
