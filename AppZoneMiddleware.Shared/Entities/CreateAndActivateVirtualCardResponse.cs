using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class CreateAndActivateVirtualCardResponse : BaseResponse
    {
        public string PAN { get; set; }

        public string ExpiryDate { get; set; }

        public string SequenceNumber { get; set; }
        
        public string CVV { get; set; }

        public string PostalAddress1 { get; set; }

        public string PostalAddress2 { get; set; }

        public string PostalCity { get; set; }

        public string PostalRegion { get; set; }

        public string PostalCode { get; set; }

        public string PostalCountry { get; set; }

        public string LinkedAccountNumber { get; set; }
    }
}
