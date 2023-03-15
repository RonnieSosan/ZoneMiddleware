using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppzoneSharedMiddleware.Controllers
{
    [RoutePrefix("api/CardControl")]
    public class CardControlController : ApiController
    {
        IPinService _PinService;

        public CardControlController(IPinService PinService)
        {
            _PinService = PinService;
        }

        [HttpPost]
        [Route("PinRetrieval")]
        public IHttpActionResult PinRetrieval(PinRetrievalRequest paymentRequest)
        {
            PinRetrievalResponse response = _PinService.PinRetrieval(paymentRequest);
            return Ok(response);
        }

        [HttpPost]
        [Route("Test")]
        public IHttpActionResult Test()
        {
            return Ok();
        }
    }
}
