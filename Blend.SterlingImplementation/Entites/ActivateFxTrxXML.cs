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
    public partial class ActivateFxTrxXML
    {

        private string requestIDField;

        private string requestTypeField;

        private string accountNumberField;

        private string accountTypeField;

        private string startDateField;

        private string endDateField;

        private string panField;

        private string countriesField;

        /// <remarks/>
        public string RequestID
        {
            get
            {
                return this.requestIDField;
            }
            set
            {
                this.requestIDField = value;
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
        public string StartDate
        {
            get
            {
                return this.startDateField;
            }
            set
            {
                this.startDateField = value;
            }
        }

        /// <remarks/>
        public string EndDate
        {
            get
            {
                return this.endDateField;
            }
            set
            {
                this.endDateField = value;
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

        /// <remarks/>
        public string Countries
        {
            get
            {
                return this.countriesField;
            }
            set
            {
                this.countriesField = value;
            }
        }
    }

}
