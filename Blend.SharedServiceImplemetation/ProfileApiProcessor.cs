using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SharedServiceImplementation
{
    public class ProfileApiProcessor
    {
        public string Url = System.Configuration.ConfigurationManager.AppSettings.Get("ProfileApiUrl");
        private static HttpClient client = null;

        public ProfileApiProcessor(string Platform, string Service, string Operation)
        {
            Url = string.Format(Url, Platform, Service, Operation);
        }
        public class GenericFailedResponse : BaseResponse { }
        public string CallService <T>(string RequestMessage)
        {
            Logger.LogInfo("AppzoneApiProcessor.CallService, Url", Url);

            try {
                //call providusAPI to do posting for providus recharge request
                HttpResponseMessage apiResponse = null;

                using (client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("iSectimeout")));
                    using (var apiRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(Url)))
                    {
                        apiRequest.Content = new StringContent(RequestMessage, Encoding.UTF8, "application/json");

                        Logger.LogInfo("AppzoneApiProcessor.CallService, Headers", apiRequest.Headers + " " + RequestMessage);
                        var responseTask = client.SendAsync(apiRequest);
                        responseTask.Wait();
                        apiResponse = responseTask.Result;
                        var response = Newtonsoft.Json.JsonConvert.SerializeObject(apiResponse.Content.ReadAsAsync<T>().Result);
                        Logger.LogInfo("AppzoneApiProcessor.CallService, Response", response);
                        return response;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Newtonsoft.Json.JsonConvert.SerializeObject(new GenericFailedResponse { ResponseCode = "06", ResponseDescription = "Failed: "+ex.Message});
            }
          }
        }
    }
