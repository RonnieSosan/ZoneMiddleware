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
    [Route("SharedService")]
    public class CustomerServiceController : ApiController
    {

        IAirtimeTopup _AirtimeTopup;
        public CustomerServiceController(IAirtimeTopup AirtimeTopup)
        {
            _AirtimeTopup = AirtimeTopup;
        }

        [HttpPost]  
        public async Task<IHttpActionResult> AirTimeTopup([FromBody]AirtimeRequest airTimeRequest)
        {
            AirtimeResponse response = await _AirtimeTopup.DoAirtimeTopUp(airTimeRequest);
            return Ok(response);
        }
    }
}
