using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    [JsonObject]
    public class FundAccountRequestJSON : sPayBaseRequest
    {
        public string TransactionReference { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phonenumber { get; set; }
        public string recipient_bank { get; set; }
        public string recipient_account_number { get; set; }
        public string amount { get; set; }
        public string narration { get; set; }
        public string card_no { get; set; }
        public string cvv { get; set; }
        public string pin { get; set; }
        public string expiry_year { get; set; }
        public string charge_auth { get; set; }
        public string expiry_month { get; set; }
        public string fee { get; set; }
        public string medium { get; set; }
        public string redirecturl { get; set; }
        public string CreditAccount { get; set; }
    }
}
