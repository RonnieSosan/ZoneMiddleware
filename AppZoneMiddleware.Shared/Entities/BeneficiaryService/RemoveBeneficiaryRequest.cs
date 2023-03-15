using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.BeneficiaryService
{
    public class RemoveBeneficiaryRequest
    {
        public string CustomerID { get; set; }
        public string BeneficiaryName { get; set; }
        public string AccountNumber { get; set; }
    }
}
