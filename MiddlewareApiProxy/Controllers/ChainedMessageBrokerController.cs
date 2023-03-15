using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MiddlewareApiProxy.Controllers
{
    public class ChainedMessageBrokerController : ApiController
    {
        // POST api/values
        [HttpPost]
        public IHttpActionResult ProcessChainedRequest([FromBody]ApiProxyRequest value)
        {
            string apiChainURL = System.Configuration.ConfigurationManager.AppSettings["ApiChainBaseURL"];
            Logger.LogInfo($"ChainedMessageBrokerController.ProcessChainedRequest: [ApiChainURL = {apiChainURL}] => request", value);
            string rawResponse = string.Empty;
            ApiProxyResponse retVal = new ApiProxyResponse();

            if (string.IsNullOrWhiteSpace(value.EndPointURL))
            {
                rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'ApiUrl' is not supplied." });
                retVal = JsonConvert.DeserializeObject<ApiProxyResponse>(rawResponse);
            }
            else if (string.IsNullOrWhiteSpace(value.Context.Body))
            {
                rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'PayLoad' is not supplied." });
                retVal = JsonConvert.DeserializeObject<ApiProxyResponse>(rawResponse);
            }
            else
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(apiChainURL), Method = HttpMethod.Post, Content = new ObjectContent<ApiProxyRequest>(value, new System.Net.Http.Formatting.JsonMediaTypeFormatter()) };

                        HttpResponseMessage response = client.SendAsync(request).Result;
                        //response.EnsureSuccessStatusCode();   // This will result in an Exception if the HTTP Status Code is not successful. 
                        rawResponse = response.Content.ReadAsStringAsync().Result;
                    }

                    Logger.LogInfo($"ChainedMessageBrokerController.ProcessChainedRequest: [ApiChainURL = {apiChainURL}] => rawResponse", rawResponse);

                    if (!string.IsNullOrWhiteSpace(rawResponse))
                    {
                        retVal = JsonConvert.DeserializeObject<ApiProxyResponse>(Utils.ExtractJSON(rawResponse));
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Failed" });
                    retVal = JsonConvert.DeserializeObject<ApiProxyResponse>(rawResponse);
                }
            }

            Logger.LogInfo($"ChainedMessageBrokerController.ProcessChainedRequest: [ApiChainURL = {apiChainURL}] => retVal", retVal);
            return Json(retVal);
        }


    }
}
