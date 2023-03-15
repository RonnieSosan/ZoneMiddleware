using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AppZoneMiddleware.Shared.Extension
{
    public class InterswitchAuthenticationService
    {
        static byte[] Hash(string data)
        {
            var bytes = ConvertToByte(data);
            byte[] hash;
            using (SHA1 shaM = new SHA1Managed())
            {
                hash = shaM.ComputeHash(bytes);
            }
            return hash;
        }

        static byte[] ConvertToByte(string data)
        {
            return Encoding.UTF8.GetBytes(data);
        }

        private static string GetNonce()
        {
            Random rand = new Random(10000);
            int randomNumbers = rand.Next(10, 10);
            long date = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds;
            return string.Format("{0}{1}", randomNumbers, date);
        }

        /// <summary>
        /// Setting authentication headers for HttpRequestMessage
        /// </summary>
        /// <param name="request">HttpRequestMesage object</param>
        /// <param name="path">Interswitch api path</param>
        /// <param name="httpMethod">Http Method</param>
        public static void SetHeaders(HttpRequestMessage request, string path, string httpMethod, string clientIDConfigKey, string SecretKeyConfigKey)
        {
            //Prepare Headers
            Regex reg = new Regex(@"%[a-f0-9]{2}");
            string Nonce = GetNonce();
            string signatureMethod = "SHA1";
            string client_id = System.Configuration.ConfigurationManager.AppSettings[clientIDConfigKey];
            string secretKey = System.Configuration.ConfigurationManager.AppSettings[SecretKeyConfigKey];
            string _path = HttpUtility.UrlEncode(path);
            string EncodedPath = reg.Replace(_path, m => m.Value.ToUpperInvariant());
            string unixTimestamp = Convert.ToString((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            string rawSignature = string.Format("{0}&{1}&{2}&{3}&{4}&{5}", httpMethod, EncodedPath, unixTimestamp, Nonce, client_id, secretKey);
            string signature = Convert.ToBase64String(Hash(rawSignature));
            string authorization = "InterswitchAuth " + Convert.ToBase64String(ConvertToByte(client_id));

            //Set Headers
            request.Headers.Add("Authorization", authorization);
            request.Headers.Add("Signature", signature);
            request.Headers.Add("Nonce", Nonce);
            request.Headers.Add("Timestamp", unixTimestamp);
            request.Headers.Add("SignatureMethod", signatureMethod);
        }

        /// <summary>
        /// setting authentication headers for the HttpClient
        /// </summary>
        /// <param name="request">HttpClient object</param>
        /// <param name="path">interswitch api path</param>
        /// <param name="httpMethod">Http method</param>
        public static void SetHeaders(HttpClient request, string path, string httpMethod, string clientIDConfigKey, string SecretKeyConfigKey)
        {
            //Prepare Headers
            Regex reg = new Regex(@"%[a-f0-9]{2}");
            string Nonce = GetNonce();
            string signatureMethod = "SHA1";
            string client_id = System.Configuration.ConfigurationManager.AppSettings[clientIDConfigKey];
            string secretKey = System.Configuration.ConfigurationManager.AppSettings[SecretKeyConfigKey];
            string _path = HttpUtility.UrlEncode(path);
            string EncodedPath = reg.Replace(_path, m => m.Value.ToUpperInvariant());
            string unixTimestamp = Convert.ToString((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
            string rawSignature = string.Format("{0}&{1}&{2}&{3}&{4}&{5}", httpMethod, EncodedPath, unixTimestamp, Nonce, client_id, secretKey);
            string signature = Convert.ToBase64String(Hash(rawSignature));
            string authorization = "InterswitchAuth " + Convert.ToBase64String(ConvertToByte(client_id));

            //Set Headers
            request.DefaultRequestHeaders.Add("Authorization", authorization);
            request.DefaultRequestHeaders.Add("Signature", signature);
            request.DefaultRequestHeaders.Add("Nonce", Nonce);
            request.DefaultRequestHeaders.Add("Timestamp", unixTimestamp);
            request.DefaultRequestHeaders.Add("SignatureMethod", signatureMethod);
        }
    }
}
