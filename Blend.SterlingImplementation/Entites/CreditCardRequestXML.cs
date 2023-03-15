using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blend.SterlingImplementation.Entites
{
    [XmlRoot("IBSRequest")]
    public class CreditCardRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }//103364
        [XmlElement]
        public string RequestType { get; set; }//935
        [XmlElement]
        public string AccountId { get; set; }//908389
        [XmlElement]
        public string Title { get; set; }//Mr
        [XmlElement]
        public string LastName { get; set; }//ALAO
        [XmlElement]
        public string FirstName { get; set; } //RAMON
        [XmlElement]
        public string IsMasterCard { get; set; }//1
        [XmlElement]
        public string IsVerveCard { get; set; }//0

    }

    [XmlRoot("IBSRequest")]
    public class CreditCkeckRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }//103364
        [XmlElement]
        public string RequestType { get; set; }//935
        [XmlElement]
        public string dob { get; set; }//29-JUN-1964
        [XmlElement]
        public string gender { get; set; }//001
        [XmlElement]
        public string nuban { get; set; } //001256372
        [XmlElement]
        public string BVN { get; set; }//22268769282
        [XmlElement]
        public string mobile { get; set; }//07088122707

    }
}
