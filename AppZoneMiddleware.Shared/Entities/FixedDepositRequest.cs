using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class FixedDepositRequest : BaseRequest
    {
        public string ChangePeriod { get; set; }
        public string PayInAcct { get; set; }
        public string PayOutAcct3 { get; set; }
        public string PayOutAcct2 { get; set; }
        public string PayOutAcct1 { get; set; }
        public string Currency { get; set; }
        public string Amount { get; set; }
        public string Rate { get; set; }
        public string EffectiveDate { get; set; }
        public InterestType InterestType { get; set; }

    }

    public enum InterestType
    {
        WithInterest = 0,
        WithoutInterest = 1
    }

    public class FixedDepositResponse : BaseResponse
    {
        public string ArrangementID { get; set; }
    }


    public class TerminateDepositRequest : BaseRequest
    {
        public string arrangementid { get; set; }
    }

    public class TerminateDepositResponse : BaseResponse { }
}
