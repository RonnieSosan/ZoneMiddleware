using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class CardRequest : BaseRequest
    {
        public string NUBAN { get; set; }
        public DateTime DateRequested { get; set; }
        public string Carddeliverybranch { get; set; }
        public string Pindeliverybranch { get; set; }
        public string Cuscity { get; set; }
        public string cusregion { get; set; }
        public string CardType { get; set; }
    }

    public class VisaCardRequest : BaseRequest
    {
        public string NUBAN { get; set; }
        public string CardFirstName { get; set; }
        public string CardSurnameName { get; set; }
        public string Birthday { get; set; }
        public string IdentificationNum { get; set; }
        public string Issuedate { get; set; }
        public string Expirydate { get; set; }
        public string PlaceofIssue { get; set; }
        public string SecretQuestion { get; set; }
        public string SecretAnswer { get; set; }
        public string Card { get; set; }
        public string Resident { get; set; }
        public string stringCountryResident { get; set; }
        public string CountryofReg { get; set; }
        public string BillingRegion { get; set; }
        public string BillingCityofReg { get; set; }
        public string BillingAddress { get; set; }
        public string HomeCountry { get; set; }
        public string HomeRegion { get; set; }
        public string HomeAddress { get; set; }
        public string CarddeliveryBranch { get; set; }
        public string PinDeliveryBranch { get; set; }
    }

    public class CardResponse : BaseResponse
    {
    }

    public class CreditCardRequest : BaseRequest
    {
        public string Title { get; set; }//Mr
        public string LastName { get; set; }//ALAO
        public string FirstName { get; set; } //RAMON
        public string AccountNumber { get; set; }
        public CreditCardType CardType { get; set; }
    }

    public class ActivateChannelRequest : BaseRequest
    {
        public string CardPAN { get; set; }
        public Dictionary<CardChannel, bool> ActivationOptions { get; set; }
    }

    public class ActivateChannelResponse : BaseResponse
    {
        public Dictionary<CardChannel, string> ActivationResponse { get; set; }
    }

    public class CardActionRequest : BaseRequest
    {
        public string CardPAN { get; set; }
        public CardAction CardAction { get; set; }
    }

    public enum CardAction
    {
        ActivateForeignTrnx = 0,
        DeactivateForeignTrnx = 1,
    }

    public enum CreditCardType
    {
        MasterCard = 0,
        VisaCard = 1
    }

    public enum CardChannel
    {
        Atm = 0,
        Pos = 1,
        Web = 2
    }
}
