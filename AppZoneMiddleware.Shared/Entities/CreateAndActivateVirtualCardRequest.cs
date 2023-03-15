using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class CreateAndActivateVirtualCardRequest : BaseRequest
    {
        public string Title {get; set;}

        public string FirstName {get; set;}

        public string MiddleName {get; set;}

        public string LastName {get; set;}

        public string DateOfBirth {get; set;}

        public string Gender {get; set;}

        public string AddressHome {get; set;}
        
        public string BVN {get; set;}

        public string Email {get; set;}
    }
}
