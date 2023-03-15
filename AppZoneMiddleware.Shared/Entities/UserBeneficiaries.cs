using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    public class UserBeneficiaries
    {
        [Key]
        [Required]
        public string CustomerID { get; set; }
        public string Beneficiaries { get; set; }
    }
    public class Beneficiary
    {
        [Required]
        public string Name { get; set; }
        [Key]
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public string BankCode { get; set; }
        [Required]
        public string BankName { get; set; }
    }
}
