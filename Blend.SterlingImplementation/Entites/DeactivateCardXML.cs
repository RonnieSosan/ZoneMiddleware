using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "CardRequest", Namespace = "", IsNullable = false)]
    public partial class DeactivateCardXML
    {

        private string reqeuestIDField;

        private string requestTypeField;

        private string accountNumberField;

        private string accountTypeField;

        private string panField;

        /// <remarks/>
        public string ReqeuestID
        {
            get
            {
                return this.reqeuestIDField;
            }
            set
            {
                this.reqeuestIDField = value;
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
        public string AccountNumber
        {
            get
            {
                return this.accountNumberField;
            }
            set
            {
                this.accountNumberField = value;
            }
        }

        /// <remarks/>
        public string AccountType
        {
            get
            {
                return this.accountTypeField;
            }
            set
            {
                this.accountTypeField = value;
            }
        }

        /// <remarks/>
        public string Pan
        {
            get
            {
                return this.panField;
            }
            set
            {
                this.panField = value;
            }
        }
    }

}
