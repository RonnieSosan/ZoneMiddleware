using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    #region NairaBoxQueries API Data Models
    public class NairaBoxQueriesRequest
    {
        public string Referenceid { get; set; }
        public string RequestType { get; set; }
        public string Translocation { get; set; }
        /// <summary>
        /// The type of data to query for. Either "movies" or "cinemas".
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// The ID of the movie. Used to query for movie details
        /// </summary>
        public string MovieID { get; set; }
        /// <summary>
        /// The ID of the cinema. Used to query for showtime details
        /// </summary>
        public string cinema_uid { get; set; }
        /// <summary>
        /// The ID of the movie ticket. Used to query for showtime details
        /// </summary>
        public string ticketId { get; set; }
    }

    public class NairaBoxQueriesResponse
    {
        public string message { get; set; }
        public string response { get; set; }
        public NairaBoxQueriesResponseDetails responsedata { get; set; }
        public string data { get; set; }
    }

    public class NairaBoxQueriesResponseDetails
    {
        public string status { get; set; }
        public string message { get; set; }
        public NairaBoxMovie movie { get; set; }
        public NairaBoxMovie[] movies { get; set; }
        public NairaBoxCinema cinema { get; set; }
        public NairaBoxCinema[] cinemas { get; set; }
        public NairaBoxShowtime showtime { get; set; }
        public NairaBoxShowtime[] showtimes { get; set; }
        public NairaBoxEvent[] events { get; set; }
    }

    public class NairaBoxMovie
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string genre { get; set; }
        public string duration { get; set; }
        public string director { get; set; }
        public string starring { get; set; }
        public string youtubeid { get; set; }
        public string youtubeurl { get; set; }
        public string artwork { get; set; }
        public string artworkThumbnail { get; set; }
        public NairaBoxCinema[] cinemas { get; set; }
    }

    public class NairaBoxCinema
    {
        public string uid { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public string logoThumbnail { get; set; }
        public NairaBoxTicket[] tickets { get; set; }
    }

    public class NairaBoxTicket
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string genre { get; set; }
        public string duration { get; set; }
        public string youtubeid { get; set; }
        public string youtubeurl { get; set; }
        public string artwork { get; set; }
        public string artworkThumbnail { get; set; }
    }
    
    public class NairaBoxShowtime
    {
        public string id { get; set; }
        public string day { get; set; }
        public string date { get; set; }
        public int adult { get; set; }
        public int student { get; set; }
        public int children { get; set; }
    }

    public class NairaBoxEvent
    {
        public string id { get; set; }
        public string organizer { get; set; }
        public string organizerLogo { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string date { get; set; }
        public string eventArtwork { get; set; }
        public string eventBanner { get; set; }
        public NairaBoxEventTicketclasss[] ticketClassses { get; set; }
    }

    public class NairaBoxEventTicketclasss
    {
        public string classid { get; set; }
        public string price { get; set; }
        public string title { get; set; }
        public string venue { get; set; }
    }
    #endregion
}
