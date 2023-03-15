using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class FlightSearchResponse
    {
        [JsonProperty("FlightCombination")]
        public FlightCombination FlightCombination { get; set; }

        [JsonProperty("SelectData")]
        public string SelectData { get; set; }
    }

    public class SearchResult : BaseResponse
    {
        public FlightSearchResponse[] flightSearchResult { get; set; }
    }
}
