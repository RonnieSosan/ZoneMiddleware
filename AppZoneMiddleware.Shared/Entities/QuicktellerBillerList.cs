using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    /// <summary>
    /// List of qucikteller billers 
    /// </summary>
    public class QuicktellerBillerList : InterswitchBaseResponse
    {
        public List<QuickTellerBiller> billers { get; set; }
    }


    /// <summary>
    /// Qucikteller biller details
    /// </summary>
    public class QuickTellerBiller
    {
        public string categoryid { get; set; }
        public string categoryname { get; set; }
        public string categorydescription { get; set; }
        public string billerid { get; set; }
        public string billername { get; set; }
        public string customerfield1 { get; set; }
        public string customerfield2 { get; set; }
        public string shortName { get; set; }
        public List<Paymentitem> paymentitems { get; set; }
    }
}
