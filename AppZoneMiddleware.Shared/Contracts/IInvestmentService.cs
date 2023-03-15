using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IInvestmentService
    {
        Task<TreasuryBillResponse> RequestTreasuryBill(TreasuryBillRequest request);
        Task<FixedDepositResponse> RequestFixedDeposit(FixedDepositRequest request);

        Task<TerminateDepositResponse> TerminateFixedDeposit(TerminateDepositRequest request);
    }
}
