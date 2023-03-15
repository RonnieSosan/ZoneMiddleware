using AppZoneMiddleware.Shared.Utility;
using SterlingApiProxy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace SterlingApiProxy.Controllers
{
    public class MessageBrokerController : ApiController
    {
        // POST api/values
        public IHttpActionResult Post([FromBody]BrokerParams value)
        {
            Logger.LogInfo("MessageBrokerController.Post: request", value);
            string retVal = string.Empty;
            string rawResponse = string.Empty;

            if (string.IsNullOrWhiteSpace(value.ApiUrl))
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'ApiUrl' is not supplied." });
            }
            else if (string.IsNullOrWhiteSpace(value.PayLoad))
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'PayLoad' is not supplied." });
            }
            else
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(value.ApiUrl), Method = HttpMethod.Post, Content = new StringContent(value.PayLoad, Encoding.UTF8, "application/json") };

                        HttpResponseMessage response = client.SendAsync(request).Result;
                        //response.EnsureSuccessStatusCode();   // This will result in an Exception if the HTTP Status Code is not successful. 
                        rawResponse = response.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Failed" });
                }
            }

            Logger.LogInfo("MessageBrokerController.Post: rawResponse", rawResponse);
            return Json(rawResponse);
        }
    }
}
