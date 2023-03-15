using AppZoneMiddleware.Shared.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace AppZoneMiddleware.Shared.Entites
{
    public class PushNotification : BaseRequest
    {
        public string title
        {
            get
            {
                return "Push Notification";
            }
        }
        public string purpose { get; set; }
        public string reference_number
        {
            get
            {
                return System.DateTime.Now.ToString("yyddHHMMss") + customer_id;
            }
        }
        public string request_type { get; set; }
        public string account_number { get; set; }
        public string transaction_currency
        {
            get
            {
                return "NGN";
            }
        }
        public string transaction_amount { get; set; }
        public string transaction_beneficiary { get; set; }
        public string phone_number { get; set; }
        // public string unique_id { get; set; }
        public string client_id
        {
            get
            {
                return "UMLTD2203";
            }
        }
        public string api_key
        {
            get
            {
                return "71548c397dc7151d006a33edca7025d608a03baf";
            }
        }

        public string transaction_channel
        {
            get
            {
                return "Internet Banking";
            }
        }
        public string time
        {
            get
            {
                return string.Format("{0:yyyy-MM-dd hh:mm:ss}", DateTime.Now);
            }
        }
    }

    public class ProfieCreationrequest : BaseRequest
    {
        public CreationType creation_type { get; set; }
        public string unique_id { get; set; }
        public string AccountNumber { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Remarkcs { get; set; }
        public decimal Amount { get; set; }
        public string DebitAccount { get; set; }
        public string CreditAccount { get; set; }
    }

    public class ISECProfieCreationrequest
    {
        public string[] account_numbers { get; set; }
        public string customer_id { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }
        public string phone_number { get; set; }
        public string client_id
        {
            get
            {
                return "UMLTD2203";
            }
        }
        public string api_key
        {
            get
            {
                return "71548c397dc7151d006a33edca7025d608a03baf";
            }
        }
    }

    public class SecondaryAuthenticationResponse
    {
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string PendingAction { get; set; }

    }

    public enum RequestType
    {
        login, transaction, transfer, nip
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CreationType
    {
        internet, mobile
    }
}