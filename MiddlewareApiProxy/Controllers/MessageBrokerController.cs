using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Utility;
using Blend.DefaultImplementation.Persistence;
using MiddlewareApiProxy.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MiddlewareApiProxy.Controllers
{
    [RoutePrefix("api/MessageBroker")]
    public class MessageBrokerController : ApiController
    {
        // POST api/values
        [HttpPost]
        [Route("Run")]
        public IHttpActionResult Run([FromBody]ApiProxyRequest value)
        {
            Logger.LogInfo("MessageBrokerController.Post: request", value);
            string retVal = string.Empty;
            string rawResponse = string.Empty;

            //Ensure request is not a null entity
            if (value == null)
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'ApiUrl' is not supplied." });
            }

            if (string.IsNullOrWhiteSpace(value.EndPointURL))
            {
                rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'ApiUrl' is not supplied." });
            }
            if (value.HttpMethod == string.Empty)
            {
                rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'HttpMethod' is not supplied." });
            }
            else if (string.IsNullOrWhiteSpace(value.Context.Body))
            {
                rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'PayLoad' is not supplied." });
            }

            else
            {
                if (value.Context.IsSoap)
                {
                    ContextRepository<ApiSecuritySpec> repository = new ContextRepository<ApiSecuritySpec>();
                    ApiSecuritySpec securitySpec = repository.Get(value.EndPointURL);
                    JObject soapParams = JsonConvert.DeserializeObject<JObject>(value.Context.Body);

                    if (securitySpec == null)
                    {
                        rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = " 'apiURL' not configured for this call" });
                    }
                    else
                    {
                        try
                        {
                            var response = new SoapRequestProcessor().ProcessRequest(soapParams["OperationName"].ToString(), soapParams["MethodName"].ToString(), soapParams["args"].ToString(), securitySpec.URL);
                            rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "00", ResponseDescription = "SUCCESSFUL", Body = response });
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex);
                            rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Failed " + ex.Message });
                        }
                    }

                }
                else
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpRequestMessage request = new HttpRequestMessage();

                            switch (value.HttpMethod.ToUpper())
                            {
                                case "POST":
                                    request = new HttpRequestMessage() { RequestUri = new Uri(value.EndPointURL), Method = HttpMethod.Post, Content = new StringContent(value.Context.Body, Encoding.UTF8, "application/json") };
                                    break;
                                case "GET":
                                    request = new HttpRequestMessage() { RequestUri = new Uri(value.EndPointURL), Method = HttpMethod.Get };
                                    break;
                                default:
                                    break;
                            }


                            foreach (var item in value.Context.Headers)
                                request.Headers.Add(item.Key, item.Value);

                            HttpResponseMessage response = client.SendAsync(request).Result;
                            rawResponse = response.Content.ReadAsStringAsync().Result;
                            Logger.LogInfo("MessageBrokerController.Client: ReadAsStringAsync()", rawResponse);
                            rawResponse = Utils.ExtractJSON(rawResponse);

                            if (!response.IsSuccessStatusCode)
                            {
                                // This will result in an Exception if the HTTP Status Code is not successful. 
                                try
                                {

                                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(rawResponse);
                                    rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "06", ResponseDescription = $"Filed Server Response ({response.StatusCode}): {Convert.ToString(jsonObject["Message"])}" });
                                }
                                catch (Exception)
                                {
                                    rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "06", ResponseDescription = rawResponse });
                                }

                            }
                            else
                            {
                                try
                                {

                                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(rawResponse);
                                    rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "00", ResponseDescription = "SUCCESSFUL", Body = jsonObject });
                                }
                                catch (JsonException jsonEx)
                                {
                                    try
                                    {
                                        // try to convert to object arrar then
                                        JArray jsonObject = JsonConvert.DeserializeObject<JArray>(rawResponse);
                                        rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "00", ResponseDescription = "SUCCESSFUL", Body = jsonObject });
                                    }
                                    catch (Exception ex)
                                    {
                                        rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "00", ResponseDescription = "SUCCESSFUL", Body = rawResponse });
                                    }
                                }
                                catch (Exception exUnknown)
                                {
                                    rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "00", ResponseDescription = "SUCCESSFUL", Body = rawResponse });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                        rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Failed " + ex.Message });
                    }
                }
            }

            Logger.LogInfo("MessageBrokerController.Post: rawResponse", rawResponse);
            return Ok(rawResponse);
        }

        [HttpPost]
        [Route("HealthCheck")]
        public IHttpActionResult HealthCheck([FromBody]HealthCheckRequest value)
        {
            Logger.LogInfo("MessageBrokerController.Post: request", value);
            string retVal = string.Empty;
            string rawResponse = string.Empty;
            HealthCheckResponse healthResponse = new HealthCheckResponse();

            //Ensure request is not a null entity
            if (value == null)
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'ApiUrl' is not supplied." });
            }

            if (string.IsNullOrWhiteSpace(value.EndPointURL))
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'ApiUrl' is not supplied." });
            }
            else if (string.IsNullOrWhiteSpace(value.Context.Body))
            {
                rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Value for 'PayLoad' is not supplied." });
            }
            else
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpRequestMessage request = new HttpRequestMessage();
                        if (value.Context.IsSoap)
                        {
                            request = new HttpRequestMessage() { RequestUri = new Uri(value.EndPointURL), Method = HttpMethod.Post, Content = new StringContent(value.Context.Body, Encoding.UTF8, "text/xml") };
                        }
                        else
                        {
                            request = new HttpRequestMessage() { RequestUri = new Uri(value.EndPointURL), Method = HttpMethod.Post, Content = new StringContent(value.Context.Body, Encoding.UTF8, "application/json") };
                        }

                        foreach (var item in value.Context.Headers)
                            request.Headers.Add(item.Key, item.Value);

                        var sw = new Stopwatch();
                        sw.Start();
                        HttpResponseMessage response = client.SendAsync(request).Result;
                        //response.EnsureSuccessStatusCode();   // This will result in an Exception if the HTTP Status Code is not successful. 
                        rawResponse = response.Content.ReadAsStringAsync().Result;


                        // time elapsed for Health check 
                        sw.Stop();
                        var elapased = sw.Elapsed.Seconds;

                        rawResponse = Utils.ExtractJSON(rawResponse);
                        healthResponse = new HealthCheckResponse()
                        {
                            ResponseAssertionSuccess = rawResponse.ToString().Contains(value.ResponseAssertion),
                            TimeAssertionSuccess = elapased < value.TimeAssertion,
                            TimeElapsed = elapased
                        };

                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    rawResponse = Newtonsoft.Json.JsonConvert.SerializeObject(new { ResponseCode = "MWB96", ResponseDescription = "Failed" });
                }
            }

            Logger.LogInfo("MessageBrokerController.Post: rawResponse", healthResponse);
            return Json(healthResponse);
        }

        [HttpGet]
        public IHttpActionResult RunServiceUpdate()
        {
            ContextRepository<ApiSecuritySpec> repository = new ContextRepository<ApiSecuritySpec>();
            List<ApiSecuritySpec> securitySpecs = repository.Get().Where(x => x.isSoap).ToList();

            foreach (var item in securitySpecs)
            {
                string serviceDescription = string.Empty;
                string serviceUri = "?wsdl";
                if (item.URL.Contains(".svc")) serviceUri = "?singlewsdl";
                try
                {
                    HttpRequestMessage requestHttp = new HttpRequestMessage(HttpMethod.Get, new Uri(item.URL + serviceUri));
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage serverResponse = new HttpResponseMessage();
                        serverResponse = client.SendAsync(requestHttp).Result;
                        serviceDescription = serverResponse.Content.ReadAsStringAsync().Result;
                        item.ServiceDescription = serviceDescription;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.InnerException == null ? ex : ex.InnerException);
                    continue;
                }
                repository.Update(item);

                ServiceDescriptionAssembler.Initialize();
            }

            return Ok();
        }

        [HttpPost]
        [Route("GetServiceOperations")]
        public IHttpActionResult GetServiceOperations(Models.GetServiceDescriptionRequest request)
        {
            Models.GetServiceDescriptionResponse response = new ServiceDescriptionAssembler().GetForSoap(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("GenerateServiceOperations")]
        public IHttpActionResult GenerateServiceOperations(Models.GenerateServiceDescriptionRequest request)
        {
            var response =new ServiceDescriptionAssembler().GenerateServiceOperations(request);
            return Ok(response);
        }
    }
}
