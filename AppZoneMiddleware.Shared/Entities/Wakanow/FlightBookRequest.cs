using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class FlightBookRequest
    {
        [JsonProperty("PassengerDetails")]
        public PassengerDetail[] PassengerDetails { get; set; }

        [JsonProperty("BookingItemModels")]
        public BookingItemModel[] BookingItemModels { get; set; }

        [JsonProperty("BookingId")]
        public string BookingId { get; set; }
    }

    public partial class BookingItemModel
    {
        [JsonProperty("ProductType")]
        public string ProductType { get; set; }

        [JsonProperty("BookingData")]
        public string BookingData { get; set; }

        [JsonProperty("BookingId")]
        public string BookingId { get; set; }

        [JsonProperty("TargetCurrency")]
        public string TargetCurrency { get; set; }
    }

    public partial class PassengerDetail
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
        public string DateOfBirth { get; set; }

        [JsonProperty("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty("Address")]
        public string Address { get; set; }

        [JsonProperty("PassportNumber")]
        public string PassportNumber { get; set; }

        [JsonProperty("ExpiryDate")]
        public string ExpiryDate { get; set; }

        [JsonProperty("PassportIssuingAuthority")]
        public string PassportIssuingAuthority { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Gender")]
        public string Gender { get; set; }

        [JsonProperty("Title")]
        public string Title { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("Country")]
        public string Country { get; set; }

        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("PostalCode")]
        public string PostalCode { get; set; }
    }
}
