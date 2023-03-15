using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities.NairaBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AppzoneSharedMiddleware.Controllers
{
    [RoutePrefix("api/NairaBox")]
    public class NairaBoxController : ApiController
    {
        INairaBoxService _nairaBoxService = null;

        public NairaBoxController(INairaBoxService nairaBoxService)
        {
            _nairaBoxService = nairaBoxService;
        }

        [HttpGet]
        [ActionName("GetMovies")]
        // GET: Wakanow
        public async Task<IHttpActionResult> GetMovies()
        {
            var response = await _nairaBoxService.GetMovies();
            return Ok(response);
        }

        [HttpGet]
        [Route("GetMovieById/{MovieId}")]
        // GET: Wakanow
        public async Task<IHttpActionResult> GetMovieById([FromUri]string MovieId)
        {
            var response = await _nairaBoxService.GetMovieById(MovieId);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetMovieDetails/{MovieId}")]
        // GET: Wakanow
        public async Task<IHttpActionResult> GetMovieDetails([FromUri]string MovieId)
        {
            var response = await _nairaBoxService.GetMovieDetails(MovieId);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetByCinema")]
        // GET: Wakanow
        public async Task<IHttpActionResult> GetByCinema()
        {
            var response = await _nairaBoxService.GetByCinema();
            return Ok(response);
        }

        [HttpPost]
        [Route("GetShowtime")]
        // GET: Wakanow
        public async Task<IHttpActionResult> GetShowtime(GetShowtimeRequest Request)
        {
            var response = await _nairaBoxService.GetShowtime(Request);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetEvents")]
        // GET: Wakanow
        public async Task<IHttpActionResult> GetEvents()
        {
            var response = await _nairaBoxService.GetEvents();
            return Ok(response);
        }

        [HttpPost]
        [Route("PurchaseTicket/{PurchaseType}")]
        // GET: Wakanow
        public async Task<IHttpActionResult> PurchaseTicket(PurchaseTicketRequest Request, string PurchaseType)
        {
            var response = await _nairaBoxService.PurchaseTicket(Request, PurchaseType);
            return Ok(response);
        }

        [HttpGet]
        [Route("VerifyTicket/{RefId}")]
        // GET: Wakanow
        public async Task<IHttpActionResult> VerifyTicket(string RefId)
        {
            var response = await _nairaBoxService.VerifyTicket(RefId);
            return Ok(response);
        }
    }
}
