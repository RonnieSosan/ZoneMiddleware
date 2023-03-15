using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blend.SterlingImplementation.Entites
{
    [XmlRoot("IBSResponse")]
    public class SterlingBaseResponse
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

    [XmlRoot("CardResponse")]
    public class CardBaseResponse
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
