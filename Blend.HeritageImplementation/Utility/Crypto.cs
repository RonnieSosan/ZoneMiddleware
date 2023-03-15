using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blend.HeritageImplementation.Utility
{
    public class Crypto
    {
        public string DecryptString(string base64CipherText, string Key)
        {
            string decryptedString = string.Empty;
            try
            {
                var plainCypherText = Convert.FromBase64String(base64CipherText);
                var aes = new AesManaged();
                aes.Key = Convert.FromBase64String(Key);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                var cryptor = aes.CreateDecryptor();
                var cypher = cryptor.TransformFinalBlock(plainCypherText, 0, plainCypherText.Length);
                decryptedString = Encoding.UTF8.GetString(cypher);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return decryptedString;
        }

        public string EncryptString(string plainText, string Key)
        {
            string encryptedString = string.Empty;
            try
            {
                var aes = new AesManaged();
                aes.Key = Convert.FromBase64String(Key);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                var cryptor = aes.CreateEncryptor();
                var plainTextByte = Encoding.UTF8.GetBytes(plainText);
                var cypher = cryptor.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
                encryptedString = Convert.ToBase64String(cypher);
            }
            catch (Exception)
            {

                throw;
            }
            return encryptedString;
        }
    }
}
