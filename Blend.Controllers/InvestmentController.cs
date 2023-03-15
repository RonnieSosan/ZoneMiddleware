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
    [Route("Investment")]
    public class InvestmentController : ApiController
    {
        IInvestmentService _investmentService;
        public InvestmentController(IInvestmentService investmentService)
        {
            _investmentService = investmentService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> RequestTreasuryBill([FromBody]TreasuryBillRequest treasuryBillRequest)
        {
            TreasuryBillResponse response = await _investmentService.RequestTreasuryBill(treasuryBillRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> RequestFixedDeposit([FromBody]FixedDepositRequest fixedDepostiRequest)
        {
            FixedDepositResponse response = await _investmentService.RequestFixedDeposit(fixedDepostiRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> TerminateFixedDeposit([FromBody]TerminateDepositRequest terminateRequest)
        {
            TerminateDepositResponse response = await _investmentService.TerminateFixedDeposit(terminateRequest);
            return Ok(response);
        }
    }
}
