using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class FlightSearchRequest
    {
        [JsonProperty("FlightSearchType")]
        public string FlightSearchType { get; set; }

        [JsonProperty("Ticketclass")]
        public string Ticketclass { get; set; }

        [JsonProperty("Adults")]
        public string Adults { get; set; }

        [JsonProperty("Children")]
        public string Children { get; set; }

        [JsonProperty("Infants")]
        public string Infants { get; set; }

        [JsonProperty("Itineraries")]
        public Itinerary[] Itineraries { get; set; }

        [JsonProperty("TargetCurrency")]
        public string TargetCurrency { get; set; }
    }

    public partial class Itinerary
    {
        [JsonProperty("Departure")]
        public string Departure { get; set; }

        [JsonProperty("Destination")]
        public string Destination { get; set; }

        [JsonProperty("DepartureDate")]
        public string DepartureDate { get; set; }
    }
}
