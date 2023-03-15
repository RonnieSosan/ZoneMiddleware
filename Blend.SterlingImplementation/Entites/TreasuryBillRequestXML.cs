using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blend.SterlingImplementation.Entites
{
    [XmlRoot("IBSRequest")]
    public class TreasuryBillRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string NUBAN { get; set; }
        [XmlElement]
        public string tenor { get; set; }
        [XmlElement]
        public string f_value { get; set; }
    }
}
