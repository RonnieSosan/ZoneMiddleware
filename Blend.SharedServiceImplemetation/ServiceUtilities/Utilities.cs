using AppZoneMiddleware.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SharedServiceImplementation.ServiceUtilities
{
    public class Utilities
    {
        public string BinaryToString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                throw new ArgumentNullException("binary");

            if ((binary.Length % 8) != 0)
                throw new ArgumentException("Binary string invalid (must divide by 8)", "binary");

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < binary.Length; i += 8)
            {
                string section = binary.Substring(i, 8);
                int ascii = 0;
                try
                {
                    ascii = Convert.ToInt32(section, 2);
                }
                catch
                {
                    throw new ArgumentException("Binary string contains invalid section: " + section, "binary");
                }
                builder.Append((char)ascii);
            }
            return builder.ToString();
        }

        public string SterlingEncrypt(string val)
        {
            MemoryStream ms = new MemoryStream();
            string rsp = "";
            try
            {
                string sharedkeyval = ""; string sharedvectorval = "";
                sharedkeyval = "000000010000001000000101000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100001100000000110000010100000111000010110000110100011100";

                //"000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100010001";
                sharedkeyval = BinaryToString(sharedkeyval);

                sharedvectorval = "0000000100000010000000110000010100000111000010110000110100000100";

                //"0000000100000010000000110000010100000111000010110000110100010001";
                sharedvectorval = BinaryToString(sharedvectorval);
                byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
                byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

                CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
                cs.Write(toEncrypt, 0, toEncrypt.Length);
                cs.FlushFinalBlock();
            }
            catch
            {
                Logger.LogInfo("SterlingUtility.SterlingEncrypt", "There is an issue with the xml received " + val + " Invalid xml");
                //rsp = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><IBSResponse><ResponseCode>57</ResponseCode><ResponseText>Transaction not permitted to sender</ResponseText></IBSResponse>";
                //rsp = Encrypt(rsp, Appid);
                return rsp;
            }
            return Convert.ToBase64String(ms.ToArray());
        }
        public string SterlingDecrypt(string val)
        {
            MemoryStream ms = new MemoryStream();
            string rsp = "";
            try
            {
                string sharedkeyval = ""; string sharedvectorval = "";

                sharedkeyval = "000000010000001000000101000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100001100000000110000010100000111000010110000110100011100";

                //"000000010000001000000011000001010000011100001011000011010001000100010010000100010000110100001011000001110000001000000100000010000000000100000010000000110000010100000111000010110000110100010001";
                sharedkeyval = BinaryToString(sharedkeyval);

                sharedvectorval = "0000000100000010000000110000010100000111000010110000110100000100";

                //"0000000100000010000000110000010100000111000010110000110100010001";
                sharedvectorval = BinaryToString(sharedvectorval);

                byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
                byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                byte[] toDecrypt = Convert.FromBase64String(val);

                CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);


                cs.Write(toDecrypt, 0, toDecrypt.Length);
                cs.FlushFinalBlock();
            }
            catch
            {
                Logger.LogInfo("SterlingUtility.SterlingDecrypt", "There is an issue with the xml received " + val);
                //rsp = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><IBSResponse><ResponseCode>57</ResponseCode><ResponseText>Transaction not permitted to sender</ResponseText></IBSResponse>";
                //rsp = Encrypt(rsp, Appid);
                return rsp;
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
