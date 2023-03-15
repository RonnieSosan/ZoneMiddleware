using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    public class WalletDetails:sPayBaseResponse
    {
        public string message { get; set; }
        public string response { get; set; }
        public WalletData data { get; set; }
    }
}
