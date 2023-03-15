using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Blend.Controllers
{
    [Route("User")]
    public class UserController:ApiController
    {
        IUserService _adProcessor;
        public UserController(IUserService ADProcessor)
        {
            _adProcessor = ADProcessor;
        }

        [HttpPost]
        public async Task<IHttpActionResult> ValidateUser([FromBody]ValidateADUserRequest validateUserRequest)
        {
            ValidateADUserResponse respo = await _adProcessor.ValidateUser(validateUserRequest);
            return Ok(respo);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetUserDetails([FromBody]GetADUserRequest getADUserRequest)
        {
            GetADUserResponse respo = await _adProcessor.GetUserDetails(getADUserRequest);
            return Ok(respo);
        }
    }
}
