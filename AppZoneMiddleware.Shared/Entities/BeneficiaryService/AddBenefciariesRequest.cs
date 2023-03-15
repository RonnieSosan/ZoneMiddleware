using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.BeneficiaryService
{
    public class AddBenefciariesRequest : Beneficiary
    {
        [Required]
        public string CustomerID { get; set; }
    }
}
