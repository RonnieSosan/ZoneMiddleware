using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class AccountRequest : BaseRequest
    {
        public string AccountNumber { get; set; }
        public string CustomerID { get; set; }
    }


    public class AccountDetails
    {
        public string CustomerID { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountProductCode { get; set; }
        public string SEX { get; set; }
        public string TITLE { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE { get; set; }
        public string ACCOUNTBALANCE { get; set; }
        public string AccountCurrency { get; set; }
        public string BirthDate { get; set; }
        public string BVN { get; set; }
        public string AccountStatus { get; set; }
        public string emailAddress { get; set; }
        public string AccountType { get; set; }
        public string FullName { get; set; }
        public string LedgerCode { get; set; }

    }
    public class AccountResponse : BaseResponse
    {
        public AccountDetails AccountInformation { get; set; }
    }

    public class CustomerAccountsResponse : BaseResponse
    {
        public IList<AccountDetails> AccountInformation { get; set; }
    }

    public class OpenAccountRequest : BaseRequest
    {
        public string ref_num { get; set; }
        [Required]
        public string Title { get; set; }
        public string bvn { get; set; }
        public string mobile_no { get; set; }
        [Required]
        public string email { get; set; }
        public string bra_code { get; set; }
        public int account_type { get; set; }
        public string surname { get; set; }
        [Required]
        public string first_name { get; set; }
        [Required]
        public string last_name { get; set; }
        [Required]
        public string middle_name { get; set; }
        public string account_name { get; set; }
        [Required]
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string id_image_url { get; set; }
        public string photo_image_url { get; set; }
        [Required]
        public string date_of_birth { get; set; }
        [Required]
        public string gender { get; set; }
        public OpenAccountClass DesiredAccountClass { get; set; }
        /// <summary>
        ///  The three (3) letter currency code. See ISO 4217 currency code ("alphabetic code") standard. This will default to NGN if not supplied. 
        /// </summary>
        public string DesiredCurrencyCode { get; set; }
    }

    public enum OpenAccountClass 
    {
        Tier1Savings = 0,
        Tier2Savings = 1,
        Current = 2,
    }

    public class OpenAccountResponse : BaseResponse
    {
        public string AccountNo { get; set; }
        public string CustomerID { get; set; }
        public string AccountName { get; set; }
        public string Currency { get; set; }
    }

    public class NameInquiryResponse : BaseResponse
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string SessionID { get; set; } 
    }

    public class NameInquiryRequest : BaseRequest
    {
        public string myDestinationBankCode { get; set; }
        public string myAccountNumber { get; set; }
        public string myChannelCode { get; set; }
        public string DestinationAccountNumber { get; set; }
    }

    public class NIPRequest : BaseRequest
    {
        public string BVN { get; set; }
    }

    public class NIPResponse : BaseResponse
    {
        public string BVN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string DateOfBirth { get; set; }
        public string MiddleName { get; set; }
    }

    public class NIPBankResponse : BaseResponse
    {
        public List<NIPBanks> TheBanks { get; set; }

    }

    public class NIPBanks
    {
        public string Name { get; set; }
        public string BankCode { get; set; }
    }

    public class BalanceResponse : BaseResponse
    {
        public string AccountNumber { get; set; }
        public string Balance { get; set; }
    }
}
