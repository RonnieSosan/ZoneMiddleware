using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public class GetMovieDetailsResponse: BaseResponse
    {
        public Movie Movie { get; set; }
    }
}
