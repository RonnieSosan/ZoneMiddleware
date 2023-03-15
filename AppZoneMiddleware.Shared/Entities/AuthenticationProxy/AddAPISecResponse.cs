using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.AuthenticationProxy
{
    public class AddAPISecResponse : BaseResponse
    {
        public ApiSecuritySpec ApiSecurity { get; set; }
    }
}
