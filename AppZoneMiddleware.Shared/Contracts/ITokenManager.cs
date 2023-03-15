using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface ITokenManager
    {
        string ExtendToken(string CustomerID);

        Task<UserProfileResponse> NonTransactionalTokenAuthentication(BaseRequest TokenValidationRequest);

        Task<UserProfileResponse> TransactionalTokenAuthentication(BaseRequest TokenValidationRequest);
    }
}
