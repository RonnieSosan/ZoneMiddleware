using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Utility;
using Blend.DefaultImplementation.Persistence;
using MiddlewareApiProxy.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MiddlewareApiProxy.Controllers
{
    [RoutePrefix("api/ProfileManager")]
    public class ProfileController : ApiController
    {
        IProfileManager _profileManager;

        public ProfileController(IProfileManager profileManager)
        {
            _profileManager = profileManager;
        }

        [HttpPost]
        [Route("InitiateProfileRegistration")]
        public async Task<IHttpActionResult> InitiateProfileRegistration([FromBody]UserAccount ProfileToRegister)
        {
            UserProfileResponse respo = await _profileManager.InitiateProfileRegistration(ProfileToRegister);
            return Ok(respo);
        }

        [HttpPost]
        [Route("RegisterProfile")]
        public async Task<IHttpActionResult> RegisterProfile([FromBody]UserAccount ProfileToRegister)
        {
            string profile = Newtonsoft.Json.JsonConvert.SerializeObject(ProfileToRegister);
            UserProfileResponse respo = await _profileManager.RegisterProfile(profile);
            return Ok(respo);
        }

        [HttpGet]
        [Route("GetCurrentlyLoggedInUsers")]
        public async Task<IHttpActionResult> GetCurrentlyLoggedInUsers()
        {
            LoggedInUsersResponse respo = await _profileManager.GetCurrentLoggedInUsers();
            return Ok(respo);
        }

        [HttpPost]
        [Route("UserLogin")]
        public async Task<IHttpActionResult> UserLogin([FromBody]UserLoginRequest request)
        {
            UserLoginResponse respo = await _profileManager.UserLogin(request);
            return Ok(respo);
        }

        [HttpPost]
        [Route("LockProfile")]
        public async Task<IHttpActionResult> LockProfile([FromBody]string CustomerID)
        {
            UserProfileResponse respo = await _profileManager.LockProfile(CustomerID);
            return Ok(respo);
        }

        [HttpPost]
        [Route("UnlockProfile")]
        public async Task<IHttpActionResult> UnlockProfile([FromBody]string CustomerID)
        {
            UserProfileResponse respo = await _profileManager.UnlockProfile(CustomerID);
            return Ok(respo);
        }

        [HttpPost]
        [Route("ProxyLogin")]
        public async Task<IHttpActionResult> ProxyLogin([FromBody]ApiProxyRequest request)
        {
            Logger.LogInfo("ProfileManager.ProxyLogin: request", request);

            JObject retVal = await _profileManager.ProxyLogin(request);
            return Ok(retVal);
        }
    }
}
