using AppZoneMiddleware.Shared.Entities.Middleware.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IPayCode
    {
        Task<InterswitchTokenGenResponse> GenerateToken(InterswitchTokenGenRequest Request);

        Task<InterswitchTokenStatusResponse> CheckStatus(InterswitchTokenStatusRequest request);

        Task<InterswitchCancelTokenResponse> CancleToken(InterswitchCancelTokenRequest Request);
    }
}
