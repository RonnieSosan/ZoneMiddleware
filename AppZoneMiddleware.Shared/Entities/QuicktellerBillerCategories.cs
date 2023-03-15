using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Entities
{
    /// <summary>
    /// Quickteller list of biller categories
    /// </summary>
    public class QuicktellerBillerCategories : InterswitchBaseResponse
    {
        /// <summary>
        /// List of biller categories
        /// </summary>
        public List<QuicktellerBillerCategory> categorys { get; set; }
    }

    /// <summary>
    /// quickteller biller category details
    /// </summary>
    public class QuicktellerBillerCategory
    {
        public string categoryid { get; set; }
        public string categoryname { get; set; }
        public string categorydescription { get; set; }
    }

}
