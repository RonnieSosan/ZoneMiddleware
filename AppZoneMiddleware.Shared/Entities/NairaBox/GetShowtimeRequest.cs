using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public class GetShowtimeRequest
    {
        public string CinemaId { get; set; }
        public string TicketId { get; set; }
    }
}
