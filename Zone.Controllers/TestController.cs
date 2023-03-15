using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Zone.Controllers
{
    public class TestController:ApiController
    {
        public IHttpActionResult Get()   
        {
            return Ok("Replying from Zone...");
        }
    }
}
