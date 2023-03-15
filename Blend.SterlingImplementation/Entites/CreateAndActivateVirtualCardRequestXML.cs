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
    public partial class CreateAndActivateVirtualCardRequestXML
    {

        private string referenceIDField;

        private string requestTypeField;

        private string appCodeField;

        private string titleField;

        private string mobileField;

        private string firstNameField;

        private string middleNameField;

        private string lastNameField;

        private string dateOfBirthField;

        private string genderField;

        private string addressHomeField;

        private string bvnField;

        private string emailField;

        private string cusnumField;

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
        public string AppCode
        {
            get
            {
                return this.appCodeField;
            }
            set
            {
                this.appCodeField = value;
            }
        }

        /// <remarks/>
        public string Title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        public string mobile
        {
            get
            {
                return this.mobileField;
            }
            set
            {
                this.mobileField = value;
            }
        }

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
        public string DateOfBirth
        {
            get
            {
                return this.dateOfBirthField;
            }
            set
            {
                this.dateOfBirthField = value;
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
        public string AddressHome
        {
            get
            {
                return this.addressHomeField;
            }
            set
            {
                this.addressHomeField = value;
            }
        }

        /// <remarks/>
        public string bvn
        {
            get
            {
                return this.bvnField;
            }
            set
            {
                this.bvnField = value;
            }
        }

        /// <remarks/>
        public string email
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
        public string cusnum
        {
            get
            {
                return this.cusnumField;
            }
            set
            {
                this.cusnumField = value;
            }
        }
    }


}
