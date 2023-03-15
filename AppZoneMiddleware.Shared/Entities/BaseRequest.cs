using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    [Serializable]
    public class BaseRequest
    {
        public string Passkey { get; set; }
        public string PhoneNumber { get; set; }
        public string customer_id { get; set; }
        public bool isMobile { get; set; }
        public string AuthToken { get; set; }
        public string PIN { get; set; }
        public string RequestChannel { get; set; }
        public string CustomerNumber { get; set; }
        public MailRequest MailRequest { get; set; }
    }

    [JsonObject]
    [Serializable]
    public abstract class BaseResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseDescription { get; set; }

    }

    [JsonObject]
    [Serializable]
    public class MailRequest
    {
        public Dictionary<string, byte[]> Attachement { get; set; }
        public string customer_id { get; set; }
        public string MailSubject { get; set; }
        public string MailRecepients { get; set; }
        public string MailBody { get; set; }
        public string AccountNumber { get; set; }
        public bool Use2FA { get; set; }
    }

    [JsonObject]
    [Serializable]
    public class MailResponse : BaseResponse
    {

    }
}