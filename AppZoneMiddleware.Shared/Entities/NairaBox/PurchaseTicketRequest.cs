using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public class NiraboxTicketRequest
    {
        public string auth { get; set; }
        public UserDetails userDetails { get; set; }
        public TicketInfo ticketInfo { get; set; }
        public int totalTickets { get; set; }
        public string reference { get; set; }
    }

    public class PurchaseTicketRequest
    {
        public string fullname { get; set; }
        public string showTimeId { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string TicketType { get; set; }
        public string TicketTypeId { get; set; }
        public string Qty { get; set; }
        public string Reference { get; set; }
        public string purchaseType { get; set; }
        public string classid { get; set; }
    }

    public class NairaBoxEventTicketRequest
    {
        public string qty { get; set; }
        public string classid { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string reference { get; set; }
        public string auth { get; set; }
    }

    public class UserDetails
    {
        public string fullname { get; set; }
        public string ticketType { get; set; }
        public string ticketTypeId { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public int quantity { get; set; }
    }

    public class TicketInfo
    {
        public string showTimeId { get; set; }
    }
    public enum PurchaseType
    {
        Movie = 1,
        Event = 2
    }
}
