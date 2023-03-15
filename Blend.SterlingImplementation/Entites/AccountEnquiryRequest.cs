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
    public class AccountEnquiryRequest
    {

        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string FromAccount { get; set; }
    }

    [Serializable]

    [XmlRoot("IBSRequest")]
    public class CustomerAccountRequest
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string customerID { get; set; }
    }

    [Serializable]
    [XmlRoot("IBSRequest")]
    public class XMLinterBankNameEnqRequest
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string ToAccount { get; set; }
        [XmlElement]
        public string DestinationBankCode { get; set; }
    }

    [Serializable]
    [XmlRoot("IBSRequest")]
    public class XMLBalanceEnqRequest
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string Account { get; set; }
    }
}
