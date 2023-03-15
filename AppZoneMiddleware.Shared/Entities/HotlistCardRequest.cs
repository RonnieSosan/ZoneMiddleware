using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class HotlistCardRequest : BaseRequest
    {
        public string CardPAN { get; set; }
        public string ReasonForHotList { get; set; }
        public string last_updated_user { get; set; }
        public string expiryDate { get; set; }
    }

    public class HotlistCardResponse : BaseResponse { }

    public class RetrieveCardRequest : BaseRequest
    {
        public string PAN { get; set; }
        public string ExpDate { get; set; }
        public string AccountNumber { get; set; }
    }

    public class RetrieveCardResponse : BaseResponse
    {
        public IList<Card> Cards { get; set; }
    }

    public class ActivateCardRequest : BaseRequest
    {
        public string PAN { get; set; }
        public string ExpDate { get; set; }
        public string Seq_nr { get; set; }
        public string ActivationPin { get; set; }
    }

    public class ActivateCardResponse : BaseResponse { }
}
