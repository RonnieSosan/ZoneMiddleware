using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities.Middleware.Main.Entities;
using System.Net;
using AppZoneMiddleware.Shared.Utility;
using System.Text.RegularExpressions;
using System.Net.Http;
using AppZoneMiddleware.Shared.Entities;
using Blend.ProvidusImplementation.Custom_Service_Reference;
using AppZoneMiddleware.Shared.Extension;

namespace Blend.ProvidusImplementation.SharedServices
{
    public class InterswitchPayCode : IPayCode
    {
        public static string frontEndPartnerId = "PVB";

        private static HttpClient client = null;

        public Task<InterswitchCancelTokenResponse> CancleToken(InterswitchCancelTokenRequest Request)
        {
            Logger.LogInfo("InterswitchPayCodeService.CancleToken, input", Request);

            InterswitchCancelTokenResponse interswitchResponse = new InterswitchCancelTokenResponse();

            try
            {
                MailRequest mailRequest = Request.MailRequest != null ? Request.MailRequest : null;
                Request.frontEndPartner = frontEndPartnerId;
                string jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(Request);

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var baseAddress = string.Format("{0}/api/v1/pwm/tokens", System.Configuration.ConfigurationManager.AppSettings.Get("InterswitchPaycodeURL"));

                HttpResponseMessage response = null;

                using (client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Delete, new Uri(baseAddress)))
                    {
                        client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("iSectimeout")));

                        request.Content = new StringContent(jsonMessage.ToString(), Encoding.UTF8, "application/json");

