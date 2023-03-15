using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("IBSResponse", Namespace = "", IsNullable = false)]
    public partial class ActivateCardResponseXML
    {

        private string referenceIDField;

        private string requestTypeField;

        private string responseCodeField;

        private string responseTextField;

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
        public string ResponseCode
        {
            get
            {
                return this.responseCodeField;
            }
            set
            {
                this.responseCodeField = value;
            }
        }

        /// <remarks/>
        public string ResponseText
        {
            get
            {
                return this.responseTextField;
            }
            set
            {
                this.responseTextField = value;
            }
        }
    }


}
