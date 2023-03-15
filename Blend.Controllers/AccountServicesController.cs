using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Blend.Controllers
{
    [Route("AccountServices")]
    public class AccountServicesController : ApiController
    {
        IAccountServices _account;
        public AccountServicesController(IAccountServices accountService)
        {
            _account = accountService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> OpenAccount([FromBody]OpenAccountRequest accountRequest)
        {
            OpenAccountResponse response = await _account.OpenAccount(accountRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> FundAccount([FromBody]FundAccountRequest accountRequest)
        {
            FundAccountResponse response = await _account.FundAccount(accountRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> BlockFund([FromBody]BlockFundRequest blockFundRequest)
        {
            BlockFundResponse response = await _account.BlockFund(blockFundRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UnBlockFund([FromBody]UnBlockFundRequest unblockFundRequest)
        {
            UnBlockFundResponse response = await _account.UnBlockFund(unblockFundRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadDocument([FromBody]UploadRequest uploadRequest)
        {
            UploadResponse response = await _account.UploadDocument(uploadRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpdateBVN([FromBody]UpdateBVNRequest request)
        {
            UpdateBvnResponse response = await _account.UpdateBvn(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> UpgradeAccount([FromBody]UpgradeRequest request)
        {
            UpgradeResponse response = await _account.UpgradeAccount(request);
            return Ok(response);
        }
    }
}
