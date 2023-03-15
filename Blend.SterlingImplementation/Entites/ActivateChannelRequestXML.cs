using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blend.SterlingImplementation.Entites
{
    [Serializable]

    [XmlRoot("CardRequest")]
    public class CardActionRequestXML
    {
        [XmlElement]
        public string ReqeuestID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string AccountNumber { get; set; }
        [XmlElement]
        public string AccountType { get; set; }
        [XmlElement]
        public string Pan { get; set; }

    }
}
