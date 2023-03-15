using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IBankTransfer
    {
        Task<IntraBankTransactionResponse> SameBankTransfer(IntraBankTransactionRequest request);

        Task<FundsTransferResponse> InterBankTransfer(FundsTransferRequest TransferRequest);

        LifestyleDebitResponse LifeStyleDebit(LifestyleDebitRequest request);
    }
}
