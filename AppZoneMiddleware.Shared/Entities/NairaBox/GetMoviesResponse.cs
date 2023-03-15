using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public class GetMoviesResponse : BaseResponse
    {
        [JsonProperty("movies")]
        public Movie[] Movies { get; set; }
    }

    public partial class Movie
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("duration")]
        public string Duration { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("youtubeid")]
        public string Youtubeid { get; set; }

        [JsonProperty("youtubeurl")]
        public Uri Youtubeurl { get; set; }

        [JsonProperty("artwork")]
        public string Artwork { get; set; }

        [JsonProperty("artworkThumbnail")]
        public Uri ArtworkThumbnail { get; set; }

        [JsonProperty("cinemas")]
        public Cinema[] Cinemas { get; set; }
    }
}
