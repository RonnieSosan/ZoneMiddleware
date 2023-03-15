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
    [RoutePrefix("api/PinManager")]
    public class PinManagerController: ApiController
    {
        IPinManager _pinManager;

        public PinManagerController(IPinManager PinManager)
        {
            _pinManager = PinManager;
        }

        [HttpPost]
        [Route("ChangePin")]
        public async Task<IHttpActionResult> ChangePin([FromBody]ChangePinRequest changePinRequest)
        {
            UserProfileResponse respo = await _pinManager.ChangePin(changePinRequest);
            return Ok(respo);
        }

        [HttpPost]
        [Route("ForgotPin")]
        public async Task<IHttpActionResult> ForgotPin([FromBody]UserAccount changePinRequest)
        {
            UserProfileResponse respo = await _pinManager.ForgotPin(changePinRequest);
            return Ok(respo);
        }

        [HttpPost]
        [Route("ResetPin")]
        public async Task<IHttpActionResult> ResetPin([FromBody]ResetPinRequest changePinRequest)
        {
            UserProfileResponse respo = await _pinManager.ResetPin(changePinRequest);
            return Ok(respo);
        }

    }
}
