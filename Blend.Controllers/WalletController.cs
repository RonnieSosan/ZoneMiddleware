using AppZoneMiddleware.Shared.Entities;
using Blend.SterlingImplementation.CoreBankingService;
using Blend.SterlingImplementation.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Blend.Controllers
{
    [Route("WalletService")]
    public class WalletServiceController : ApiController
    {
        WalletAccountService service = new WalletAccountService();

        [HttpPost]
        public async Task<IHttpActionResult> OpenAccount([FromBody]CreateWalletRequest accountRequest)
        {
            WalletResponse response = await service.CreateWalletAccount(accountRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetWalletBalance([FromBody]WalletBalanceRequest BalanceRequest)
        {
            WalletResponse response = await service.GetWalletBalance(BalanceRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetWalletDetails([FromBody]GetWalletDetails request)
        {
            WalletDetails response = await service.GetWalletDetails(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> DoWalletTransaction([FromBody]WalletTransactionRequest request)
        {
            WalletTransactionResponse response = await service.DoWalletTransaction(request);
            return Ok(response);
        }
    }
}
