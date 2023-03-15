using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    public partial class Showtime
    {
        public string id { get; set; }
        public TicketTypes ticketTypes { get; set; }
        public int available { get; set; }
        public string day { get; set; }
        public string date { get; set; }
        public DateTime mongodate { get; set; }
        public string ticketID { get; set; }
        public string uid { get; set; }
    }

    public class Adult
    {
        public string id { get; set; }
        public int price { get; set; }
    }

    public class Student
    {
        public string id { get; set; }
        public int price { get; set; }
    }

    public class Children
    {
        public string id { get; set; }
        public int price { get; set; }
    }

    public class TicketTypes
    {
        public Adult adult { get; set; }
        public Student student { get; set; }
        public Children children { get; set; }
    }
}
