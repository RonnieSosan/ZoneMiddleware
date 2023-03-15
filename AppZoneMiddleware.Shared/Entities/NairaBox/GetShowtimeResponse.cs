using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public class GetShowtimeResponse : BaseResponse
    {
        public Showtime[] Showtimes { get; set; }
    }
}
