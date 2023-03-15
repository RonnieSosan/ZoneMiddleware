using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class Flight
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Airline")]
        public string Airline { get; set; }

        [JsonProperty("AirlineName")]
        public string AirlineName { get; set; }

        [JsonProperty("DepartureCode")]
        public string DepartureCode { get; set; }

        [JsonProperty("DepartureName")]
        public string DepartureName { get; set; }

        [JsonProperty("DepartureTime")]
        public DateTimeOffset DepartureTime { get; set; }

        [JsonProperty("ArrivalName")]
        public string ArrivalName { get; set; }

        [JsonProperty("ArrivalCode")]
        public string ArrivalCode { get; set; }

        [JsonProperty("ArrivalTime")]
        public DateTimeOffset ArrivalTime { get; set; }

        [JsonProperty("Stops")]
        public long Stops { get; set; }

        [JsonProperty("StopTime")]
        public string StopTime { get; set; }

        [JsonProperty("TripDuration")]
        public string TripDuration { get; set; }

        [JsonProperty("StopCity")]
        public string StopCity { get; set; }

        [JsonProperty("AirlineLogoUrl")]
        public Uri AirlineLogoUrl { get; set; }

        [JsonProperty("FlightLegs")]
        public FlightLeg[] FlightLegs { get; set; }
    }

    public partial class FlightLeg
    {
        [JsonProperty("DepartureCode")]
        public string DepartureCode { get; set; }

        [JsonProperty("DepartureName")]
        public string DepartureName { get; set; }

        [JsonProperty("DestinationCode")]
        public string DestinationCode { get; set; }

        [JsonProperty("DestinationName")]
        public string DestinationName { get; set; }

        [JsonProperty("StartTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("EndTime")]
        public DateTimeOffset EndTime { get; set; }

        [JsonProperty("Duration")]
        public string Duration { get; set; }

        [JsonProperty("IsStop")]
        public bool IsStop { get; set; }

        [JsonProperty("Layover")]
        public string Layover { get; set; }

        [JsonProperty("LayoverDuration")]
        public DateTimeOffset LayoverDuration { get; set; }

        [JsonProperty("BookingClass")]
        public string BookingClass { get; set; }

        [JsonProperty("CabinClass")]
        public string CabinClass { get; set; }

        [JsonProperty("CabinClassName")]
        public string CabinClassName { get; set; }

        [JsonProperty("OperatingCarrier")]
        public string OperatingCarrier { get; set; }

        [JsonProperty("OperatingCarrierName")]
        public string OperatingCarrierName { get; set; }

        [JsonProperty("MarketingCarrier")]
        public string MarketingCarrier { get; set; }

        [JsonProperty("FlightNumber")]
        public string FlightNumber { get; set; }

        [JsonProperty("Aircraft")]
        public string Aircraft { get; set; }

        [JsonProperty("FareType")]
        public string FareType { get; set; }

        [JsonProperty("FarebasisCode")]
        public string FarebasisCode { get; set; }

        [JsonProperty("CorporateCode")]
        public string CorporateCode { get; set; }

        [JsonProperty("FlightSelectData")]
        public object FlightSelectData { get; set; }
    }
}
