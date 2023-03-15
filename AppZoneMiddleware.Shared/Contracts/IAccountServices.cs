using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IAccountServices
    {
        Task<StopChequeResponse> StopCheque(StopChequeRequest stopCheque);

        Task<OpenAccountResponse> OpenAccount(OpenAccountRequest openAccount);

        Task<PNDResponse> PlacePND(PNDRequest Request);
        Task<UpgradeResponse> UpgradeAccount(UpgradeRequest Request);
        Task<BlockFundResponse> BlockFund(BlockFundRequest request);
        Task<UnBlockFundResponse> UnBlockFund(UnBlockFundRequest request);
        Task<UploadResponse> UploadDocument(UploadRequest request);
        Task<UpdateBvnResponse> UpdateBvn(UpdateBVNRequest request);
        Task<FundAccountResponse> FundAccount(FundAccountRequest Request);
    }
}
