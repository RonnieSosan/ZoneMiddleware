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
    [Route("AccountValidation")]
    public class AccountValidationController : ApiController
    {
        IAccountInquiry _CoreBanking;
        public AccountValidationController(IAccountInquiry CoreBankingService)
        {
            _CoreBanking = CoreBankingService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetAccountWithCustomerID([FromBody]AccountRequest accountRequest)
        {
            CustomerAccountsResponse response = await _CoreBanking.GetAccountsWithCustomerID(accountRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ValidateAccountNumber([FromBody]AccountRequest accountRequest)
        {
            AccountResponse response = await _CoreBanking.ValidateAccountNumber(accountRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> BalanceEnquiry([FromBody]AccountRequest accountRequest)
        {
            BalanceResponse response = await _CoreBanking.BalanceEnquiry(accountRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> InterBankNameInquiry([FromBody]NameInquiryRequest nameEnquiryRequest)
        {
            NameInquiryResponse response = await _CoreBanking.InterBankNameInquiry(nameEnquiryRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> SameBankNameInquiry([FromBody]AccountRequest accountRequest)
        {
            NameInquiryResponse response = await _CoreBanking.SameBankNameInquiry(accountRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> DoNIPBVNInquiry([FromBody]NIPRequest BVNInquiry)
        {
            NIPResponse response = await _CoreBanking.DoNIPBVNInquiry(BVNInquiry);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetMiniStatement([FromBody]AccountStatementRequest StatementRequest)
        {
            AccountStatementResponse response = await _CoreBanking.GetMiniStatement(StatementRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreditBureauCheck([FromBody]CreditCheckRequest creditCheckRequest)
        {
            CreditCheckResponse response = await _CoreBanking.CreditBureauCheck(creditCheckRequest);
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> FetchExistingAccountHolderData([FromBody]ExistingAccountHolderRequest reqData)
        {
            ExistingAccountHolderResponse respData = await _CoreBanking.FetchExistingAccountHolderData(reqData);
            return Ok(respData);
        }
    }
}
