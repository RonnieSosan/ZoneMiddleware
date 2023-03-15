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
    public class BlockFundRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string NUBAN { get; set; }
        [XmlElement]
        public string Amount { get; set; }
    }


    [XmlRoot("IBSResponse")]
    public class BlockFundResponseXML
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
        public string LockID { get; set; }
    }

    [XmlRoot("IBSRequest")]
    public class UnBlockFundRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string LockID { get; set; }
    }


    [XmlRoot("IBSResponse")]
    public class UnBlockFundResponseXML
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
        public string LockID { get; set; }
    }
}
