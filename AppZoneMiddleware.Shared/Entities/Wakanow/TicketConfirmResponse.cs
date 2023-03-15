using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class TicketConfirmResponse
    {
        [JsonProperty("BookingId")]
        public string BookingId { get; set; }

        [JsonProperty("CustomerId")]
        public Guid CustomerId { get; set; }

        [JsonProperty("ProductType")]
        public string ProductType { get; set; }

        [JsonProperty("FlightBookingSummary")]
        public FlightBookingSummary FlightBookingSummary { get; set; }

        [JsonProperty("ProductTermsAndConditions")]
        public object ProductTermsAndConditions { get; set; }
    }

    public partial class FlightBookingSummary
    {
        [JsonProperty("PnrReferenceNumber")]
        public string PnrReferenceNumber { get; set; }

        [JsonProperty("PnrDate")]
        public DateTimeOffset PnrDate { get; set; }

        [JsonProperty("FlightSummaryModel")]
        public FlightSummaryModel FlightSummaryModel { get; set; }

        [JsonProperty("TravellerDetails")]
        public TravellerDetail[] TravellerDetails { get; set; }

        [JsonProperty("PnrStatus")]
        public string PnrStatus { get; set; }

        [JsonProperty("TicketStatus")]
        public string TicketStatus { get; set; }
    }
}
