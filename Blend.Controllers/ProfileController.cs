using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using Blend.SterlingImplementation.ProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Blend.Controllers
{
    [Route("Profile")]
    public class ProfileController : ApiController
    {
        IProfileManager _profileManager;

        public ProfileController(IProfileManager profileManager)
        {
            _profileManager = profileManager;
        }

        [HttpPost]
        public async Task<IHttpActionResult> InitiateProfileRegistration([FromBody]UserAccount ProfileToRegister)
        {
            UserProfileResponse respo = await _profileManager.InitiateProfileRegistration(ProfileToRegister);
            return Ok(respo);
        }

        [HttpPost]
        public async Task<IHttpActionResult> RegisterProfile([FromBody]UserAccount ProfileToRegister)
        {
            string profile = Newtonsoft.Json.JsonConvert.SerializeObject(ProfileToRegister);
            UserProfileResponse respo = await _profileManager.RegisterProfile(profile);
            return Ok(respo);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ValidateOTP([FromBody]ValidateOTP request)
        {
            string profile = Newtonsoft.Json.JsonConvert.SerializeObject(request);

            UserProfileResponse respo = await new ProfileManager().ValidateOTP(request);
            return Ok(respo);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetCurrentlyLoggedInUsers()
        {
            LoggedInUsersResponse respo = await _profileManager.GetCurrentLoggedInUsers();
            return Ok(respo);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UserLogin([FromBody]UserLoginRequest request)
        {
            UserLoginResponse respo = await _profileManager.UserLogin(request);
            return Ok(respo);
        }

        [HttpPost]
        public async Task<IHttpActionResult> LockProfile([FromBody]string CustomerID)
        {
            UserProfileResponse respo = await _profileManager.LockProfile(CustomerID);
            return Ok(respo);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UnlockProfile([FromBody]string CustomerID)
        {
            UserProfileResponse respo = await _profileManager.UnlockProfile(CustomerID);
            return Ok(respo);
        }
    }
}
