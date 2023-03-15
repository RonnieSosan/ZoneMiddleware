using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class AriiyaOrderResponse : BaseResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("parent_id")]
        public long ParentId { get; set; }

        [JsonProperty("number")]
        public string Number { get; set; }

        [JsonProperty("order_key")]
        public string OrderKey { get; set; }

        [JsonProperty("created_via")]
        public string CreatedVia { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("date_created")]
        public DateTimeOffset DateCreated { get; set; }

        [JsonProperty("date_created_gmt")]
        public DateTimeOffset DateCreatedGmt { get; set; }

        [JsonProperty("date_modified")]
        public DateTimeOffset DateModified { get; set; }

        [JsonProperty("date_modified_gmt")]
        public DateTimeOffset DateModifiedGmt { get; set; }

        [JsonProperty("discount_total")]
        public string DiscountTotal { get; set; }

        [JsonProperty("discount_tax")]
        public string DiscountTax { get; set; }

        [JsonProperty("shipping_total")]
        public string ShippingTotal { get; set; }

        [JsonProperty("shipping_tax")]
        public string ShippingTax { get; set; }

        [JsonProperty("cart_tax")]
        public string CartTax { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("total_tax")]
        public string TotalTax { get; set; }

        [JsonProperty("prices_include_tax")]
        public bool PricesIncludeTax { get; set; }

        [JsonProperty("customer_id")]
        public long CustomerId { get; set; }

        [JsonProperty("customer_ip_address")]
        public string CustomerIpAddress { get; set; }

        [JsonProperty("customer_user_agent")]
        public string CustomerUserAgent { get; set; }

        [JsonProperty("customer_note")]
        public string CustomerNote { get; set; }

        [JsonProperty("billing")]
        public Ing Billing { get; set; }

        [JsonProperty("shipping")]
        public Ing Shipping { get; set; }

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("payment_method_title")]
        public string PaymentMethodTitle { get; set; }

        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("date_paid")]
        public DateTimeOffset DatePaid { get; set; }

        [JsonProperty("date_paid_gmt")]
        public DateTimeOffset DatePaidGmt { get; set; }

        [JsonProperty("date_completed")]
        public DateTimeOffset DateCompleted { get; set; }

        [JsonProperty("date_completed_gmt")]
        public DateTimeOffset DateCompletedGmt { get; set; }

        [JsonProperty("cart_hash")]
        public string CartHash { get; set; }

        [JsonProperty("meta_data")]
        public MetaDatum[] MetaData { get; set; }

        [JsonProperty("line_items")]
        public LineItem[] LineItems { get; set; }

        [JsonProperty("tax_lines")]
        public object[] TaxLines { get; set; }

        [JsonProperty("shipping_lines")]
        public object[] ShippingLines { get; set; }

        [JsonProperty("fee_lines")]
        public object[] FeeLines { get; set; }

        [JsonProperty("coupon_lines")]
        public object[] CouponLines { get; set; }

        [JsonProperty("refunds")]
        public object[] Refunds { get; set; }

        [JsonProperty("ticket_info")]
        public TicketInfo[] TicketInfo { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }

    public partial class Ing
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("address_1")]
        public string Address1 { get; set; }

        [JsonProperty("address_2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("postcode")]
        public string Postcode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("phone", NullValueHandling = NullValueHandling.Ignore)]
        public string Phone { get; set; }
    }

    public partial class LineItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("product_id")]
        public long productId { get; set; }

        [JsonProperty("variation_id")]
        public long VariationId { get; set; }

        [JsonProperty("quantity")]
        public long quantity { get; set; }

        [JsonProperty("tax_class")]
        public string TaxClass { get; set; }

        [JsonProperty("subtotal")]
        public string Subtotal { get; set; }

        [JsonProperty("subtotal_tax")]
        public string SubtotalTax { get; set; }

        [JsonProperty("total")]
        public string Total { get; set; }

        [JsonProperty("total_tax")]
        public string TotalTax { get; set; }

        [JsonProperty("taxes")]
        public object[] Taxes { get; set; }

        [JsonProperty("meta_data")]
        public MetaDatum[] MetaData { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }
    }

    public partial class MetaDatum
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class Links
    {
        [JsonProperty("self")]
        public Collection[] Self { get; set; }

        [JsonProperty("collection")]
        public Collection[] Collection { get; set; }
    }

    public partial class Collection
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }
    }

    public partial class TicketInfo
    {
        [JsonProperty("event_id")]
        public string EventId { get; set; }

        [JsonProperty("ticket_id")]
        public long TicketId { get; set; }

        [JsonProperty("security_code")]
        public string SecurityCode { get; set; }

        [JsonProperty("ticket_no")]
        public string TicketNo { get; set; }

        [JsonProperty("ticket_type")]
        public string TicketType { get; set; }
    }
}
