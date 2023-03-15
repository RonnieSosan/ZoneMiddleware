using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.BeneficiaryService
{
    public class GetBeneficiariesResponse : BaseResponse
    {
        public List<Beneficiary> Beneficiaries { get; set; }
    }
}
