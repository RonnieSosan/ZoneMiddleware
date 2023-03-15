using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blend.SterlingImplementation.Entites
{
    /// <summary>
    /// For uploading the following: ID, Photo, Utility Bill.
    /// </summary>
    [Serializable]
    [XmlRoot("IBSRequest")]
    public class UploadXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string NUBAN { get; set; }
        [XmlElement]
        public byte[] ImageByte { get; set; }
        [XmlElement]
        public string FileType { get; set; }
    }

    [Serializable]
    [XmlRoot("IBSRequest")]
    public class UploadIDXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string NUBAN { get; set; }
        [XmlElement]
        public byte[] ImageByte { get; set; }
        [XmlElement]
        public string FileType { get; set; }
        [XmlElement]
        public string IDNO { get; set; }
    }

    /// <summary>
    /// For updating customer's BVN.
    /// </summary>
    [Serializable]
    [XmlRoot("IBSRequest")]
    public class UpdateBvnXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string NUBAN { get; set; }
        [XmlElement]
        public string Bvn { get; set; }
    }

    [Serializable]

    [XmlRoot("IBSRequest")]
    public class UpgradeAccountXML
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string newproduct { get; set; }
        [XmlElement]
        public string acctnumber { get; set; }
        [XmlElement]
        public string acctBranch { get; set; }
    }
}
