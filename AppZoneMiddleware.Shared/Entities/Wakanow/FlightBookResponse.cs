using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class FlightBookResponse
    {
        [JsonProperty("BookingId")]
        public string BookingId { get; set; }

        [JsonProperty("CustomerId")]
        public Guid CustomerId { get; set; }

        [JsonProperty("ProductType")]
        public string ProductType { get; set; }

        [JsonProperty("TargetCurrency")]
        public string TargetCurrency { get; set; }

        [JsonProperty("ProductTermsAndConditions")]
        public object ProductTermsAndConditions { get; set; }

        [JsonProperty("FlightBookingResult")]
        public FlightBookingResult FlightBookingResult { get; set; }
    }

    public partial class FlightBookingResult
    {
        [JsonProperty("FlightBookingSummaryModel")]
        public FlightBookingSummaryModel FlightBookingSummaryModel { get; set; }

        [JsonProperty("IsFareLow")]
        public bool IsFareLow { get; set; }

        [JsonProperty("IsFareHigh")]
        public bool IsFareHigh { get; set; }

        [JsonProperty("HasResult")]
        public bool HasResult { get; set; }
    }

    public partial class FlightBookingSummaryModel
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
        public object PnrStatus { get; set; }

        [JsonProperty("TicketStatus")]
        public object TicketStatus { get; set; }
    }

    public partial class TravellerDetail
    {
        [JsonProperty("PassengerType")]
        public string PassengerType { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("MiddleName")]
        public string MiddleName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("DateOfBirth")]
        public DateTimeOffset DateOfBirth { get; set; }

        [JsonProperty("Age")]
        public object Age { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("PassportNumber")]
        public string PassportNumber { get; set; }

        [JsonProperty("ExpiryDate")]
        public object ExpiryDate { get; set; }

        [JsonProperty("PassportIssuingAuthority")]
        public string PassportIssuingAuthority { get; set; }

        [JsonProperty("Gender")]
        public string Gender { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("PostalCode")]
        public long PostalCode { get; set; }

        [JsonProperty("TicketNumber")]
        public object TicketNumber { get; set; }

        [JsonProperty("RoomNumber")]
        public object RoomNumber { get; set; }
    }
}
