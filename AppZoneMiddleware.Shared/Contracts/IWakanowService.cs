using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Entities.Wakanow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IWakanowService
    {
        Task<WakanowAirport[]> GetAirports();
        Task<SearchResult> FlightSerach(FlightSearchRequest searchRequest);
        Task<SelectFlightResponse> SelectFlight(SelectFlightRequest selectRequest);
        Task<FlightBookResponse> FlightBook(FlightBookRequest BookingRequest);
        Task<TicketConfirmResponse> TicketConfirm(TicketConfirmRequest confirmRequest);
    }
}
