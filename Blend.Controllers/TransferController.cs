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
    [Route("Transfer")]
    public class TransferController: ApiController
    {
        IBankTransfer _bankTransfer;

        public TransferController(IBankTransfer BankTransfer)
        {
            _bankTransfer = BankTransfer;
        }

        [HttpPost]
        public async Task<IHttpActionResult> SameBankTransfer([FromBody]IntraBankTransactionRequest transferRequest)
        {
            IntraBankTransactionResponse response = await _bankTransfer.SameBankTransfer(transferRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> InterBankTransfer([FromBody]FundsTransferRequest transferRequest)
        {
            FundsTransferResponse response = await _bankTransfer.InterBankTransfer(transferRequest);
            return Ok(response);
        }
    }
}
