using AppZoneMiddleware.Shared.Entities.NairaBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface INairaBoxService
    {
        Task<GetMoviesResponse> GetMovies();
        Task<GetNairaBoxMovieResponse> GetMovieById(string MovieId);
        Task<GetMovieDetailsResponse> GetMovieDetails(string MovieId);
        Task<GetByCinemaResponse> GetByCinema();
        Task<GetShowtimeResponse> GetShowtime(GetShowtimeRequest Request);
        Task<GetEventsResponse> GetEvents();
        Task<PurcaseTicketResponse> PurchaseTicket(PurchaseTicketRequest Request, string PurchaseType);
        Task<VerifyTicketResponse> VerifyTicket(string RefId);
    }
}
