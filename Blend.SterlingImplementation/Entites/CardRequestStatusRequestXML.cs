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
    public partial class CardRequestStatusRequestXML
    {

        private string referenceIDField;

        private string requestTypeField;

        private string accountIdField;

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
        public string AccountId
        {
            get
            {
                return this.accountIdField;
            }
            set
            {
                this.accountIdField = value;
            }
        }
    }


}
