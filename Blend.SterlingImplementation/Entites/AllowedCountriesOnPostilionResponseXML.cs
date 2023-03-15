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
    public class AllowedCountriesOnPostilionResponseXML
    {

        private string referenceIDField;

        private string requestTypeField;

        private AllowedCountryOnPostillionXML[] allowedCountriesField;

        private string responseTextField;

        private string responseCodeField;

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
        [System.Xml.Serialization.XmlArrayItemAttribute("GetAllowedCountriesOnPostillionList", IsNullable = false)]
        public AllowedCountryOnPostillionXML[] AllowedCountries
        {
            get
            {
                return this.allowedCountriesField;
            }
            set
            {
                this.allowedCountriesField = value;
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
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AllowedCountryOnPostillionXML
    {

        private string country_NameField;

        private string iSO_2_codeField;

        private string alpha_codeField;

        private string currency_codeField;

        private string iSO_3_CodeField;

        /// <remarks/>
        public string Country_Name
        {
            get
            {
                return this.country_NameField;
            }
            set
            {
                this.country_NameField = value;
            }
        }

        /// <remarks/>
        public string ISO_2_code
        {
            get
            {
                return this.iSO_2_codeField;
            }
            set
            {
                this.iSO_2_codeField = value;
            }
        }

        /// <remarks/>
        public string alpha_code
        {
            get
            {
                return this.alpha_codeField;
            }
            set
            {
                this.alpha_codeField = value;
            }
        }

        /// <remarks/>
        public string currency_code
        {
            get
            {
                return this.currency_codeField;
            }
            set
            {
                this.currency_codeField = value;
            }
        }

        /// <remarks/>
        public string ISO_3_Code
        {
            get
            {
                return this.iSO_3_CodeField;
            }
            set
            {
                this.iSO_3_CodeField = value;
            }
        }
    }

}
