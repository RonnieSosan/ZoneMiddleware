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
    public partial class ActivateCardRequestXML
    {

        private string referenceIDField;

        private string requestTypeField;

        private string panField;

        private string seq_nrField;

        private string selectedPinField;

        private string expField;

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
        public string pan
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

        /// <remarks/>
        public string seq_nr
        {
            get
            {
                return this.seq_nrField;
            }
            set
            {
                this.seq_nrField = value;
            }
        }

        /// <remarks/>
        public string SelectedPin
        {
            get
            {
                return this.selectedPinField;
            }
            set
            {
                this.selectedPinField = value;
            }
        }

        /// <remarks/>
        public string exp
        {
            get
            {
                return this.expField;
            }
            set
            {
                this.expField = value;
            }
        }
    }


}
