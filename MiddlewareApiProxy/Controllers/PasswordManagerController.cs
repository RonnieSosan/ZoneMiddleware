using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MiddlewareApiProxy.Controllers
{
    [RoutePrefix("api/PasswordManager")]
    public class PasswordManagerController : ApiController
    {
        IPasswordManager _passwordManager;

        public PasswordManagerController(IPasswordManager PasswordManager)
        {
            _passwordManager = PasswordManager;
        }

        [HttpPost]
        [Route("ChangePasword")]
        public async Task<IHttpActionResult> ChangePasword([FromBody]ChangePasswordRequest passwordRequest)
        {
            UserProfileResponse respo = await _passwordManager.ChangePassword(passwordRequest);
            return Ok(respo);
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword([FromBody]UserAccount forgotPasswordRequest)
        {
            UserProfileResponse respo = await _passwordManager.ForgotPassword(forgotPasswordRequest);
            return Ok(respo);
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword([FromBody]ResetPasswordRequest resetPasswordRequest)
        {
            UserProfileResponse respo = await _passwordManager.ResetPassword(resetPasswordRequest);
            return Ok(respo);
        }
    }
}
