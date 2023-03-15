using AppZoneMiddleware.Shared.Utility;
using SterlingApiProxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SterlingApiProxy.Controllers
{
    public class ChainedMessageBrokerController : ApiController
    {
        // POST api/values
        [HttpPost]
        public IHttpActionResult PostToSpay([FromBody]BrokerParams value)
        {
            bool isLastGuyInChain = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLastGuyInChain"]);
            string apiChainURL = System.Configuration.ConfigurationManager.AppSettings["ApiChainBaseURL"] + "/PostToSpay";
            Logger.LogInfo($"ChainedMessageBrokerController.PostToSpay: [IsLastGuyInChain = {isLastGuyInChain} | [ApiChainURL = {apiChainURL}] => request", value);
            string rawResponse = string.Empty;
            BrokerParams retVal = new BrokerParams();

            if (string.IsNullOrWhiteSpace(value.ApiUrl))
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'ApiUrl' is not supplied." });
                retVal = new BrokerParams() { Result = rawResponse };
            }
            else if (string.IsNullOrWhiteSpace(value.PayLoad))
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'PayLoad' is not supplied." });
                retVal = new BrokerParams() { Result = rawResponse };
            }
            else
            {
                try
                {
                    if (isLastGuyInChain)
                    {
                        rawResponse = new Blend.SterlingImplementation.ServiceUtilities.RESTProcessor<string, string>(value.ApiUrl).DoSpayPOST(value.PayLoad, false) as string;
                    }
                    else
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(apiChainURL), Method = HttpMethod.Post, Content = new ObjectContent<BrokerParams>(value, new System.Net.Http.Formatting.JsonMediaTypeFormatter()) };

                            HttpResponseMessage response = client.SendAsync(request).Result;
                            //response.EnsureSuccessStatusCode();   // This will result in an Exception if the HTTP Status Code is not successful. 
                            rawResponse = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                    Logger.LogInfo($"ChainedMessageBrokerController.PostToSpay: [IsLastGuyInChain = {isLastGuyInChain} | [ApiChainURL = {apiChainURL}] => rawResponse", rawResponse);

                    if (!string.IsNullOrWhiteSpace(rawResponse))
                    {
                        if (isLastGuyInChain)
                        {
                            string extractedJSON = Utils.ExtractJSON(rawResponse);
                            retVal = new BrokerParams() { Result = extractedJSON };
                        }
                        else
                        {
                            retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<BrokerParams>(rawResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Failed" });
                    retVal = new BrokerParams() { Result = rawResponse };
                }
            }
            
            Logger.LogInfo($"ChainedMessageBrokerController.PostToSpay: [IsLastGuyInChain = {isLastGuyInChain} | [ApiChainURL = {apiChainURL}] => retVal", retVal);
            return Json(retVal);
        }

        // POST api/values
        [HttpPost]
        public IHttpActionResult PostToIBSBridge([FromBody]BrokerParams value)
        {
            bool isLastGuyInChain = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsLastGuyInChain"]);
            string apiChainURL = System.Configuration.ConfigurationManager.AppSettings["ApiChainBaseURL"] + "/PostToIBSBridge";
            Logger.LogInfo($"ChainedMessageBrokerController.PostToIBSBridge: [IsLastGuyInChain = {isLastGuyInChain} | [ApiChainURL = {apiChainURL}] => request", value);
            string rawResponse = string.Empty;
            BrokerParams retVal = new BrokerParams();

            if (string.IsNullOrWhiteSpace(value.ApiUrl))
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'ApiUrl' is not supplied." });
                retVal = new BrokerParams() { Result = rawResponse };
            }
            else if (string.IsNullOrWhiteSpace(value.PayLoad))
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'PayLoad' is not supplied." });
                retVal = new BrokerParams() { Result = rawResponse };
            }
            else
            {
                try
                {
                    if (isLastGuyInChain)
                    {
                        rawResponse = new Blend.SterlingImplementation.ServiceUtilities.IBSBridgeProcessor<string, string>().ProcessIBSBridge(value.PayLoad, true) as string;
                    }
                    else
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(apiChainURL), Method = HttpMethod.Post, Content = new ObjectContent<BrokerParams>(value, new System.Net.Http.Formatting.JsonMediaTypeFormatter()) };

                            HttpResponseMessage response = client.SendAsync(request).Result;
                            //response.EnsureSuccessStatusCode();   // This will result in an Exception if the HTTP Status Code is not successful. 
                            rawResponse = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                    Logger.LogInfo($"ChainedMessageBrokerController.PostToIBSBridge: [IsLastGuyInChain = {isLastGuyInChain} | [ApiChainURL = {apiChainURL}] => rawResponse", rawResponse);

                    if (!string.IsNullOrWhiteSpace(rawResponse))
                    {
                        if (isLastGuyInChain)
                        {
                            string extractedJSON = Utils.ExtractJSON(rawResponse);
                            retVal = new BrokerParams() { Result = extractedJSON };
                        }
                        else
                        {
                            retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<BrokerParams>(rawResponse);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Failed" });
                    retVal = new BrokerParams() { Result = rawResponse };
                }
            }
            
            Logger.LogInfo($"ChainedMessageBrokerController.PostToIBSBridge: [IsLastGuyInChain = {isLastGuyInChain} | [ApiChainURL = {apiChainURL}] => retVal", retVal);
            return Json(retVal);
        }
    }
}
