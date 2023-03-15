using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IUserService
    {
        Task<ValidateADUserResponse> ValidateUser(ValidateADUserRequest input);

        Task<GetADUserResponse> GetUserDetails(GetADUserRequest input);
    }
}
