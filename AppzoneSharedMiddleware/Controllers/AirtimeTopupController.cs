using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AppzoneSharedMiddleware.Controllers
{
    [RoutePrefix("api/AirtimeTopUp")]
    public class AirtimeTopupController : ApiController
    {

        IAirtimeTopup _topUpService;

        public AirtimeTopupController(IAirtimeTopup TopUpService)
        {
            _topUpService = TopUpService;
        }

        [HttpPost]
        [Route("ProcessRecharge")]
        public async Task<IHttpActionResult> ProcessRecharge(AirtimeRequest Request)
        {
            AirtimeResponse response = await _topUpService.DoAirtimeTopUp(Request);
            return Ok(response);
        }
    }
}
