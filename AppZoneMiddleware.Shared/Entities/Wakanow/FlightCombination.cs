using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class FlightCombination
    {
        [JsonProperty("FlightModels")]
        public Flight[] Flights { get; set; }

        [JsonProperty("Price")]
        public Price Price { get; set; }

        [JsonProperty("MarketingCarrier")]
        public string MarketingCarrier { get; set; }

        [JsonProperty("Adults")]
        public long Adults { get; set; }

        [JsonProperty("Children")]
        public long Children { get; set; }

        [JsonProperty("Infants")]
        public long Infants { get; set; }

        [JsonProperty("PriceDetails")]
        public PriceDetail[] PriceDetails { get; set; }

        [JsonProperty("FareRules")]
        public string[] FareRules { get; set; }

        [JsonProperty("PenaltyRules")]
        public object PenaltyRules { get; set; }

        [JsonProperty("AirlineCode")]
        public object AirlineCode { get; set; }

        [JsonProperty("IsRefundable")]
        public bool IsRefundable { get; set; }
    }


    public partial class Price
    {
        [JsonProperty("Amount")]
        public double Amount { get; set; }

        [JsonProperty("CurrencyCode")]
        public string CurrencyCode { get; set; }
    }

    public partial class PriceDetail
    {
        [JsonProperty("BaseFare")]
        public Price BaseFare { get; set; }

        [JsonProperty("Tax")]
        public Price Tax { get; set; }

        [JsonProperty("PassengerType")]
        public string PassengerType { get; set; }
    }
}
