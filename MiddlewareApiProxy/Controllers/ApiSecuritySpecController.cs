using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MiddlewareApiProxy.Controllers
{
    [RoutePrefix("api/ApiSecuritySpec")]
    public class ApiSecuritySpecController : ApiController
    {
        IAPISecurityService _APISecurityService;

        public ApiSecuritySpecController(IAPISecurityService IAPISecurityService)
        {
            _APISecurityService = IAPISecurityService;
        }

        [HttpPost]
        [Route("AddAPI")]
        public IHttpActionResult AddAPI(ApiSecuritySpec AddRequest)
        {
            AddAPISecResponse response = _APISecurityService.AddAPI(AddRequest);
            return Ok(response);
        }

        [HttpPost]
        [Route("UpdateAPI")]
        public IHttpActionResult UpdateAPI(ApiSecuritySpec UpdateRequest)
        {
            AddAPISecResponse response = _APISecurityService.UpdateAPI(UpdateRequest);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetAPIs")]
        public IHttpActionResult GetAPIs()
        {
            List<ApiSecuritySpec> response = _APISecurityService.GetAPIs();
            return Ok(response);
        }
    }
}
