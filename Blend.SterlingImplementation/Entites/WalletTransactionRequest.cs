﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    [JsonObject]
    public class WalletTransactionRequest : sPayBaseRequest
    {
        public string amt { get; set; }
        public string tellerid { get; set; }
        public string frmacct { get; set; }
        public string toacct { get; set; }
        public string exp_code { get; set; }
        public string paymentRef { get; set; }
        public string remarks { get; set; }
    }

}
