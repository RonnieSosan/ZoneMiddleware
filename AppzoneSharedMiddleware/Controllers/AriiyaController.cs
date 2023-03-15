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
    [RoutePrefix("api/Ariiya")]
    public class AriiyaController : ApiController
    {
        IAriiyaTicketService _ariiyaService;

        public AriiyaController(IAriiyaTicketService ariiyaService)
        {
            _ariiyaService = ariiyaService;
        }

        [HttpGet]
        [ActionName("GetEvents")]
        // GET: Wakanow
        public IHttpActionResult GetEvents()
        {
            var response = _ariiyaService.GetEvents();
            return Ok(response);
        }

        [HttpPost]
        [ActionName("PlaceTicketOrder")]
        // GET: Wakanow
        public IHttpActionResult PlaceTicketOrder(AriiyaTicketOrderRequest orderRequest)
        {
            var response = _ariiyaService.PlaceTicketOrder(orderRequest);
            return Ok(response);
        }
    }
}
