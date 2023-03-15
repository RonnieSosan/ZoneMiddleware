using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class BillerCustomerValidation : BaseRequest
    {
        public List<Customer> customers { get; set; }
    }

    public class Customer
    {
        public string paymentCode { get; set; }
        public string customerId { get; set; }
        public string responseCode { get; set; }
        public string fullName { get; set; }
    }
}
