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
    public class SMSRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string Msg { get; set; }
        [XmlElement]
        public string gsm { get; set; }
    }

    [XmlRoot("IBSResponse")]
    public class SMSResponseXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string ResponseCode { get; set; }
        [XmlElement]
        public string ResponseText { get; set; }
    }
}
