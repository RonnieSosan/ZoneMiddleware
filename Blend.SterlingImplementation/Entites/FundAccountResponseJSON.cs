using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    [JsonObject]
    public class FundAccountResponseJSON : sPayBaseResponse
    {
        public string message { get; set; }
        public string response { get; set; }
        public FundAccountResponseJSONDetails responsedata { get; set; }
        public string data { get; set; }
    }

    [JsonObject]
    public class FundAccountResponseJSONDetails
    {
        public string status { get; set; }
        public FundAccountResponseJSONData data { get; set; }
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
    public class FundAccountResponseJSONData
    {
        public FundAccountResponseJSONTransferDetails transfer { get; set; }
        public string authurl { get; set; }
        public string responsehtml { get; set; }
        public bool pendingValidation { get; set; }
        public string chargeMethod { get; set; }
    }

    [JsonObject]
    public class FundAccountResponseJSONTransferDetails
    {
        public string id { get; set; }
        public string type { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string recipientPhone { get; set; }
        public string status { get; set; }
        public string system_status { get; set; }
        public string medium { get; set; }
        public string ip { get; set; }
        public string exchangeRate { get; set; }
        public string amountToSend { get; set; }
        public string amountToCharge { get; set; }
        public string disburseCurrency { get; set; }
        public string chargeCurrency { get; set; }
        public string flutterChargeResponseCode { get; set; }
        public string flutterChargeResponseMessage { get; set; }
        public string flutterDisburseResponseMessage { get; set; }
        public string flutterChargeReference { get; set; }
        public string flutterDisburseReference { get; set; }
        public string flutterDisburseResponseCode { get; set; }
        public string merchantCommission { get; set; }
        public string moneywaveCommission { get; set; }
        public string netDebitAmount { get; set; }
        public string chargedFee { get; set; }
        public string receiptNumber { get; set; }
        public string redirectUrl { get; set; }
        public string linkingReference { get; set; }
        public string source { get; set; }
        public string source_id { get; set; }
        public FundAccountResponseJSONMetaDetails meta { get; set; }
        public string additionalFields { get; set; }
        public string _ref { get; set; }
        public string r1 { get; set; }
        public string r2 { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime deletedAt { get; set; }
        public string userId { get; set; }
        public string merchantId { get; set; }
        public string beneficiaryId { get; set; }
        public string accountId { get; set; }
        public string cardId { get; set; }
        public string account { get; set; }
        public FundAccountResponseJSONBeneficiaryDetails beneficiary { get; set; }
    }

    [JsonObject]
    public class FundAccountResponseJSONMetaDetails
    {
    }

    [JsonObject]
    public class FundAccountResponseJSONBeneficiaryDetails
    {
        public string id { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string bankCode { get; set; }
        public string bankName { get; set; }
        public string userId { get; set; }
        public string currency { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime deletedAt { get; set; }
    }

}
