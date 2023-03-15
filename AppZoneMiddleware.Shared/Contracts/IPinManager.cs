using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IPinManager
    {
        Task<UserProfileResponse> ChangePin(ChangePinRequest changePinRequest);

        Task<UserProfileResponse> ForgotPin(UserAccount ForgotPinRequest);

        Task<UserProfileResponse> ResetPin(ResetPinRequest ResetPinRequest);

    }
}
