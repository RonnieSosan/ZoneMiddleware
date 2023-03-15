using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace MiddlewareApiProxy.Controllers
{
    [RoutePrefix("api/ActiveDirectory")]
    public class ActiveDirectoryController : ApiController
    {
        IUserService _ActiveDirectoryService;

        public ActiveDirectoryController(IUserService IActiveDirectoryService)
        {
            _ActiveDirectoryService = IActiveDirectoryService;
        }

        [HttpPost]
        [Route("ValidateUser")]
        public async Task<IHttpActionResult> ValidateUser(ValidateADUserRequest ValidateUserRequest)
        {
            ValidateADUserResponse response = await _ActiveDirectoryService.ValidateUser(ValidateUserRequest);
            return Ok(response);
        }

        [HttpPost]
        [Route("GetUserDetails")]
        public async Task<IHttpActionResult> GetUserDetails(GetADUserRequest GetUserDetalisRequest)
        {
            GetADUserResponse response = await _ActiveDirectoryService.GetUserDetails(GetUserDetalisRequest);
            return Ok(response);
        }
        
    }
}
