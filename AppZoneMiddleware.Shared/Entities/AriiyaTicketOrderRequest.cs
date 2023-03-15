using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class AriiyaTicketOrderRequest:BaseResponse
    {
        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("payment_method_title")]
        public string PaymentMethodTitle { get; set; }

        [JsonProperty("set_paid")]
        public bool SetPaid { get; set; }

        [JsonProperty("billing")]
        public Billing Billing { get; set; }

        [JsonProperty("line_items")]
        public LineItem[] LineItems { get; set; }
    }

    public partial class Billing
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }
    }

    public partial class LineItem
    {
        [JsonProperty("product_id")]
        public long ProductId { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }
    }
}
