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
    public partial class ExistingAccountHolderResponseXML
    {

        private string referenceIDField;

        private string requestTypeField;

        private string responseCodeField;

        private string responseTextField;

        private bool isIDAvailableField;

        private bool isPassportPhotoAvailableField;

        private bool isSignatureAvailableField;

        private bool isUtilityBillAvailableField;

        private bool isReferenceAvailableField;

        private IBSResponseSterlingAccountist[] theFDlistField;

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

        /// <remarks/>
        public bool IsIDAvailable
        {
            get
            {
                return this.isIDAvailableField;
            }
            set
            {
                this.isIDAvailableField = value;
            }
        }

        /// <remarks/>
        public bool IsPassportPhotoAvailable
        {
            get
            {
                return this.isPassportPhotoAvailableField;
            }
            set
            {
                this.isPassportPhotoAvailableField = value;
            }
        }

        /// <remarks/>
        public bool IsSignatureAvailable
        {
            get
            {
                return this.isSignatureAvailableField;
            }
            set
            {
                this.isSignatureAvailableField = value;
            }
        }

        /// <remarks/>
        public bool IsUtilityBillAvailable
        {
            get
            {
                return this.isUtilityBillAvailableField;
            }
            set
            {
                this.isUtilityBillAvailableField = value;
            }
        }

        /// <remarks/>
        public bool IsReferenceAvailable
        {
            get
            {
                return this.isReferenceAvailableField;
            }
            set
            {
                this.isReferenceAvailableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("SterlingAccountist", IsNullable = false)]
        public IBSResponseSterlingAccountist[] TheFDlist
        {
            get
            {
                return this.theFDlistField;
            }
            set
            {
                this.theFDlistField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class IBSResponseSterlingAccountist
    {

        private string firstNameField;

        private string lastNameField;

        private string middleNameField;

        private string emailField;

        private string phoneField;

        private string dateofbirthField;

        private string genderField;

        private string residentialAddressField;

        private string bVNField;

        private string customeridField;

        private string nubanField;

        private bool isIDAvailableField;

        private bool isPassportPhotoAvailableField;

        private bool isSignatureAvailableField;

        private bool isUtilityBillAvailableField;

        private bool isReferenceAvailableField;

        /// <remarks/>
        public string FirstName
        {
            get
            {
                return this.firstNameField;
            }
            set
            {
                this.firstNameField = value;
            }
        }

        /// <remarks/>
        public string LastName
        {
            get
            {
                return this.lastNameField;
            }
            set
            {
                this.lastNameField = value;
            }
        }

        /// <remarks/>
        public string MiddleName
        {
            get
            {
                return this.middleNameField;
            }
            set
            {
                this.middleNameField = value;
            }
        }

        /// <remarks/>
        public string Email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        public string Phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        public string Dateofbirth
        {
            get
            {
                return this.dateofbirthField;
            }
            set
            {
                this.dateofbirthField = value;
            }
        }

        /// <remarks/>
        public string Gender
        {
            get
            {
                return this.genderField;
            }
            set
            {
                this.genderField = value;
            }
        }

        /// <remarks/>
        public string ResidentialAddress
        {
            get
            {
                return this.residentialAddressField;
            }
            set
            {
                this.residentialAddressField = value;
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
        public string customerid
        {
            get
            {
                return this.customeridField;
            }
            set
            {
                this.customeridField = value;
            }
        }

        /// <remarks/>
        public string nuban
        {
            get
            {
                return this.nubanField;
            }
            set
            {
                this.nubanField = value;
            }
        }

        /// <remarks/>
        public bool IsIDAvailable
        {
            get
            {
                return this.isIDAvailableField;
            }
            set
            {
                this.isIDAvailableField = value;
            }
        }

        /// <remarks/>
        public bool IsPassportPhotoAvailable
        {
            get
            {
                return this.isPassportPhotoAvailableField;
            }
            set
            {
                this.isPassportPhotoAvailableField = value;
            }
        }

        /// <remarks/>
        public bool IsSignatureAvailable
        {
            get
            {
                return this.isSignatureAvailableField;
            }
            set
            {
                this.isSignatureAvailableField = value;
            }
        }

        /// <remarks/>
        public bool IsUtilityBillAvailable
        {
            get
            {
                return this.isUtilityBillAvailableField;
            }
            set
            {
                this.isUtilityBillAvailableField = value;
            }
        }

        /// <remarks/>
        public bool IsReferenceAvailable
        {
            get
            {
                return this.isReferenceAvailableField;
            }
            set
            {
                this.isReferenceAvailableField = value;
            }
        }
    }
}
