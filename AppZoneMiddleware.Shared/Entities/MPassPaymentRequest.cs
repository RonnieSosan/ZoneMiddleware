using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    public class MPassPaymentRequest : BaseRequest
    {
        public string Translocation { get; set; }
        public string LocalDate { get; set; }
        public string LocalTime { get; set; }
        public string TransactionReference { get; set; }
        public string SenderName_First { get; set; }
        public string SenderName_Middle { get; set; }
        public string SenderName_Last { get; set; }
        public string SenderPhone { get; set; }
        public string SenderDateOfBirth { get; set; }
        public string SenderAddress_Line1 { get; set; }
        public string SenderAddress_Line2 { get; set; }
        public string SenderAddress_City { get; set; }
        public string SenderAddress_CountrySubdivision { get; set; }
        public string SenderAddress_PostalCode { get; set; }
        public string SenderAddress_Country { get; set; }
        public string FundingCard_AccountNumber { get; set; }
        public string FundingSource { get; set; }
        public string AdditionalMessage { get; set; }
        public string ParticipationID { get; set; }
        public string LanguageIdentification { get; set; }
        public string LanguageData { get; set; }
        public string ReceivingCard_AccountNumber { get; set; }
        public string ReceiverName_Middle { get; set; }
        public string ReceiverName_Last { get; set; }
        public string ReceiverAddress_Line1 { get; set; }
        public string ReceiverAddress_Line2 { get; set; }
        public string ReceiverAddress_City { get; set; }
        public string ReceiverAddress_CountrySubdivision { get; set; }
        public string ReceiverAddress_PostalCode { get; set; }
        public string ReceiverAddress_Country { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverDateOfBirth { get; set; }
        public string ReceivingAmount_Value { get; set; }
        public string ReceivingAmount_Currency { get; set; }
        public string ProcessorId { get; set; }
        public string RoutingAndTransitNumber { get; set; }
        public string CardAcceptor_Name { get; set; }
        public string CardAcceptor_City { get; set; }
        public string CardAcceptor_State { get; set; }
        public string CardAcceptor_PostalCode { get; set; }
        public string CardAcceptor_Country { get; set; }
        public string TransactionDesc { get; set; }
        public string MerchantId { get; set; }
        public string ReceiverIdentification_Type { get; set; }
        public string ReceiverIdentification_Number { get; set; }
        public string ReceiverIdentification_CountryCode { get; set; }
        public string ReceiverIdentification_ExpirationDate { get; set; }
        public string ReceiverNationality { get; set; }
        public string ReceiverCountryOfBirth { get; set; }
        public string SenderIdentification_Type { get; set; }
        public string SenderIdentification_Number { get; set; }
        public string SenderIdentification_CountryCode { get; set; }
        public string SenderIdentification_ExpirationDate { get; set; }
        public string SenderNationality { get; set; }
        public string SenderCountryOfBirth { get; set; }
        public string TransactionPurpose { get; set; }
    }
}
