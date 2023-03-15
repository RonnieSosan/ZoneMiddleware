using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    public class FundAccountResponse : BaseResponse
    {
        public FundAccountResponseDetails ResponseDetails { get; set; }
    }

    [JsonObject]
    public class FundAccountResponseDetails
    {
        public string Status { get; set; }
        public FundAccountResponseData ResponseData { get; set; }
        public string RequestId { get; set; }
        public string TransactionReference { get; set; }
        public string TransferType { get; set; }
        public string SystemTraceAuditNumber { get; set; }
        public string NetworkReferenceNumber { get; set; }
        public string SettlementDate { get; set; }
        public string ResponseCode { get; set; }
        public string Response_Description { get; set; }
        public string SubmitDateTime { get; set; }
    }

    [JsonObject]
    public class FundAccountResponseData
    {
        public FundAccountResponseTransferDetails TransferDetails { get; set; }
        public string AuthUrl { get; set; }
        public string ResponseHTML { get; set; }
        public bool PendingValidation { get; set; }
        public string ChargeMethod { get; set; }
    }

    [JsonObject]
    public class FundAccountResponseTransferDetails
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string RecipientPhone { get; set; }
        public string Status { get; set; }
        public string SystemStatus { get; set; }
        public string Medium { get; set; }
        public string IPAddress { get; set; }
        public string ExchangeRate { get; set; }
        public string AmountToSend { get; set; }
        public string AmountToCharge { get; set; }
        public string DisburseCurrency { get; set; }
        public string ChargeCurrency { get; set; }
        public string FlutterChargeResponseCode { get; set; }
        public string FlutterChargeResponseMessage { get; set; }
        public string FlutterDisburseResponseMessage { get; set; }
        public string FlutterChargeReference { get; set; }
        public string FlutterDisburseReference { get; set; }
        public string FlutterDisburseResponseCode { get; set; }
        public string MerchantCommission { get; set; }
        public string MoneywaveCommission { get; set; }
        public string NetDebitAmount { get; set; }
        public string ChargedFee { get; set; }
        public string ReceiptNumber { get; set; }
        public string RedirectUrl { get; set; }
        public string LinkingReference { get; set; }
        public string Source { get; set; }
        public string SourceID { get; set; }
        public FundAccountResponseMetaDetails Meta { get; set; }
        public string AdditionalFields { get; set; }
        public string Ref { get; set; }
        public string R1 { get; set; }
        public string R2 { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateDeleted { get; set; }
        public string UserId { get; set; }
        public string MerchantId { get; set; }
        public string BeneficiaryId { get; set; }
        public string AccountId { get; set; }
        public string CardId { get; set; }
        public string Account { get; set; }
        public FundAccountResponseBeneficiaryDetails BeneficiaryDetails { get; set; }
    }

    [JsonObject]
    public class FundAccountResponseMetaDetails
    {
    }

    [JsonObject]
    public class FundAccountResponseBeneficiaryDetails
    {
        public string ID { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string UserId { get; set; }
        public string Currency { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateDeleted { get; set; }
    }
}
