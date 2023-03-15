using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IAriiyaTicketService
    {
        AriiyaGetEventsResponse GetEvents();

        AriiyaOrderResponse PlaceTicketOrder(AriiyaTicketOrderRequest request);
    }
}
