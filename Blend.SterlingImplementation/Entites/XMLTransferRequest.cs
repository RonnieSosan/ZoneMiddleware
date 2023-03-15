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
    public class XMLTransferRequest
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string FromAccount { get; set; }
        [XmlElement]
        public string ToAccount { get; set; }
        [XmlElement]
        public string Amount { get; set; }
        [XmlElement]
        public string PaymentReference { get; set; }
    }

    [Serializable]

    [XmlRoot("IBSRequest")]
    public class XMLIBTransferRequest
    {
        [XmlElement]
        public string SessionID { get; set; }
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string FromAccount { get; set; }
        [XmlElement]
        public string ToAccount { get; set; }
        [XmlElement]
        public string Amount { get; set; }
        [XmlElement]
        public string DestinationBankCode { get; set; }
        [XmlElement]
        public string NEResponse { get; set; }
        [XmlElement]
        public string BenefiName { get; set; }
        [XmlElement]
        public string PaymentReference { get; set; }
    }
}
