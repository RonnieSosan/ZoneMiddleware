using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class CustomerValidationResponse : InterswitchBaseResponse
    {
        public List<Customer> Customers { get; set; }
    }
}
