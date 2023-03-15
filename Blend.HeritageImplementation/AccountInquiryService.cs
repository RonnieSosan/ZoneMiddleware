using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;

namespace Blend.HeritageImplementation
{
    public class AccountInquiryService : IAccountInquiry
    {
        public Task<BalanceResponse> BalanceEnquiry(AccountRequest accountRequest)
        {
            throw new NotImplementedException();
        }

        public Task<CreditCheckResponse> CreditBureauCheck(CreditCheckRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BVNInquiryResponse> DoBVNInquiry(BVNInquiryRequest bvnInquiryRequest)
        {
            throw new NotImplementedException();
        }

        public Task<NIPResponse> DoNIPBVNInquiry(NIPRequest BVNInquiry)
        {
            throw new NotImplementedException();
        }

        public Task<ExistingAccountHolderResponse> FetchExistingAccountHolderData(ExistingAccountHolderRequest accountRequest)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerAccountsResponse> GetAccountsWithCustomerID(AccountRequest accountRequest)
        {
            throw new NotImplementedException();
        }

        public Task<AccountStatementResponse> GetMiniStatement(AccountStatementRequest statementRequest)
        {
            throw new NotImplementedException();
        }

        public Task<NameInquiryResponse> InterBankNameInquiry(NameInquiryRequest nameEnquiryRequest)
        {
            throw new NotImplementedException();
        }

        public Task<NameInquiryResponse> SameBankNameInquiry(AccountRequest accountNumber)
        {
            throw new NotImplementedException();
        }

        public Task<AccountResponse> ValidateAccountNumber(AccountRequest accountRequest)
        {
            throw new NotImplementedException();
        }
    }
}
