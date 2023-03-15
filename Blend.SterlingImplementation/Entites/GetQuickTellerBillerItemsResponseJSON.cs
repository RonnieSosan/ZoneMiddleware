using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    public class GetQuickTellerBillerItemsResponseJSON
    {
        public string message { get; set; }
        public string response { get; set; }
        public GetQuickTellerBillerItemsResponseJSONData data { get; set; }
    }
    
    public class GetQuickTellerBillerItemsResponseJSONData
    {
        public string billers { get; set; }
        public string status { get; set; }
    }

}
