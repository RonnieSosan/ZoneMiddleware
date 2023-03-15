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
    public partial class AllowedCountriesOnPostilionRequestXML
    {

        private string referenceIDField;

        private string requestTypeField;

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
    }


}
