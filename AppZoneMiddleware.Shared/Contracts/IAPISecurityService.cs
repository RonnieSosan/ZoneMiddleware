using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IAPISecurityService
    {
        List<ApiSecuritySpec> GetAPIs();
        AddAPISecResponse AddAPI(ApiSecuritySpec AddRequest);
        AddAPISecResponse UpdateAPI(ApiSecuritySpec UpdateRequest);
    }
}
