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
    public class AirtimeRequestXML
    {
        public string ReferenceID { get; set; }
        public string RequestType { get; set; } //932
        public string Mobile { get; set; } //2347038951726
        public string Beneficiary { get; set; } //2347036083928
        public string NUBAN { get; set; } //0005969437
        public string Amount { get; set; } //200
        public string NetworkID { get; set; } //1
        public string Type { get; set; } //2
    }
}
