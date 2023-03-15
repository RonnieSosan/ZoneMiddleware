using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using System.Text.RegularExpressions;
using AppZoneMiddleware.Shared.Extension;
using Blend.DefaultImplementation.ProfileService;
using Blend.DefaultImplementation.Persistence;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Blend.HeritageImplementation.Utility;
using System.Net.Http;

namespace Blend.HeritageImplementation.ProfileService
{
    public class ProfileManager : DefaultProfileManager, IProfileManager
    {
        string base64Key = "2344asdfWPOuARSFUs10Lm==";
        string chainMessageBroker = System.Configuration.ConfigurationManager.AppSettings.Get("ApiChainBaseURL");
        public ProfileManager(IMailService mailServcie, ISMSSender smsService, IAccountInquiry accountInquiryService) : base(mailServcie, smsService, accountInquiryService)
        {
        }

        public Task<UserProfileResponse> RegisterProfile(UserAccount ProfileToRegister)
        {
            Logger.LogInfo("UserProfileService.RegisterProfile, input", ProfileToRegister);
            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            ContextRepository<UserOTP> otpRep = new ContextRepository<UserOTP>();
            UserProfileResponse response = null;
            UserOTP userOtp = null;
            try
            {
                userOtp = otpRep.Get(ProfileToRegister.CustomerID);

                if (ProfileToRegister.IsExistingAccountHolder)
                {
                    userOtp = otpRep.Get(ProfileToRegister.CustomerID);
                }
                else
                {
                    userOtp = otpRep.Get(ProfileToRegister.BVN);
                }

                if (userOtp != null)
                {
                    //if (DateTime.Now <= userOtp.OTPExpDate)
                    //{

                    Logger.LogInfo("UserProfileService.RegisterProfile, Saved userOTP", PasswordStorage.CreateHash(ProfileToRegister.userOTP.ValidationToken));
                    if (PasswordStorage.VerifyPassword(ProfileToRegister.userOTP.ValidationToken, userOtp.ValidationToken))
                    {
                        string authToken = new PasswordStorage().generateOTP();
                        UserProfile profile = new UserProfile
                        {
                            Username = ProfileToRegister.Username,
                            CustomerID = ProfileToRegister.CustomerID,
                            Password = ProfileToRegister.Password,
                            PIN = ProfileToRegister.PIN,
                            AuthToken = PasswordStorage.CreateHash(authToken),
                            TokenExpDate = DateTime.Now.AddMinutes(otpTimeSpan),
                            InvalidTrials = 0,
                            IsLocked = false
                        };
                        repository.Save(profile);

                        response.ResponseCode = "00";
                        response.ResponseDescription = "User profiling successful";
                    }
                    else
                    {
                        response = new UserProfileResponse()
                        {
                            ResponseCode = "06",
                            ResponseDescription = "User Token Invalid"
                        };
                    }
                }
                else
                {
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "User has not requested for verification code"
                    };
                }

            }
            catch (Exception ex)
            {
                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
                Logger.LogError(ex);
            };

            Logger.LogInfo("UserProfileService.RegisterProfile, response", response);
            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public new Task<JObject> ProxyLogin(ApiProxyRequest loginRequest)
        {
            Logger.LogInfo("UserProfileService.UserLogin, input: ", loginRequest);
            JObject loginResponse = new JObject();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage();

                    httpRequestMessage = new HttpRequestMessage() { RequestUri = new Uri(chainMessageBroker), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json") };

                    HttpResponseMessage httpResponse = client.SendAsync(httpRequestMessage).Result;
                    var rawResponse = Utils.ExtractJSON(httpResponse.Content.ReadAsStringAsync().Result);

                    Logger.LogInfo("UserProfileService.UserLogin, endpoint string response: ", rawResponse);

                    JObject response = JsonConvert.DeserializeObject<JObject>(rawResponse);
                    Logger.LogInfo("UserProfileService.UserLogin, endpoint body response: ", Convert.ToString(response["Body"]));
                    loginResponse = JsonConvert.DeserializeObject<JObject>(new Crypto().DecryptString(Convert.ToString(response["Body"]), base64Key));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                loginResponse.Add("ResponseCode", "06");
                loginResponse.Add("ResponseDescription", "unable to complete login process");
                return Task.Factory.StartNew(() => loginResponse);
            }

            JObject responseStatus = new JObject();
            JObject StatusResponse = new JObject();
            JArray accounts = new JArray();
            try
            {

                responseStatus = JsonConvert.DeserializeObject<JObject>(Convert.ToString(loginResponse["AccountList"]));
                StatusResponse = JsonConvert.DeserializeObject<JObject>(Convert.ToString(responseStatus["StatusResponse"]));
                accounts = JsonConvert.DeserializeObject<JArray>(Convert.ToString(responseStatus["Account"]));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                loginResponse.Add("ResponseCode", "06");
                loginResponse.Add("ResponseDescription", "unable to read response from login API");
                loginResponse.Add("Body", loginResponse);
                return Task.Factory.StartNew(() => loginResponse);
            }
            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            if (Convert.ToString(StatusResponse["ResponseCode"]) == "00")
            {
                try
                {
                    UserProfile verifyingAccount = repository.Get(loginRequest.AuthData.customer_id);
                    string authToken = new PasswordStorage().generateOTP();

                    if (verifyingAccount != null)
                    {
                        verifyingAccount.AuthToken = PasswordStorage.CreateHash(authToken);
                        verifyingAccount.TokenExpDate = DateTime.Now.AddMinutes(otpTimeSpan);
                        verifyingAccount.InvalidTrials = 0;
                        verifyingAccount.IsLocked = false;

                        repository.Update(verifyingAccount);
                    }
                    else
                    {
                        verifyingAccount = new UserProfile
                        {
                            AuthToken = PasswordStorage.CreateHash(authToken),
                            TokenExpDate = DateTime.Now.AddMinutes(otpTimeSpan),
                            InvalidTrials = 0,
                            IsLocked = false,
                            CustomerID = loginRequest.AuthData.customer_id
                        };

                        repository.Save(verifyingAccount);
                    }
                    loginResponse = new JObject();
                    loginResponse.Add("ResponseCode", "00");
                    loginResponse.Add("ResponseDescription", "SUCCESSFUL");
                    loginResponse.Add("AuthToken", authToken);
                    loginResponse.Add("Accounts", accounts);
                }
                catch (Exception ex)
                {
                    StatusResponse["ResponseCode"] = "06";
                    loginResponse.Add("ResponseCode", "06");
                    loginResponse.Add("ResponseDescription", "FAILURE: " + ex.Message);
                }
            }
            else
            {
                loginResponse.Add(Convert.ToString(StatusResponse["ResponseCode"]));
                loginResponse.Add(Convert.ToString(StatusResponse["ResponseText"]));
            }
            Logger.LogInfo("UserProfileService.UserProxyLogin, response", loginResponse);
            return Task.Factory.StartNew(() => loginResponse);

        }
    }
}
