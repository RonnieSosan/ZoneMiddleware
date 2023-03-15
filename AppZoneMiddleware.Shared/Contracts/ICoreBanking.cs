using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IAccountInquiry
    {
        Task<CustomerAccountsResponse> GetAccountsWithCustomerID(AccountRequest accountRequest);

        Task<AccountResponse> ValidateAccountNumber(AccountRequest accountRequest);

        Task<NameInquiryResponse> InterBankNameInquiry(NameInquiryRequest nameEnquiryRequest);

        Task<BalanceResponse> BalanceEnquiry(AccountRequest accountRequest);

        Task<BVNInquiryResponse> DoBVNInquiry(BVNInquiryRequest bvnInquiryRequest);

        Task<NameInquiryResponse> SameBankNameInquiry(AccountRequest accountNumber);

        Task<NIPResponse> DoNIPBVNInquiry(NIPRequest BVNInquiry);

        Task<AccountStatementResponse> GetMiniStatement(AccountStatementRequest statementRequest);

        Task<CreditCheckResponse> CreditBureauCheck(CreditCheckRequest request);
        
        Task<ExistingAccountHolderResponse> FetchExistingAccountHolderData(ExistingAccountHolderRequest accountRequest);
    }
}
