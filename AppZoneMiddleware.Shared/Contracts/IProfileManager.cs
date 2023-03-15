using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IProfileManager
    {
        Task<UserProfileResponse> InitiateProfileRegistration(UserAccount profileToRegister);

        Task<UserProfileResponse> RegisterProfile(string profileToRegister);

        Task<UserLoginResponse> UserLogin(UserLoginRequest LoginRequest);

        Task<UserProfileResponse> LockProfile(string CustomerID);

        Task<UserProfileResponse> UnlockProfile(string CustomerID);

        Task<LoggedInUsersResponse> GetCurrentLoggedInUsers();

        Task<UserProfileResponse> ValidateProfile(UserProfile validateProfileRequest);

        Task<JObject> ProxyLogin(ApiProxyRequest loginRequest);
    }
}
