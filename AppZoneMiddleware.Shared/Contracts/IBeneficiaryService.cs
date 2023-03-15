using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Entities.BeneficiaryService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IBeneficiaryService
    {
        GetBeneficiariesResponse GetBeneficiaries(UserBeneficiaries Request);
        GetBeneficiariesResponse AddBeneficiary(AddBenefciariesRequest Request);
        GetBeneficiariesResponse RemoveBeneficiary(RemoveBeneficiaryRequest Request);
    }
}
