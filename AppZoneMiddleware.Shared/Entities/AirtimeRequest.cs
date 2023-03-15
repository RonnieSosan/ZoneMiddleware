using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class AirtimeRequest : BaseRequest
    {
        public string RechargeSuspenceAccount { get; set; }
        public Telco BatchId { get; set; }
        public string CustomerAccount { get; set; }
        public string CustomerAccountName { get; set; }
        public decimal RechargeAmount { get; set; }
        public decimal IncomeAmount { get; set; }
        public bool ZoneOriginated { get; set; }
        public ServiceType ServiceType { get; set; }
        public string ExtraParams { get; set; }
        public string Currency { get; set; }
    }

    public class AirtimeExtraData
    {
        public string BeneficiaryPhoneNumber { get; set; }
        public string Type { get; set; }
    }

    public enum Telco
    {
        Airtel = 1,
        Etisalat = 2,
        Visafone = 3,
        MTN = 5,
        Glo = 6
    }

    public enum ServiceType
    {
        MobileTopUp = 2,
        DataBundle = 16
    }

    public class AirtimeResponse : BaseResponse
    {
        public string ConfirmationCode { get; set; }
        public string ProviderResponse { get; set; }
        public string AuditNo { get; set; }

        public RechargeStatus Status { get; set; }
    }

    public enum RechargeStatus
    {
        Successful = 1,
        PendingReversal = 2,
        DebitFailed = 3,
        RechargeFailed = 4,
        PendingConfirmation = 5,
    }
}
