using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public class WakanowAirport
    {
        [JsonProperty("AirportCode")]
        public string AirportCode { get; set; }

        [JsonProperty("AirportName")]
        public string AirportName { get; set; }

        [JsonProperty("CityCountry")]
        public string CityCountry { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }
    }
}
