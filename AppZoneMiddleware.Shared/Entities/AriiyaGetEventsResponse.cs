using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class AriiyaGetEventsResponse : BaseResponse
    {
        public List<Event> events { get; set; }
        [JsonProperty("next_rest_url")]
        public Uri NextRestUrl { get; set; }
        public string rest_url { get; set; }
        public int total { get; set; }
        public int total_pages { get; set; }
    }

    public class Event
    {
        public int id { get; set; }
        public string global_id { get; set; }
        public string[] global_id_lineage { get; set; }
        public string author { get; set; }
        public string status { get; set; }
        public string date { get; set; }
        public string date_utc { get; set; }
        public string modified { get; set; }
        public string modified_utc { get; set; }
        public string url { get; set; }
        public string rest_url { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string excerpt { get; set; }
        public string slug { get; set; }
        public Image image { get; set; }
        public bool all_day { get; set; }
        public string start_date { get; set; }
        public Start_Date_Details start_date_details { get; set; }
        public string end_date { get; set; }
        public End_Date_Details end_date_details { get; set; }
        public string utc_start_date { get; set; }
        public Utc_Start_Date_Details utc_start_date_details { get; set; }
        public string utc_end_date { get; set; }
        public Utc_End_Date_Details utc_end_date_details { get; set; }
        public string timezone { get; set; }
        public string timezone_abbr { get; set; }
        public string cost { get; set; }
        public Cost_Details cost_details { get; set; }
        public string website { get; set; }
        public bool show_map { get; set; }
        public bool show_map_link { get; set; }
        public bool hide_from_listings { get; set; }
        public bool sticky { get; set; }
        public bool featured { get; set; }
        public Category[] categories { get; set; }
        public Tag[] tags { get; set; }
        public object venue { get; set; }
        public Organizer[] organizer { get; set; }
        public Ticket[] tickets { get; set; }
        public object ticketed { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
        public string id { get; set; }
        public string extension { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        [JsonProperty("sizes")]
        public Dictionary<string, Size> Sizes { get; set; }
    }

    public partial class Size
    {
        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("mime-type")]
        public string MimeType { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("uncropped", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Uncropped { get; set; }
    }

    public class Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Medium
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Medium_Large
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxPostsList
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxPostsGrid
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxPostsGrid2X
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxRelatedPostThumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxRelatedPostThumbnail2X
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxRecentThumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxEvents555X450
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxEvents360X240
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxEvents720X4802X
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Ticketbox555X300
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Ticketbox360X200
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Ticketbox720X4002X
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Woocommerce_Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public bool uncropped { get; set; }
        public string url { get; set; }
    }

    public class Woocommerce_Single
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Woocommerce_Gallery_Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Shop_Catalog
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Shop_Single
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Shop_Thumbnail
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxPostsList2X
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Ticketbox1110X6002X
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Large
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class TicketboxFeaturedImage
    {
        public int width { get; set; }
        public int height { get; set; }
        public string mimetype { get; set; }
        public string url { get; set; }
    }

    public class Start_Date_Details
    {
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string hour { get; set; }
        public string minutes { get; set; }
        public string seconds { get; set; }
    }

    public class End_Date_Details
    {
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string hour { get; set; }
        public string minutes { get; set; }
        public string seconds { get; set; }
    }

    public class Utc_Start_Date_Details
    {
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string hour { get; set; }
        public string minutes { get; set; }
        public string seconds { get; set; }
    }

    public class Utc_End_Date_Details
    {
        public string year { get; set; }
        public string month { get; set; }
        public string day { get; set; }
        public string hour { get; set; }
        public string minutes { get; set; }
        public string seconds { get; set; }
    }

    public class Cost_Details
    {
        public string currency_symbol { get; set; }
        public string currency_position { get; set; }
        public string[] values { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int term_group { get; set; }
        public int term_taxonomy_id { get; set; }
        public string taxonomy { get; set; }
        public string description { get; set; }
        public int parent { get; set; }
        public int count { get; set; }
        public string filter { get; set; }
        public int id { get; set; }
        public Urls urls { get; set; }
    }

    public class Urls
    {
        public string self { get; set; }
        public string collection { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int term_group { get; set; }
        public int term_taxonomy_id { get; set; }
        public string taxonomy { get; set; }
        public string description { get; set; }
        public int parent { get; set; }
        public int count { get; set; }
        public string filter { get; set; }
        public int id { get; set; }
        public Urls1 urls { get; set; }
    }

    public class Urls1
    {
        public string self { get; set; }
        public string collection { get; set; }
    }

    public class Organizer
    {
        public int id { get; set; }
        public string author { get; set; }
        public string status { get; set; }
        public string date { get; set; }
        public string date_utc { get; set; }
        public string modified { get; set; }
        public string modified_utc { get; set; }
        public string url { get; set; }
        public string organizer { get; set; }
        public string slug { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string global_id { get; set; }
        public string[] global_id_lineage { get; set; }
        public string website { get; set; }
    }

    public class Ticket
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool show_description { get; set; }
        public string price { get; set; }
        public int capacity { get; set; }
        public string regular_price { get; set; }
        public bool on_sale { get; set; }
        public string admin_link { get; set; }
        public string report_link { get; set; }
        public string frontend_link { get; set; }
        public string provider_class { get; set; }
        public string sku { get; set; }
        public object menu_order { get; set; }
        public string start_date { get; set; }
        public string start_time { get; set; }
        public string end_date { get; set; }
        public string end_time { get; set; }
        public object purchase_limit { get; set; }
    }

}