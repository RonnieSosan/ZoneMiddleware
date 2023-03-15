using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class SelectFlightResponse
    {
        [JsonProperty("FlightSummaryModel")]
        public FlightSummaryModel FlightSummaryModel { get; set; }

        [JsonProperty("IsPriceMatched")]
        public bool IsPriceMatched { get; set; }

        [JsonProperty("HasResult")]
        public bool HasResult { get; set; }

        [JsonProperty("SelectData")]
        public string SelectData { get; set; }

        [JsonProperty("ProductTermsAndConditions")]
        public ProductTermsAndConditions ProductTermsAndConditions { get; set; }

        [JsonProperty("TermsAndConditions")]
        public object TermsAndConditions { get; set; }

        [JsonProperty("BookingId")]
        public string BookingId { get; set; }
    }

    public partial class FlightSummaryModel
    {
        [JsonProperty("FlightCombination")]
        public FlightCombination FlightCombination { get; set; }
    }
}
