using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts.ProxyAuthentication
{
    public interface IProxyRequestAuthentication
    {
        Task<ApiProxyResponse> RunAuthentication(ApiProxyRequest proxyRequest);
    }
}
