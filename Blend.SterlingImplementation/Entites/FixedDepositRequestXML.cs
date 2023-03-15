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
    public class FixedDepositRequestXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; } //926
        [XmlElement]
        public string changeperiod { get; set; } //3M
        [XmlElement]
        public string payinacct { get; set; } //>0027087964
        [XmlElement]
        public string payoutacct3 { get; set; } //0025514745
        [XmlElement]
        public string payoutacct2 { get; set; } //0025514745
        [XmlElement]
        public string payoutacct1 { get; set; } //0025514745
        [XmlElement]
        public string customerid { get; set; } //008976
        [XmlElement]
        public string currency { get; set; } //NGN
        [XmlElement]
        public string amount { get; set; } //1000000
        [XmlElement]
        public string rate { get; set; } //12
        [XmlElement]
        public string effectivedate { get; set; } //2017-10-31
    }

    [XmlRoot("IBSResponse")]
    public class FixedDepositResponseXML
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
        public string arrangementid { get; set; }
    }

    [Serializable]

    [XmlRoot("IBSRequest")]
    public class TerminateFixedDepositXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; } //925
        [XmlElement]
        public string arrangementid { get; set; }
    }
}
