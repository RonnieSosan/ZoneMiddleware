using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public partial class GetByCinema
    {
        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logo")]
        public Uri Logo { get; set; }

        [JsonProperty("logoThumbnail")]
        public Uri LogoThumbnail { get; set; }

        [JsonProperty("tickets")]
        public Ticket[] Tickets { get; set; }
    }

    [JsonObject("Cinema")]
    public partial class Cinema
    {
        [JsonProperty("_id")]
        public string _id { get; set; }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public string location { get; set; }

        [JsonProperty("state")]
        public string state { get; set; }
    }

    public partial class Ticket
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("youtubeId")]
        public string YoutubeId { get; set; }

        [JsonProperty("youtubeUrl")]
        public Uri YoutubeUrl { get; set; }

        [JsonProperty("artwork")]
        public Uri Artwork { get; set; }

        [JsonProperty("artworkThumbnail")]
        public Uri ArtworkThumbnail { get; set; }
    }
}
