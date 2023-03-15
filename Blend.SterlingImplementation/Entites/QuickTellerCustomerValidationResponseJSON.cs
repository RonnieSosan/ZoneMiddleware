using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    [JsonObject]
    public class QuickTellerCustomerValidationResponseJSON : sPayBaseResponse
    {
        public string message { get; set; }
        public string response { get; set; }
        public QuickTellerCustomerValidationResponseData data { get; set; }
    }

    public class QuickTellerCustomerValidationResponseData
    {
        [Newtonsoft.Json.JsonIgnore]
        public QuickTellerCustomerValidationResponseDetails Details { get; set; }
        public string customerDetails { get; set; }
        public string status { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute("Response", Namespace = "", IsNullable = false)]
    public partial class QuickTellerCustomerValidationResponseDetails
    {

        private string responseCodeField;

        private QuickTellerCustomerValidationResponseResponseCustomerDetails customerField;

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
        public QuickTellerCustomerValidationResponseResponseCustomerDetails Customer
        {
            get
            {
                return this.customerField;
            }
            set
            {
                this.customerField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class QuickTellerCustomerValidationResponseResponseCustomerDetails
    {

        private string customerValidationFieldField;

        private string paymentCodeField;

        private string customerIdField;

        private string withDetailsField;

        private string responseCodeField;

        private string fullNameField;

        private string amountField;

        /// <remarks/>
        public string CustomerValidationField
        {
            get
            {
                return this.customerValidationFieldField;
            }
            set
            {
                this.customerValidationFieldField = value;
            }
        }

        /// <remarks/>
        public string PaymentCode
        {
            get
            {
                return this.paymentCodeField;
            }
            set
            {
                this.paymentCodeField = value;
            }
        }

        /// <remarks/>
        public string CustomerId
        {
            get
            {
                return this.customerIdField;
            }
            set
            {
                this.customerIdField = value;
            }
        }

        /// <remarks/>
        public string WithDetails
        {
            get
            {
                return this.withDetailsField;
            }
            set
            {
                this.withDetailsField = value;
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
        public string FullName
        {
            get
            {
                return this.fullNameField;
            }
            set
            {
                this.fullNameField = value;
            }
        }

        /// <remarks/>
        public string Amount
        {
            get
            {
                return this.amountField;
            }
            set
            {
                this.amountField = value;
            }
        }
    }
}
