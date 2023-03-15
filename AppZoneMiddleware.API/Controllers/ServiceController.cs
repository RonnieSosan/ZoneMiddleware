using AppZoneMiddleware.API.Infrastructure;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppZoneMiddleware.API.Controllers
{
    public class ServiceController : ApiController
    {
        string ADUsername = System.Configuration.ConfigurationManager.AppSettings["ADUsername"];

        [HttpGet]
        public IHttpActionResult Get()
        {
            try
            {
                Dictionary<string, List<string>> services = new AllowedServices().DisplayServices(ADUsername);
                return Ok(services);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Add([FromBody]UpdateServiceRequest request)
        {
            try
            {
                string errorMessage = string.Empty;
                Dictionary<string, List<string>> services = new AllowedServices().AddServiceToList(request, out errorMessage);
                if (errorMessage != string.Empty)
                    return BadRequest(errorMessage);
                else
                    return Ok(services);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Remove([FromBody]UpdateServiceRequest request)
        {
            try
            {
                string errorMessage = string.Empty;
                Dictionary<string, List<string>> services = new AllowedServices().RemoveServiceFromList(request, out errorMessage);
                if (errorMessage != string.Empty)
                    return BadRequest(errorMessage);
                else
                    return Ok(services);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }


    }
}
