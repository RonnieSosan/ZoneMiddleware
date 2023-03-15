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
    public class BVNValidationRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string Bvn { get; set; }
    }

    [Serializable]

    [XmlRoot("IBSResponse")]
    public class BVNValidationResponseXML
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
        public string Bvn { get; set; }
        [XmlElement]
        public string FirstName { get; set; }
        [XmlElement]
        public string MiddleName { get; set; }
        [XmlElement]
        public string LastName { get; set; }
        [XmlElement]
        public string DateOfBirth { get; set; }
        [XmlElement]
        public string PhoneNumber { get; set; }
        [XmlElement]
        public string RegistrationDate { get; set; }
    }


}
