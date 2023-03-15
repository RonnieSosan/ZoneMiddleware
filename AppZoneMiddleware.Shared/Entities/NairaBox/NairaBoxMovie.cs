using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public partial class NairaBoxMovie
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("vid")]
        public string Vid { get; set; }

        [JsonProperty("director")]
        public string Director { get; set; }

        [JsonProperty("starring")]
        public string Starring { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("available")]
        public string Available { get; set; }

        [JsonProperty("artwork")]
        public string Artwork { get; set; }

        [JsonProperty("purchaseCount")]
        public string PurchaseCount { get; set; }

        [JsonProperty("featured")]
        public string Featured { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("__v")]
        public long V { get; set; }
    }
}
