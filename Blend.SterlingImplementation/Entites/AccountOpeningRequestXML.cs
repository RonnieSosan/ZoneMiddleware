using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blend.SterlingImplementation.Entites
{
    [Serializable]
    [XmlRoot("IBSRequest")]
    public class AccountOpeningRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string Title { get; set; }    
        [XmlElement]
        public string mobile { get; set; }
        [XmlElement]
        public string FirstName { get; set; }
        [XmlElement]
        public string MiddleName { get; set; }
        [XmlElement]
        public string LastName { get; set; }
        [XmlElement]
        public string DateOfBirth { get; set; }
        [XmlElement]
        public string Gender { get; set; }
        [XmlElement]
        public string AcctType { get; set; }
        [XmlElement]
        public string AddressHome { get; set; }
        [XmlElement]
        public string bvn { get; set; }
        [XmlElement]
        public string email{ get; set; }
        [XmlElement]
        public string productCode { get; set; }
        [XmlElement]
        public string CurrencyCode { get; set; }
    }

    [XmlRoot("IBSResponse")]
    public class AccountOpeningResponseXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string ResponseCode { get; set; }
        [XmlElement]
        public string ResponseText { get; set; }
        [XmlElement]
        public string AccountNo { get; set; }
        [XmlElement]
        public string customerID { get; set; }
        [XmlElement]
        public string AccountName { get; set; }
        [XmlElement]
        public string Currency { get; set; }
    }
}
