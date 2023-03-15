using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("IBSRequest", Namespace = "", IsNullable = false)]
    public partial class ExistingAccountHolderRequestXML
    {

        private string referenceIDField;

        private string requestTypeField;

        private string bVNField;

        private string phoneNoField;

        /// <remarks/>
        public string ReferenceID
        {
            get
            {
                return this.referenceIDField;
            }
            set
            {
                this.referenceIDField = value;
            }
        }

        /// <remarks/>
        public string RequestType
        {
            get
            {
                return this.requestTypeField;
            }
            set
            {
                this.requestTypeField = value;
            }
        }

        /// <remarks/>
        public string BVN
        {
            get
            {
                return this.bVNField;
            }
            set
            {
                this.bVNField = value;
            }
        }

        /// <remarks/>
        public string PhoneNo
        {
            get
            {
                return this.phoneNoField;
            }
            set
            {
                this.phoneNoField = value;
            }
        }
    }


}
