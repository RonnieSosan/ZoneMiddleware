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
    [RoutePrefix("api/TokenManager")]
    public class TokenController : ApiController
    {
        ITokenManager _tokenManager;

        public TokenController(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }

        [HttpPost]
        [Route("TokenAuthentication")]
        public async Task<IHttpActionResult> TokenAuthentication([FromBody]BaseRequest request)
        {
            UserProfileResponse respo = await _tokenManager.NonTransactionalTokenAuthentication(request);
            return Ok(respo);
        }


        [HttpPost]
        [Route("TransactionalTokenAuthentication")]
        public async Task<IHttpActionResult> TransactionalTokenAuthentication([FromBody]BaseRequest request)
        {
            UserProfileResponse respo = await _tokenManager.TransactionalTokenAuthentication(request);
            return Ok(respo);
        }
    }
}
