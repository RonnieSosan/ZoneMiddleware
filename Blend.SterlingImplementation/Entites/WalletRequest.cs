using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.Entites
{
    [JsonObject]
    public class WalletRequest
    {
        public string Referenceid { get; set; }
        public string RequestType { get; set; }
        public string Translocation { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string MOBILE { get; set; }
        public string GENDER { get; set; }
        public string BIRTHDATE { get; set; }
        public string EMAIL { get; set; }
        public string NATIONALITY { get; set; }
        public string TARGET { get; set; }
        public float SECTOR { get; set; }
        public string ADDR_LINE1 { get; set; }
        public string ADDR_LINE2 { get; set; }
        public string CUST_TYPE { get; set; }
        public string MARITAL_STATUS { get; set; }
        public string TITLE { get; set; }
        public string CUST_STATUS { get; set; }
        public string RESIDENCE { get; set; }
        public string CATEGORYCODE { get; set; }
    }

}
