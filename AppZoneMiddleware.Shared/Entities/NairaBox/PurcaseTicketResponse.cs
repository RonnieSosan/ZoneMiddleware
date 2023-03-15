using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public class PurcaseTicketResponse : BaseResponse
    {
        public string ticketID { get; set; }

        [JsonProperty("Transaction reference")]
        public string TransactionReference { get; set; }
    }
}
