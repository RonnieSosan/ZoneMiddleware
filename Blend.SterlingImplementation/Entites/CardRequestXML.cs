using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blend.SterlingImplementation.Entites
{
    [XmlRoot("IBSRequest")]
    public class CardRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string AppID { get; set; }
        [XmlElement]
        public string NUBAN { get; set; }
        [XmlElement]
        public DateTime DateRequested { get; set; }
        [XmlElement]
        public string Carddeliverybranch { get; set; }
        [XmlElement]
        public string Pindeliverybranch { get; set; }
        [XmlElement]
        public string Cuscity { get; set; }
        [XmlElement]
        public string cusregion { get; set; }
    }

    [XmlRoot("IBSRequest")]
    public class VisaCardRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string AppID { get; set; }
        [XmlElement]
        public string NUBAN { get; set; }
        [XmlElement]
        public string CardFirstName { get; set; }
        [XmlElement]
        public string CardSurnameName { get; set; }
        [XmlElement]
        public string Birthday { get; set; }
        [XmlElement]
        public string IdentificationNum { get; set; }
        [XmlElement]
        public string Issuedate { get; set; }
        [XmlElement]
        public string Expirydate { get; set; }
        [XmlElement]
        public string PlaceofIssue { get; set; }
        [XmlElement]
        public string SecretQuestion { get; set; }
        [XmlElement]
        public string SecretAnswer { get; set; }
        [XmlElement]
        public string Card { get; set; }
        [XmlElement]
        public string Resident { get; set; }
        [XmlElement]
        public string stringCountryResident { get; set; }
        [XmlElement]
        public string CountryofReg { get; set; }
        [XmlElement]
        public string BillingRegion { get; set; }
        [XmlElement]
        public string BillingCityofReg { get; set; }
        [XmlElement]
        public string BillingAddress { get; set; }
        [XmlElement]
        public string HomeCountry { get; set; }
        [XmlElement]
        public string HomeRegion { get; set; }
        [XmlElement]
        public string HomeAddress { get; set; }
        [XmlElement]
        public string CarddeliveryBranch { get; set; }
        [XmlElement]
        public string PinDeliveryBranch { get; set; }
    }
}