                        //set interswitch authentication headers
                        InterswitchAuthenticationService.SetHeaders(request, baseAddress, "DELETE", "clientID", "secretKey");
                        //var response = client.PostAsJsonAsync<InterswitchTokenGenRequest>(baseAddress, tokenRequest).Result;
                        var responseTask = client.SendAsync(request);
                        responseTask.Wait();
                        response = responseTask.Result;
                    }
                }

                Logger.LogInfo("InterswitchPayCodeService.CancleToken,CancleToken", response.Content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    interswitchResponse = response.Content.ReadAsAsync<InterswitchCancelTokenResponse>().Result;
                    interswitchResponse.ResponseCode = "00";
                    interswitchResponse.ResponseDescription = "SUCCESSFUL";
                    interswitchResponse.code = "00";
                    interswitchResponse.description = "SUCCESSFUL";

                    if (mailRequest != null)
                    {
                        string mail = Newtonsoft.Json.JsonConvert.SerializeObject(Request.MailRequest);

                        Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMailToCustomer").CallService<MailResponse>(mail));
                    }

                }
                else
                {
                    InterswitchBaseResponse interswitchFailedResponse = response.Content.ReadAsAsync<InterswitchBaseResponse>().Result;
                    interswitchResponse.error = interswitchFailedResponse.error;
                    interswitchResponse.ResponseCode = "06";
                    interswitchResponse.ResponseDescription = "FAILED";
                }
            }
            catch (Exception ex)
            {
                interswitchResponse.ResponseCode = "06";
                interswitchResponse.ResponseDescription = "FAILED: " + ex.Message;
            }

            Logger.LogInfo("InterswitchPayCodeService.CancleToken, CancleToken", Newtonsoft.Json.JsonConvert.SerializeObject(interswitchResponse));
            return Task<InterswitchCancelTokenResponse>.Factory.StartNew(() => interswitchResponse);
        }

        public Task<InterswitchTokenStatusResponse> CheckStatus(InterswitchTokenStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<InterswitchTokenGenResponse> GenerateToken(InterswitchTokenGenRequest request)
        {
            Logger.LogInfo("InterswitchPayCodeService.GenerateToken, input", request);

            InterswitchTokenGenResponse interswitchResponse = new InterswitchTokenGenResponse();
            UserProfileResponse authResponse = null;
            string authenticationRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            string jsonResponse = (new AppzoneApiProcessor("Blend", "Authentication", "TransactionalTokenAuthentication").CallService<UserProfileResponse>(authenticationRequest));
            authResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileResponse>(jsonResponse);

            if (authResponse.ResponseCode == "06")
            {
                InterswitchTokenGenResponse AuthResponse = new InterswitchTokenGenResponse()
                {
                    ResponseCode = authResponse.ResponseCode,
                    ResponseDescription = authResponse.ResponseDescription,
                };

                Logger.LogInfo("InterswitchPayCodeService.GenerateToken, failedResponse ", AuthResponse);
                return Task.Factory.StartNew<InterswitchTokenGenResponse>(() => AuthResponse);
            }

            try
            {
                MailRequest mailRequest = request.MailRequest != null ? request.MailRequest : null;
                request.MailRequest = null;


                string builder = request.subscriberId;
                if (builder.StartsWith("0"))
                {
                    builder = builder.Substring(1, builder.Length - 1);
                    request.subscriberId = "234" + builder;
                }

                request.amount = (Convert.ToDecimal(request.amount) * 100).ToString().Replace(".00", "");
                request.autoEnroll = "false";
                request.accountType = request.accountType == "CURRENT" ? "20" : "10";

                string jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(request);

                var baseAddress = string.Format("{0}/api/v1/pwm/subscribers/{1}/tokens", System.Configuration.ConfigurationManager.AppSettings["InterswitchPaycodeURL"], request.subscriberId);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                            | SecurityProtocolType.Tls11
                            | SecurityProtocolType.Tls12
                            | SecurityProtocolType.Ssl3;

                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;

                HttpResponseMessage response = null;

                using (client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["iSectimeout"]));
                    using (var httpMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAddress)))
                    {
                        httpMessage.Content = new StringContent(jsonMessage.ToString(), Encoding.UTF8, "application/json");
                        httpMessage.Headers.Add("frontEndPartnerId", frontEndPartnerId);

                        //set interswitch authentication headers
                        InterswitchAuthenticationService.SetHeaders(httpMessage, baseAddress, "POST", "clientID", "secretKey");
                        Logger.LogInfo("InterswitchPayCodeService.GenerateToken,Headers", httpMessage.Headers + " " + jsonMessage);
                        var responseTask = client.SendAsync(httpMessage);
                        responseTask.Wait();
                        response = responseTask.Result;
                    }
                }


                if (response.StatusCode == HttpStatusCode.Created)
                {
                    interswitchResponse = response.Content.ReadAsAsync<InterswitchTokenGenResponse>().Result;
                    interswitchResponse.ResponseCode = "00";
                    interswitchResponse.ResponseDescription = "SUCCESSFUL";
                    interswitchResponse.transactionRef = request.transactionRef;

                    if (mailRequest != null)
                    {
                        mailRequest.MailBody = Regex.Replace(mailRequest.MailBody, @"#TokenCode", interswitchResponse.payWithMobileToken);
                        mailRequest.MailBody = Regex.Replace(mailRequest.MailBody, @"#PaymentRef", interswitchResponse.transactionRef);

                        string mail = Newtonsoft.Json.JsonConvert.SerializeObject(mailRequest);

                        Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMailToCustomer").CallService<MailResponse>(mail));

                        var payload = new { customer_id = request.customer_id, Message = string.Format("Paycode Token: {0}, Payment Ref: {1}", interswitchResponse.payWithMobileToken, interswitchResponse.transactionRef) };
                        Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendSMS").CallService<SMSResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(payload)));
                    }
                }
                else
                {
                    var type = response.Content.Headers.ContentType;
                    InterswitchBaseResponse interswitchFailedResponse = response.Content.ReadAsAsync<InterswitchBaseResponse>().Result;
                    interswitchResponse.error = interswitchFailedResponse.error;
                    interswitchResponse.ResponseCode = "06";
                    interswitchResponse.ResponseDescription = "FAILED: " + interswitchFailedResponse.error.message;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                interswitchResponse.ResponseCode = "06";
                interswitchResponse.ResponseDescription = "FAILED: " + ex.Message;
            }

            Logger.LogInfo("InterswitchPayCodeService.GenerateToken, output", Newtonsoft.Json.JsonConvert.SerializeObject(interswitchResponse));
            return Task<InterswitchTokenGenResponse>.Factory.StartNew(() => interswitchResponse);
        }
    }
}
