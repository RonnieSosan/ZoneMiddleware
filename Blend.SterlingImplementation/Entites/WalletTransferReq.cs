using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    public class WalletTransferReq
    {
        public string Referenceid { get; set; }
        public string RequestType { get; set; }
        public string Translocation { get; set; }
        public string amt { get; set; }
        public string tellerid { get; set; }
        public string frmacct { get; set; }
        public string toacct { get; set; }
        public string exp_code { get; set; }
        public string paymentRef { get; set; }
        public string remarks { get; set; }
    }
}
