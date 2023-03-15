using AppZoneMiddleware.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.ServiceUtilities
{
    /// <summary>
    /// Processor for sending requests to a REST endpoint.  
    /// </summary>
    /// <typeparam name="G">Type of request class</typeparam>
    /// <typeparam name="T">Type of response class</typeparam>
    public class RESTProcessor<G, T>
    {
        DataExchangeFormat _dataFormat = DataExchangeFormat.JSON;
        int spayAppID = int.Parse(System.Configuration.ConfigurationManager.AppSettings["SPayRESTAppId"]);
        string _apiEndPoint = string.Empty;

        public RESTProcessor(string apiEndPoint, DataExchangeFormat dataFormat = DataExchangeFormat.JSON)
        {
            _apiEndPoint = apiEndPoint;
            _dataFormat = dataFormat;
        }

        public object DoPOST(G RequestMessage, bool performJSONSerialization, bool performEncryptionAfterSerialization)
        {
            Logger.LogInfo("RESTProcessor.DoPOST: request", RequestMessage);
            T retVal = default(T);
            string thePayload = string.Empty;

            try
            {
                if (performJSONSerialization)
                {
                    StringBuilder requestPayload = new StringBuilder();
                    using (var writer = new StringWriter(requestPayload))
                    {
                        new Newtonsoft.Json.JsonSerializer().Serialize(writer, RequestMessage, typeof(G));
                    }
                    Logger.LogInfo("RESTProcessor.DoPOST", $"Serialized Request Payload: {requestPayload.ToString()}");

                    if (performEncryptionAfterSerialization)
                    {
                        thePayload = new ServiceUtilities.Utilities().SterlingEncrypt(requestPayload.ToString());
                        Logger.LogInfo("RESTProcessor.DoPOST", $"Encrypted Request Payload: {thePayload}");
                    }
                    else
                    {
                        thePayload = requestPayload.ToString();
                    }
                }
                else
                {
                    thePayload = Convert.ToString(RequestMessage);
                }

                string mediaType = GetMediaType();
                string rawResponse = string.Empty;
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(_apiEndPoint), Method = HttpMethod.Post, Content = new StringContent(thePayload, Encoding.UTF8, mediaType) };
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                    request.Headers.Add("Appid", spayAppID.ToString());

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    //response.EnsureSuccessStatusCode();   // This will result in an Exception if the HTTP Status Code is not successful. 
                    rawResponse = response.Content.ReadAsStringAsync().Result;
                }
                Logger.LogInfo("RESTProcessor.DoPOST: rawResponse", rawResponse);

                if (typeof(T).FullName == typeof(string).FullName)
                {
                    return rawResponse;
                }
                else
                {
                    retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(rawResponse);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw ex;
            }

            Logger.LogInfo("RESTProcessor.DoPOST: response", retVal);
            return retVal;
        }
        
        public object DoSpayPOST(string jsonPayload, bool performEncryption)
        {
            Logger.LogInfo("RESTProcessor.DoSpayPOST: request", jsonPayload);
            T retVal = default(T);
            string thePayload = string.Empty;

            try
            {
                if (performEncryption)
                {
                    thePayload = new ServiceUtilities.Utilities().SterlingEncrypt(jsonPayload);
                    Logger.LogInfo("RESTProcessor.DoSpayPOST", $"Encrypted Request Payload: {thePayload}");
                }
                else
                {
                    thePayload = jsonPayload;
                }

                string mediaType = GetMediaType();
                string rawResponse = string.Empty;
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(_apiEndPoint), Method = HttpMethod.Post, Content = new StringContent(thePayload, Encoding.UTF8, mediaType) };
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
                    request.Headers.Add("Appid", spayAppID.ToString());

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    //response.EnsureSuccessStatusCode();   // This will result in an Exception if the HTTP Status Code is not successful. 
                    rawResponse = response.Content.ReadAsStringAsync().Result;
                }
                Logger.LogInfo("RESTProcessor.DoPOST: rawResponse", rawResponse);

                if (typeof(T).FullName == typeof(string).FullName)
                {
                    return rawResponse;
                }
                else
                {
                    retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(rawResponse);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw ex;
            }

            Logger.LogInfo("RESTProcessor.DoSpayPOST: response", retVal);
            return retVal;
        }

        public string GetMediaType()
        {
            string retVal = string.Empty;
            switch (_dataFormat)
            {
                case DataExchangeFormat.JSON:
                    retVal = "application/json";
                    break;
                case DataExchangeFormat.XML:
                    retVal = "text/xml";
                    break;
                default:
                    throw new ApplicationException($"Data Exchange Format [{_dataFormat}] is currently not supported.");
            }
            return retVal;
        }
    }
}
