using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.NairaBox
{
    #region NairaBoxPurchases API Data Models
    public class NairaBoxPurchasesRequest
    {
        public string Referenceid { get; set; }
        public string RequestType { get; set; }
        public string Translocation { get; set; }
        public string showtimeid { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string adult { get; set; }
        public string student { get; set; }
        public string children { get; set; }
        public string TotalAmount { get; set; }
        public string DebitAccount { get; set; }
        public string reference { get; set; }
    }

    public class NairaBoxPurchasesReponse
    {
        public string message { get; set; }
        public string response { get; set; }
        public NairaBoxPurchasesReponseDetails responsedata { get; set; }
        public string data { get; set; }
    }
        
    public class NairaBoxPurchasesReponseDetails
    {
        public string status { get; set; }
        public string message { get; set; }
        [Newtonsoft.Json.JsonProperty("ref")]
        public string ticketRef { get; set; }
        public string ticketid { get; set; }
        public string download_url { get; set; }
    }


    
    



    #endregion
}
