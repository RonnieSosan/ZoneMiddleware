using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities.AuthenticationProxy
{
    public class ApiSecuritySpec
    {
        [Key]
        public string URL { get; set; }
        public string SecurityMetric { get; set; }
        public string WSDLRef { get; set; }

        public bool isSoap { get; set; }
        public string ServiceDescription { get; set; }
    }
}
