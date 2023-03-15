using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    public class FundAccountRequest : BaseRequest
    {
        public string Translocation { get; set; }
        public string TransactionReference { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RecipientBank { get; set; }
        public string RecipientAccountNumberOrWallet { get; set; }
        public string Amount { get; set; }
        public string Narration { get; set; }
        public string CardNo { get; set; }
        public string CVV { get; set; }
        public string CardPIN { get; set; }
        public string ExpiryYear { get; set; }
        public string ChargeAuth { get; set; }
        public string ExpiryMonth { get; set; }
        public string Fee { get; set; }
        public string Medium { get; set; }
        public string RedirectURL { get; set; }
    }
}
