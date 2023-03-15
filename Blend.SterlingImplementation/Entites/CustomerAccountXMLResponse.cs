using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blend.SterlingImplementation.Entites
{
    [Serializable]

    [XmlRoot("IBSResponse")]
    public class AccountEnquiryResponse
    {

        public string AccountNumber { get; set; }
        public string ResponseCode { get; set; }
        public string BranchCode { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string AccountGroup { get; set; }
        public string AccountBalance { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string ResponseText { get; set; }
        public string ProductCode { get; set; }
        public string Currency { get; set; }
        public string Phone { get; set; }
        public CustomerAccountXMLResponse CustomerAccounts { get; set; }
    }
    [Serializable]

    [XmlRoot("IBSResponse")]
    public class CustomerAccountXMLResponse
    {

        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string ResponseCode { get; set; }
        [XmlElement]
        public string ResponseText { get; set; }
        [XmlArray]
        public AccountsByCustomerID[] Records { get; set; }
    }


    [Serializable]
    public class AccountsByCustomerID
    {
        [XmlElement]
        public string NUBAN { get; set; }
        [XmlElement]
        public string BRANCHCODE { get; set; }
        [XmlElement]
        public string CURRENCY { get; set; }
        [XmlElement]
        public string LEDCODE { get; set; }
        [XmlElement]
        public string ACCOUNTNAME { get; set; }
        [XmlElement]
        public string CUSTOMERSTATUS { get; set; }
        [XmlElement]
        public string ACCOUNTGROUP { get; set; }
    }

    [Serializable]

    [XmlRoot("IBSResponse")]
    public class CustomerAccountResponse
    {

        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string ResponseCode { get; set; }
        [XmlElement]
        public string ResponseText { get; set; }
        [XmlArray]
        public AccountsByCustomerID[] Records { get; set; }
    }

    [Serializable]

    [XmlRoot("IBSResponse")]
    public class XMLInterBankNameEnqResponse
    {
        [XmlElement]
        public string SessionID { get; set; }      
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string ResponseCode { get; set; }
        [XmlElement]
        public string ResponseText { get; set; }
    }

    [Serializable]

    [XmlRoot("IBSResponse")]
    public class XMLBalanceEnqResponse
    {
        [XmlElement]
        public string ReferenceID { get; set; }
        [XmlElement]
        public string RequestType { get; set; }
        [XmlElement]
        public string ResponseCode { get; set; }
        [XmlElement]
        public string ResponseText { get; set; }
        [XmlElement]
        public string Account { get; set; }
        [XmlElement]
        public string Available { get; set; }
        [XmlElement]
        public string Book { get; set; }
    }
}
