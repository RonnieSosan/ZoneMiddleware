using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public partial class Event
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("organizer")]
        public string Organizer { get; set; }

        [JsonProperty("organizerLogo")]
        public string OrganizerLogo { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("date")]
        public DateTimeOffset Date { get; set; }

        [JsonProperty("eventArtwork")]
        public string EventArtwork { get; set; }

        [JsonProperty("eventBanner")]
        public string EventBanner { get; set; }

        [JsonProperty("ticketClassses")]
        public TicketClasss[] TicketClassses { get; set; }
    }

    public partial class TicketClasss
    {
        [JsonProperty("classid")]
        public string Classid { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("venue")]
        public string Venue { get; set; }
    }
}
