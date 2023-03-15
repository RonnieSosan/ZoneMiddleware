using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Utility
{
    public class Utils
    {
        /// <summary>
        /// Returns the Implementation Bank, based on value specified in the AppSettings section of the config file using "ImplementationBank" as key.
        /// </summary>
        public static ImplementationBank ImplementationClientBank
        {
            get
            {
                string implBank = System.Configuration.ConfigurationManager.AppSettings["ImplementationBank"];
                if (string.IsNullOrWhiteSpace(implBank))
                {
                    throw new InvalidOperationException("Required AppSettings Config item is missing: ImplementationBank");
                }
                string explectedValues = "";
                Enum.GetNames(typeof(ImplementationBank)).ToList().ForEach(x => explectedValues += x + ", ");
                explectedValues = string.IsNullOrWhiteSpace(explectedValues) ? "" : explectedValues.Substring(0, explectedValues.Length - 2);
                ImplementationBank bank = ImplementationBank.ProvidusBank;
                try
                {
                    bank = (ImplementationBank)Enum.Parse(typeof(ImplementationBank), implBank);
                }
                catch
                {
                    throw new InvalidOperationException(string.Format("Invalid value for AppSettings Config item: ImplementationBank. Expected values: {0}", explectedValues));
                }
                return bank;
            }
        }

        /// <summary>
        /// Removes all Occurrence of each item in the array of special characters passed as argument from the 'stringToTrim' argument.
        /// Also, removes all occurrence of 'comma'.
        /// </summary>
        /// <param name="stringToTrim"></param>
        /// <param name="specialXters"></param>
        /// <returns></returns>        
        private static string TrimOutSpecialCharacters(string stringToTrim, string[] specialXters)
        {
            if (string.IsNullOrEmpty(stringToTrim) || string.IsNullOrEmpty(stringToTrim.Trim()) || specialXters == null)
            {
                return stringToTrim;
            }

            string result = stringToTrim;
            if (specialXters != null && specialXters.Length > 0)
            {
                foreach (string xter in specialXters)
                {
                    if (!string.IsNullOrEmpty(xter) || !string.IsNullOrEmpty(xter.Trim()))
                    {
                        result = result.Replace(xter, "");
                    }
                    result = result.Replace(",", "");       // Replace all Comma's 
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " "); // Replace all WhiteSpace with a Single Space
                result = result.Trim();
            }
            return result;
        }

        /// <summary>
        /// Removes all Occurrence of each item in the array of special characters setup in the config file, from the 'stringToTrim' argument.
        /// Also, removes all occurrence of 'comma'.
        /// </summary>
        /// <param name="stringToTrim"></param>
        /// <returns></returns>
        public static string TrimOutSpecialCharacters(string stringToTrim)
        {
            Utils.ConvertToASCII(stringToTrim, out stringToTrim);

            var SpecialXters = System.Configuration.ConfigurationManager.AppSettings["SpecialXters"];
            if (string.IsNullOrEmpty(stringToTrim) || string.IsNullOrEmpty(stringToTrim.Trim()) || SpecialXters == null)
            {
                return stringToTrim;
            }

            string trimmedString = Utils.TrimOutSpecialCharacters(stringToTrim, SpecialXters.Split(new char[] { '8' }, StringSplitOptions.RemoveEmptyEntries));
            trimmedString = Utils.TrimOutSpecialCharacters(trimmedString, SpecialXters.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            return trimmedString;
        }

        /// <summary>
        /// Converts the input string to ASCII, using the UTF8 Enconding.
        /// </summary>
        /// <param name="stringToConvert">The input string to be converted.</param>
        /// <param name="convertedString">The output of the conversion operation.</param>
        /// <returns>A value indicating that the conversion was successful or otherwise.</returns>
        public static bool ConvertToASCII(string stringToConvert, out string convertedString)
        {
            convertedString = string.Empty;
            if (string.IsNullOrEmpty(stringToConvert) || string.IsNullOrEmpty(stringToConvert.Trim()))
                return true;

            bool retVal = false;
            try
            {
                convertedString = Encoding.ASCII.GetString(
                    Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(Encoding.ASCII.EncodingName, new EncoderReplacementFallback(string.Empty), new DecoderExceptionFallback()),
                        Encoding.UTF8.GetBytes(stringToConvert.Trim())));
                retVal = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("In Utils.ConvertToASCII", ex));
                convertedString = stringToConvert.Trim();
                retVal = false;
            }

            return retVal;
        }

        public static bool IsEMailValid(string theEMail)
        {
            if (string.IsNullOrEmpty(theEMail) || string.IsNullOrEmpty(theEMail.Trim()))
                return false;

            bool retVal = false;
            try
            {
                var validEmail = new System.Net.Mail.MailAddress(theEMail);
                retVal = true;
            }
            catch (Exception)
            {
                retVal = false;
            }

            return retVal;
        }

        /// <summary>
        /// Removes all unknown and white-space characters.
        /// </summary>
        /// <param name="stringToTrim"></param>
        /// <returns></returns>
        public static string TrimOutUnknownCharacters(string stringToTrim)
        {
            string converted = string.Empty;
            Utils.ConvertToASCII(stringToTrim, out converted);
            return converted.Trim();
        }

        /// <summary>
        /// Translates the supplied <see cref="MiddleWareResponseCodes"/> value to its <see cref="Int32"/> equivalent, and returns the value as a <see cref="string"/>. 
        /// </summary>
        /// <param name="responseCode">The value to translate.</param>
        /// <returns>The <see cref="Int32"/> equivalent response code as a <see cref="string"/></returns>
        public static string ToIntVal(MiddleWareResponseCodes responseCode)
        {
            string retVal = ((int)responseCode).ToString();
            if (retVal.Length < 2)
                retVal = retVal.PadLeft(2, '0');
            return retVal;
        }

        /// <summary>
        /// Removes escape sequences from the JSON string and returns the actual JSON data. 
        /// </summary>
        /// <param name="src">The source JSON string.</param>
        /// <returns>The actual JSON data</returns>
        /// <Author>Sosan Babajide</Author>
        public static string ExtractJSON(string src)
        {
            src = src.Replace("\r\n", string.Empty)
                           .Replace("<Response>", string.Empty)
                           .Replace("</Response>", string.Empty)
                           .Trim('"')
                           .Replace("\\\"\\\"", "\" \"")  // Fix for a JSON Serialization Exception. Note that the space in-between is intentional, and it's for properties that are NULL!
                           .Replace("\\\"", "\"")
                           .Replace("\"{", "{")
                           .Replace("}\"", "}")
                           .Replace("\"[", "[")
                           .Replace("]\"", "]")
                           .Replace("\\{", "{")
                           .Replace("}\\", "}")
                           .Replace("\\[", "[")
                           .Replace("]\\", "]");
            //.Replace("\\", string.Empty).Replace("\\\\", string.Empty).Replace("\"\"", "\"");

            if (src.Contains("\\\""))
            {
                src = ExtractJSON(src);
            }

            src = System.Net.WebUtility.HtmlDecode(src);

            return src;
        }

    }
}
