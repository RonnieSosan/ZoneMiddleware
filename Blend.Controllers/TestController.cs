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
    public class TestController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(string id)
        {
            return Ok("Replying from Blend Test... " + id);
        }
    }
}
