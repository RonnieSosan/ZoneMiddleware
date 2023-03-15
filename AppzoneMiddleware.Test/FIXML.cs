using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AppzoneMiddleware.Test
{

    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.finacle.com/fixml")]
    [XmlRootAttribute(Namespace = "http://www.finacle.com/fixml", IsNullable = false)]
    public partial class FIXML
    {
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string schemaLocation = "http://www.finacle.com/fixml RetCustAdd.xsd";

        private FIXMLHeader headerField;

        private FIXMLBody bodyField;

        /// <remarks/>
        public FIXMLHeader Header
        {
            get
            {
                return this.headerField;
            }
            set
            {
                this.headerField = value;
            }
        }

        /// <remarks/>
        public FIXMLBody Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLHeader
    {

        private FIXMLHeaderRequestHeader requestHeaderField;

        /// <remarks/>
        public FIXMLHeaderRequestHeader RequestHeader
        {
            get
            {
                return this.requestHeaderField;
            }
            set
            {
                this.requestHeaderField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLHeaderRequestHeader
    {

        private FIXMLHeaderRequestHeaderMessageKey messageKeyField;

        private FIXMLHeaderRequestHeaderRequestMessageInfo requestMessageInfoField;

        private FIXMLHeaderRequestHeaderSecurity securityField;

        /// <remarks/>
        public FIXMLHeaderRequestHeaderMessageKey MessageKey
        {
            get
            {
                return this.messageKeyField;
            }
            set
            {
                this.messageKeyField = value;
            }
        }

        /// <remarks/>
        public FIXMLHeaderRequestHeaderRequestMessageInfo RequestMessageInfo
        {
            get
            {
                return this.requestMessageInfoField;
            }
            set
            {
                this.requestMessageInfoField = value;
            }
        }

        /// <remarks/>
        public FIXMLHeaderRequestHeaderSecurity Security
        {
            get
            {
                return this.securityField;
            }
            set
            {
                this.securityField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLHeaderRequestHeaderMessageKey
    {

        private string requestUUIDField;

        private string serviceRequestIdField;

        private decimal serviceRequestVersionField;

        private string channelIdField;

        private object languageIdField;

        /// <remarks/>
        public string RequestUUID
        {
            get
            {
                return this.requestUUIDField;
            }
            set
            {
                this.requestUUIDField = value;
            }
        }

        /// <remarks/>
        public string ServiceRequestId
        {
            get
            {
                return this.serviceRequestIdField;
            }
            set
            {
                this.serviceRequestIdField = value;
            }
        }

        /// <remarks/>
        public decimal ServiceRequestVersion
        {
            get
            {
                return this.serviceRequestVersionField;
            }
            set
            {
                this.serviceRequestVersionField = value;
            }
        }

        /// <remarks/>
        public string ChannelId
        {
            get
            {
                return this.channelIdField;
            }
            set
            {
                this.channelIdField = value;
            }
        }

        /// <remarks/>
        public object LanguageId
        {
            get
            {
                return this.languageIdField;
            }
            set
            {
                this.languageIdField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLHeaderRequestHeaderRequestMessageInfo
    {

        private object bankIdField;

        private object timeZoneField;

        private object entityIdField;

        private object entityTypeField;

        private object armCorrelationIdField;

        private object messageDateTimeField;

        /// <remarks/>
        public object BankId
        {
            get
            {
                return this.bankIdField;
            }
            set
            {
                this.bankIdField = value;
            }
        }

        /// <remarks/>
        public object TimeZone
        {
            get
            {
                return this.timeZoneField;
            }
            set
            {
                this.timeZoneField = value;
            }
        }

        /// <remarks/>
        public object EntityId
        {
            get
            {
                return this.entityIdField;
            }
            set
            {
                this.entityIdField = value;
            }
        }

        /// <remarks/>
        public object EntityType
        {
            get
            {
                return this.entityTypeField;
            }
            set
            {
                this.entityTypeField = value;
            }
        }

        /// <remarks/>
        public object ArmCorrelationId
        {
            get
            {
                return this.armCorrelationIdField;
            }
            set
            {
                this.armCorrelationIdField = value;
            }
        }

        /// <remarks/>
        public object MessageDateTime
        {
            get
            {
                return this.messageDateTimeField;
            }
            set
            {
                this.messageDateTimeField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLHeaderRequestHeaderSecurity
    {

        private FIXMLHeaderRequestHeaderSecurityToken tokenField;

        private object fICertTokenField;

        private object realUserLoginSessionIdField;

        private object realUserField;

        private object realUserPwdField;

        private object sSOTransferTokenField;

        /// <remarks/>
        public FIXMLHeaderRequestHeaderSecurityToken Token
        {
            get
            {
                return this.tokenField;
            }
            set
            {
                this.tokenField = value;
            }
        }

        /// <remarks/>
        public object FICertToken
        {
            get
            {
                return this.fICertTokenField;
            }
            set
            {
                this.fICertTokenField = value;
            }
        }

        /// <remarks/>
        public object RealUserLoginSessionId
        {
            get
            {
                return this.realUserLoginSessionIdField;
            }
            set
            {
                this.realUserLoginSessionIdField = value;
            }
        }

        /// <remarks/>
        public object RealUser
        {
            get
            {
                return this.realUserField;
            }
            set
            {
                this.realUserField = value;
            }
        }

        /// <remarks/>
        public object RealUserPwd
        {
            get
            {
                return this.realUserPwdField;
            }
            set
            {
                this.realUserPwdField = value;
            }
        }

        /// <remarks/>
        public object SSOTransferToken
        {
            get
            {
                return this.sSOTransferTokenField;
            }
            set
            {
                this.sSOTransferTokenField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLHeaderRequestHeaderSecurityToken
    {

        private FIXMLHeaderRequestHeaderSecurityTokenPasswordToken passwordTokenField;

        /// <remarks/>
        public FIXMLHeaderRequestHeaderSecurityTokenPasswordToken PasswordToken
        {
            get
            {
                return this.passwordTokenField;
            }
            set
            {
                this.passwordTokenField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLHeaderRequestHeaderSecurityTokenPasswordToken
    {

        private object userIdField;

        private object passwordField;

        /// <remarks/>
        public object UserId
        {
            get
            {
                return this.userIdField;
            }
            set
            {
                this.userIdField = value;
            }
        }

        /// <remarks/>
        public object Password
        {
            get
            {
                return this.passwordField;
            }
            set
            {
                this.passwordField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBody
    {

        private FIXMLBodyRetCustAddRequest retCustAddRequestField;

        /// <remarks/>
        public FIXMLBodyRetCustAddRequest RetCustAddRequest
        {
            get
            {
                return this.retCustAddRequestField;
            }
            set
            {
                this.retCustAddRequestField = value;
            }
        }
    }

    public partial class FIXMLBodyRetCustAddRequest
    {

        private FIXMLBodyRetCustAddRequestRetCustAddRq retCustAddRqField;

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRq RetCustAddRq
        {
            get
            {
                return this.retCustAddRqField;
            }
            set
            {
                this.retCustAddRqField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRq
    {

        private FIXMLBodyRetCustAddRequestRetCustAddRqCustDtls custDtlsField;

        private FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtls relatedDtlsField;

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqCustDtls CustDtls
        {
            get
            {
                return this.custDtlsField;
            }
            set
            {
                this.custDtlsField = value;
            }
        }

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtls RelatedDtls
        {
            get
            {
                return this.relatedDtlsField;
            }
            set
            {
                this.relatedDtlsField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqCustDtls
    {

        private FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustData custDataField;

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustData CustData
        {
            get
            {
                return this.custDataField;
            }
            set
            {
                this.custDataField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustData
    {

        private FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataAddrDtls addrDtlsField;

        private string birthDtField;

        private string birthMonthField;

        private string birthYearField;

        private string createdBySystemIdField;

        private System.DateTime dateOfBirthField;

        private string custHealthCodeField;

        private string languageField;

        private string lastNameField;

        private string middleNameField;

        private string isMinorField;

        private string isCustNREField;

        private string defaultAddrTypeField;

        private string genderField;

        private string managerField;

        private string nativeLanguageCodeField;

        private string occupationField;

        private FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataOperationsReasonCode operationsReasonCodeField;

        private FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataPhoneEmailDtls phoneEmailDtlsField;

        private string prefNameField;

        private string prefPhoneTypeField;

        private string primarySolIdField;

        private System.DateTime passportExpDtField;

        private System.DateTime passportIssueDtField;

        private System.DateTime relationshipOpeningDtField;

        private string salutationField;

        private string secondaryRMIdField;

        private string segmentationClassField;

        private string staffFlagField;

        private System.DateTime startDtField;

        private string subSegmentField;

        private string taxDeductionTableField;

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataAddrDtls AddrDtls
        {
            get
            {
                return this.addrDtlsField;
            }
            set
            {
                this.addrDtlsField = value;
            }
        }

        /// <remarks/>
        public string BirthDt
        {
            get
            {
                return this.birthDtField;
            }
            set
            {
                this.birthDtField = value;
            }
        }

        /// <remarks/>
        public string BirthMonth
        {
            get
            {
                return this.birthMonthField;
            }
            set
            {
                this.birthMonthField = value;
            }
        }

        /// <remarks/>
        public string BirthYear
        {
            get
            {
                return this.birthYearField;
            }
            set
            {
                this.birthYearField = value;
            }
        }

        /// <remarks/>
        public string CreatedBySystemId
        {
            get
            {
                return this.createdBySystemIdField;
            }
            set
            {
                this.createdBySystemIdField = value;
            }
        }

        /// <remarks/>
        public System.DateTime DateOfBirth
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
        public string CustHealthCode
        {
            get
            {
                return this.custHealthCodeField;
            }
            set
            {
                this.custHealthCodeField = value;
            }
        }

        /// <remarks/>
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
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
        public string IsMinor
        {
            get
            {
                return this.isMinorField;
            }
            set
            {
                this.isMinorField = value;
            }
        }

        /// <remarks/>
        public string IsCustNRE
        {
            get
            {
                return this.isCustNREField;
            }
            set
            {
                this.isCustNREField = value;
            }
        }

        /// <remarks/>
        public string DefaultAddrType
        {
            get
            {
                return this.defaultAddrTypeField;
            }
            set
            {
                this.defaultAddrTypeField = value;
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
        public string Manager
        {
            get
            {
                return this.managerField;
            }
            set
            {
                this.managerField = value;
            }
        }

        /// <remarks/>
        public string NativeLanguageCode
        {
            get
            {
                return this.nativeLanguageCodeField;
            }
            set
            {
                this.nativeLanguageCodeField = value;
            }
        }

        /// <remarks/>
        public string Occupation
        {
            get
            {
                return this.occupationField;
            }
            set
            {
                this.occupationField = value;
            }
        }

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataOperationsReasonCode OperationsReasonCode
        {
            get
            {
                return this.operationsReasonCodeField;
            }
            set
            {
                this.operationsReasonCodeField = value;
            }
        }

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataPhoneEmailDtls PhoneEmailDtls
        {
            get
            {
                return this.phoneEmailDtlsField;
            }
            set
            {
                this.phoneEmailDtlsField = value;
            }
        }

        /// <remarks/>
        public string PrefName
        {
            get
            {
                return this.prefNameField;
            }
            set
            {
                this.prefNameField = value;
            }
        }

        /// <remarks/>
        public string PrefPhoneType
        {
            get
            {
                return this.prefPhoneTypeField;
            }
            set
            {
                this.prefPhoneTypeField = value;
            }
        }

        /// <remarks/>
        public string PrimarySolId
        {
            get
            {
                return this.primarySolIdField;
            }
            set
            {
                this.primarySolIdField = value;
            }
        }

        /// <remarks/>
        public System.DateTime PassportExpDt
        {
            get
            {
                return this.passportExpDtField;
            }
            set
            {
                this.passportExpDtField = value;
            }
        }

        /// <remarks/>
        public System.DateTime PassportIssueDt
        {
            get
            {
                return this.passportIssueDtField;
            }
            set
            {
                this.passportIssueDtField = value;
            }
        }

        /// <remarks/>
        public System.DateTime RelationshipOpeningDt
        {
            get
            {
                return this.relationshipOpeningDtField;
            }
            set
            {
                this.relationshipOpeningDtField = value;
            }
        }

        /// <remarks/>
        public string Salutation
        {
            get
            {
                return this.salutationField;
            }
            set
            {
                this.salutationField = value;
            }
        }

        /// <remarks/>
        public string SecondaryRMId
        {
            get
            {
                return this.secondaryRMIdField;
            }
            set
            {
                this.secondaryRMIdField = value;
            }
        }

        /// <remarks/>
        public string SegmentationClass
        {
            get
            {
                return this.segmentationClassField;
            }
            set
            {
                this.segmentationClassField = value;
            }
        }

        /// <remarks/>
        public string StaffFlag
        {
            get
            {
                return this.staffFlagField;
            }
            set
            {
                this.staffFlagField = value;
            }
        }

        /// <remarks/>
        public System.DateTime StartDt
        {
            get
            {
                return this.startDtField;
            }
            set
            {
                this.startDtField = value;
            }
        }

        /// <remarks/>
        public string SubSegment
        {
            get
            {
                return this.subSegmentField;
            }
            set
            {
                this.subSegmentField = value;
            }
        }

        /// <remarks/>
        public string TaxDeductionTable
        {
            get
            {
                return this.taxDeductionTableField;
            }
            set
            {
                this.taxDeductionTableField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataAddrDtls
    {

        private string addrLine1Field;

        private string addrLine2Field;

        private string addrLine3Field;

        private string addrCategoryField;

        private string cityField;

        private string countryField;

        private string holdMailFlagField;

        private string houseNumField;

        private string prefAddrField;

        private string prefFormatField;

        private string startDtField;

        private string stateField;

        private string streetNameField;

        private string streetNumField;

        private string postalCodeField;

        /// <remarks/>
        public string AddrLine1
        {
            get
            {
                return this.addrLine1Field;
            }
            set
            {
                this.addrLine1Field = value;
            }
        }

        /// <remarks/>
        public string AddrLine2
        {
            get
            {
                return this.addrLine2Field;
            }
            set
            {
                this.addrLine2Field = value;
            }
        }

        /// <remarks/>
        public string AddrLine3
        {
            get
            {
                return this.addrLine3Field;
            }
            set
            {
                this.addrLine3Field = value;
            }
        }

        /// <remarks/>
        public string AddrCategory
        {
            get
            {
                return this.addrCategoryField;
            }
            set
            {
                this.addrCategoryField = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string HoldMailFlag
        {
            get
            {
                return this.holdMailFlagField;
            }
            set
            {
                this.holdMailFlagField = value;
            }
        }

        /// <remarks/>
        public string HouseNum
        {
            get
            {
                return this.houseNumField;
            }
            set
            {
                this.houseNumField = value;
            }
        }

        /// <remarks/>
        public string PrefAddr
        {
            get
            {
                return this.prefAddrField;
            }
            set
            {
                this.prefAddrField = value;
            }
        }

        /// <remarks/>
        public string PrefFormat
        {
            get
            {
                return this.prefFormatField;
            }
            set
            {
                this.prefFormatField = value;
            }
        }

        /// <remarks/>
        public string StartDt
        {
            get
            {
                return this.startDtField;
            }
            set
            {
                this.startDtField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string StreetName
        {
            get
            {
                return this.streetNameField;
            }
            set
            {
                this.streetNameField = value;
            }
        }

        /// <remarks/>
        public string StreetNum
        {
            get
            {
                return this.streetNumField;
            }
            set
            {
                this.streetNumField = value;
            }
        }

        /// <remarks/>
        public string PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataOperationsReasonCode
    {

        private System.DateTime expDtField;

        /// <remarks/>
        public System.DateTime ExpDt
        {
            get
            {
                return this.expDtField;
            }
            set
            {
                this.expDtField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataPhoneEmailDtls
    {

        private string emailField;

        private System.DateTime endDtField;

        private string phoneEmailTypeField;

        private string phoneNumField;

        private string phoneNumCityCodeField;

        private string phoneNumCountryCodeField;

        private ulong phoneNumLocalCodeField;

        private string phoneOrEmailField;

        private string prefFlagField;

        private System.DateTime startDtField;

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
        public System.DateTime EndDt
        {
            get
            {
                return this.endDtField;
            }
            set
            {
                this.endDtField = value;
            }
        }

        /// <remarks/>
        public string PhoneEmailType
        {
            get
            {
                return this.phoneEmailTypeField;
            }
            set
            {
                this.phoneEmailTypeField = value;
            }
        }

        /// <remarks/>
        public string PhoneNum
        {
            get
            {
                return this.phoneNumField;
            }
            set
            {
                this.phoneNumField = value;
            }
        }

        /// <remarks/>
        public string PhoneNumCityCode
        {
            get
            {
                return this.phoneNumCityCodeField;
            }
            set
            {
                this.phoneNumCityCodeField = value;
            }
        }

        /// <remarks/>
        public string PhoneNumCountryCode
        {
            get
            {
                return this.phoneNumCountryCodeField;
            }
            set
            {
                this.phoneNumCountryCodeField = value;
            }
        }

        /// <remarks/>
        public ulong PhoneNumLocalCode
        {
            get
            {
                return this.phoneNumLocalCodeField;
            }
            set
            {
                this.phoneNumLocalCodeField = value;
            }
        }

        /// <remarks/>
        public string PhoneOrEmail
        {
            get
            {
                return this.phoneOrEmailField;
            }
            set
            {
                this.phoneOrEmailField = value;
            }
        }

        /// <remarks/>
        public string PrefFlag
        {
            get
            {
                return this.prefFlagField;
            }
            set
            {
                this.prefFlagField = value;
            }
        }

        /// <remarks/>
        public System.DateTime StartDt
        {
            get
            {
                return this.startDtField;
            }
            set
            {
                this.startDtField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtls
    {

        private FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsDemographicData demographicDataField;

        private FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsEntityDoctData entityDoctDataField;

        private FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsPsychographicData psychographicDataField;

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsDemographicData DemographicData
        {
            get
            {
                return this.demographicDataField;
            }
            set
            {
                this.demographicDataField = value;
            }
        }

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsEntityDoctData EntityDoctData
        {
            get
            {
                return this.entityDoctDataField;
            }
            set
            {
                this.entityDoctDataField = value;
            }
        }

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsPsychographicData PsychographicData
        {
            get
            {
                return this.psychographicDataField;
            }
            set
            {
                this.psychographicDataField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsDemographicData
    {

        private string maritalStatusField;

        private string nationalityField;

        /// <remarks/>
        public string MaritalStatus
        {
            get
            {
                return this.maritalStatusField;
            }
            set
            {
                this.maritalStatusField = value;
            }
        }

        /// <remarks/>
        public string Nationality
        {
            get
            {
                return this.nationalityField;
            }
            set
            {
                this.nationalityField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsEntityDoctData
    {

        private string countryOfIssueField;

        private string docCodeField;

        private string expDtField;

        private string issueDtField;

        private string typeCodeField;

        private string placeOfIssueField;

        private string referenceNumField;

        /// <remarks/>
        public string CountryOfIssue
        {
            get
            {
                return this.countryOfIssueField;
            }
            set
            {
                this.countryOfIssueField = value;
            }
        }

        /// <remarks/>
        public string DocCode
        {
            get
            {
                return this.docCodeField;
            }
            set
            {
                this.docCodeField = value;
            }
        }

        /// <remarks/>
        public string ExpDt
        {
            get
            {
                return this.expDtField;
            }
            set
            {
                this.expDtField = value;
            }
        }

        /// <remarks/>
        public string IssueDt
        {
            get
            {
                return this.issueDtField;
            }
            set
            {
                this.issueDtField = value;
            }
        }

        /// <remarks/>
        public string TypeCode
        {
            get
            {
                return this.typeCodeField;
            }
            set
            {
                this.typeCodeField = value;
            }
        }

        /// <remarks/>
        public string PlaceOfIssue
        {
            get
            {
                return this.placeOfIssueField;
            }
            set
            {
                this.placeOfIssueField = value;
            }
        }

        /// <remarks/>
        public string ReferenceNum
        {
            get
            {
                return this.referenceNumField;
            }
            set
            {
                this.referenceNumField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsPsychographicData
    {

        private string custCurrCodeField;

        private FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsPsychographicDataPsychographMiscData psychographMiscDataField;

        /// <remarks/>
        public string CustCurrCode
        {
            get
            {
                return this.custCurrCodeField;
            }
            set
            {
                this.custCurrCodeField = value;
            }
        }

        /// <remarks/>
        public FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsPsychographicDataPsychographMiscData PsychographMiscData
        {
            get
            {
                return this.psychographMiscDataField;
            }
            set
            {
                this.psychographMiscDataField = value;
            }
        }
    }

    /// <remarks/>
    public partial class FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsPsychographicDataPsychographMiscData
    {

        private string strText10Field;

        private string typeField;

        /// <remarks/>
        public string StrText10
        {
            get
            {
                return this.strText10Field;
            }
            set
            {
                this.strText10Field = value;
            }
        }

        /// <remarks/>
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }
}