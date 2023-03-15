using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Utility
{
    public class ResponseCodeTranslator
    {
        /// <summary>
        /// Both the "Key" and "Value" is the <see cref="Int32"/> value of the corresponding <see cref="MiddleWareResponseCodes"/>.
        /// </summary>
        private static SortedList<string, string> _middlewareRespCodesIntValues;

        /// <summary>
        /// The "Key" is the <see cref="String"/> value of the corresponding <see cref="MiddleWareResponseCodes"/>, while the "Value" is it's <see cref="Int32"/> value.
        /// </summary>
        private static SortedList<string, string> _middlewareRespCodesStringValues;

        /// <summary>
        /// Static constructor that initializes required static properties. 
        /// </summary>
        static ResponseCodeTranslator()
        {
            if (_middlewareRespCodesIntValues == null || _middlewareRespCodesStringValues == null)
            {
                _middlewareRespCodesIntValues = new SortedList<string, string>();
                _middlewareRespCodesStringValues = new SortedList<string, string>();

                foreach (var arr in Enum.GetValues(typeof(MiddleWareResponseCodes)))
                {
                    var intVal = ((int)arr).ToString().PadLeft(2, '0');
                    _middlewareRespCodesIntValues.Add(intVal, intVal);
                    _middlewareRespCodesStringValues.Add(arr.ToString(), intVal);
                }
            }
        }

        /// <summary>
        /// Translates response code (expected to be from API Endpoints) to its middleware equivalent. 
        /// </summary>
        /// <param name="responseCode">The value to translate.</param>
        /// <returns>The middleware equivalent response code of type <see cref="MiddleWareResponseCodes"/>.</returns>
        public static string TranslateResponseCodeToMiddlewareEquivalent(string responseCode)
        {
            if (string.IsNullOrWhiteSpace(responseCode))
                return responseCode;

            string retVal = string.Empty;
            responseCode = responseCode.Trim();

            if (_middlewareRespCodesIntValues.ContainsKey(responseCode))
            {
                retVal = _middlewareRespCodesIntValues[responseCode];
            }
            else if (_middlewareRespCodesStringValues.ContainsKey(responseCode))
            {
                retVal = _middlewareRespCodesStringValues[responseCode];
            }
            else
            {
                switch (responseCode)
                {
                    case "00":
                    case "000":
                        retVal = ToIntVal(MiddleWareResponseCodes.Successful);
                        break;
                    case "06":
                        retVal = ToIntVal(MiddleWareResponseCodes.SystemError);
                        break;
                    case "61":
                        retVal = ToIntVal(MiddleWareResponseCodes.TransferLimitExceeded);
                        break;
                    case "65":
                        retVal = ToIntVal(MiddleWareResponseCodes.ExceedsWithdrawalFrequency);
                        break;
                    case "91":
                        retVal = ToIntVal(MiddleWareResponseCodes.IssuerOrSwitchInoperative);
                        break;
                    case "004":
                    case "014":
                    case "021":
                    case "022":
                    case "023":
                    case "024":
                    case "025":
                    case "026":
                    case "027":
                    case "032":
                    case "047":
                    case "051":
                    case "081":
                    case "083":
                    case "084":
                    case "099":
                        retVal = ToIntVal(MiddleWareResponseCodes.MobifinAirtimeTopUpFailureOnUser);
                        break;
                    default:
                        retVal = responseCode;
                        break;
                }
            }

            if (retVal.Length < 2)
                retVal = retVal.PadLeft(2, '0');
            Logger.LogInfo("TranslateResponseCodeToMiddlewareEquivalent: ", string.Format("responseCode value of [{0}] was translated to [{1}]", responseCode, retVal));
            return retVal;
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
    }
}
