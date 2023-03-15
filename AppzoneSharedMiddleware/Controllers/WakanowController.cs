using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities.Wakanow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AppzoneSharedMiddleware.Controllers
{
    [RoutePrefix("api/Wakanow")]
    public class WakanowController : ApiController
    {
        IWakanowService _wakanowService = null;

        public WakanowController(IWakanowService wakanowService)
        {
            _wakanowService = wakanowService;
        }

        [HttpGet]
        [ActionName("Airports")]
        // GET: Wakanow
        public async Task<IHttpActionResult> GetAirports()
        {
            var response = await _wakanowService.GetAirports();
            return Ok(response);
        }

        [HttpPost]
        [Route("SearchFilghts")]
        // GET: Wakanow
        public async Task<IHttpActionResult> SearchFilghts(FlightSearchRequest searchRequest)
        {
            var response = await _wakanowService.FlightSerach(searchRequest);
            return Ok(response);
        }

        [HttpPost]
        [Route("SelectFilght")]
        // GET: Wakanow
        public async Task<IHttpActionResult> SelectFilght(SelectFlightRequest selectRequest)
        {
            var response = await _wakanowService.SelectFlight(selectRequest);
            return Ok(response);
        }

        [HttpPost]
        [Route("bookFilght")]
        // GET: Wakanow
        public async Task<IHttpActionResult> BookFlight(FlightBookRequest bookRequest)
        {
            var response = await _wakanowService.FlightBook(bookRequest);
            return Ok(response);
        }

        [HttpPost]
        [Route("ConfirmTicket")]
        // GET: Wakanow
        public async Task<IHttpActionResult> ConfirmTicket(TicketConfirmRequest confirmRequest)
        {
            var response = await _wakanowService.TicketConfirm(confirmRequest);
            return Ok(response);
        }
    }
}