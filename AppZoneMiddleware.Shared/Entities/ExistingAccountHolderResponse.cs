using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    [JsonObject]
    public class ExistingAccountHolderResponse : BaseResponse
    {        
        public ExistingSterlingAccounts[] TheAccountsList { get; set; }
    }

    [JsonObject]
    public class ExistingSterlingAccounts
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string EMail { get; set; }

        public string PhoneNumber { get; set; }

        public string DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string ResidentialAddress { get; set; }

        public string BVN { get; set; }

        public string CustomerID { get; set; }

        public string NUBAN { get; set; }

        public bool IsIDAvailable { get; set; }

        public bool IsPassportPhotoAvailable { get; set; }

        public bool IsSignatureAvailable { get; set; }

        public bool IsUtilityBillAvailable { get; set; }

        public bool IsReferenceAvailable { get; set; }
    }
}
