using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.Wakanow
{
    public partial class TicketConfirmRequest
    {
        [JsonProperty("BookingId")]
        public string BookingId { get; set; }

        [JsonProperty("PnrNumber")]
        public string PnrNumber { get; set; }
    }
}
