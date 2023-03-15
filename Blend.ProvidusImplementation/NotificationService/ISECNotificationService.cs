using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entites;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.ProvidusImplementation.CoreBankingService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blend.ProvidusImplementation.NotificationService
{
    public class ISECNotificationService : ISecondaryAuthentication
    {
        public Task<SecondaryAuthenticationResponse> CreateProfile(ProfieCreationrequest request)
        {
            Task<SecondaryAuthenticationResponse> profileCreationResp = null;

            try
            {
                Logger.LogInfo("ISECService.PushNotificationService,ProfileCreationNotification, input", request);
                
                //handle the enum conversion
                bool authenticated = false;
                string[] accounts = request.AccountNumber.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                string error = "";
                IntraBankTransactionRequest requestTran = new IntraBankTransactionRequest
                {
                    CreditAccount1 = request.CreditAccount,
                    DebitAccount = request.DebitAccount,
                    Amount = request.Amount,
                    Remakcs = request.Remarkcs,
                    RRN = "0",
                    STAN = "0",
                };

                //Post a tranx
                if (new FundTransferService().PostTran(requestTran, ref error))
                {
                    authenticated = true;
                    Logger.LogInfo("ISECService.PushNotificationService,ProfileCreationNotification, authenticated", authenticated);
                }
                else
                {
                    authenticated = true;
                }

                if (authenticated)
                {
                    //if mail is present, send a mail async
                    if (request.MailRequest != null)
                    {
                        new Task(() => { new MailService().SendMail(request.MailRequest); }).Start();
                    }
                    ISECProfieCreationrequest iSecPost = new ISECProfieCreationrequest
                    {
                        account_numbers = accounts, //iSecSetup.AccountNumber,
                        customer_id = request.customer_id,
                        first_name = request.FirstName,
                        last_name = request.LastName,
                        phone_number = request.PhoneNumber.StartsWith("234") ? request.PhoneNumber : string.Format("234{0}", request.PhoneNumber.Substring(1, request.PhoneNumber.Length - 1)),
                    };

                    string jsonMessage = Newtonsoft.Json.JsonConvert.SerializeObject(iSecPost);
                    Logger.LogInfo("ISECService.PushNotificationService, jsonMessage", jsonMessage);

                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    SecondaryAuthenticationResponse iSecResponse = null;
                    var baseAddress = System.Configuration.ConfigurationManager.AppSettings["2FAProfileURL"]; //"https://192.168.1.48:8181/profile/create/profile"; //"https://192.168.1.48:8181/creation/profile"; //"https://192.168.1.48:8181/profile-creation/authenticate";

                    using (var client = new HttpClient())
                    {

                        client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["iSectimeout"]));
                        var response = client.PostAsJsonAsync<ISECProfieCreationrequest>(baseAddress, iSecPost).Result;
                        iSecResponse = response.Content.ReadAsAsync<SecondaryAuthenticationResponse>().Result;
                    }

                    Logger.LogInfo("ISECService.PushNotificationService,stringResponse", Newtonsoft.Json.JsonConvert.SerializeObject(iSecResponse));

                    #region Reversal for already existing profile
                    if (iSecResponse.response_code == "05")
                    {

                        string Cr = requestTran.CreditAccount1;
                        string Dr = requestTran.DebitAccount;

                        requestTran.DebitAccount = Cr;
                        requestTran.CreditAccount1 = Dr;

                        if (new FundTransferService().PostTran(requestTran, ref error))
                        {
                            profileCreationResp = Task<SecondaryAuthenticationResponse>.Factory.StartNew(() =>
                            {
                                return new SecondaryAuthenticationResponse
                                {
                                    response_code = "00",
                                    response_message = "Account has already been profiled",
                                    PendingAction = "Do nothing"
                                };
                            });
                        }
                        else
                        {
                            profileCreationResp = Task<SecondaryAuthenticationResponse>.Factory.StartNew(() =>
                            {
                                return new SecondaryAuthenticationResponse
                                {
                                    response_code = "06",
                                    response_message = "Account has already been profiled, reversal failed",
                                    PendingAction = "Pending Reversal"
                                };
                            });
                        }
                    }
                    #endregion

                    #region Reversal for failed profile creation
                    if (iSecResponse.response_code != "00") //if anyone fails, post a reversal and return
                    {
                        string Cr = requestTran.CreditAccount1;
                        string Dr = requestTran.DebitAccount;

                        requestTran.DebitAccount = Cr;
                        requestTran.CreditAccount1 = Dr;

                        if (new FundTransferService().PostTran(requestTran, ref error))
                        {
                            profileCreationResp = Task<SecondaryAuthenticationResponse>.Factory.StartNew(() =>
                            {
                                return new SecondaryAuthenticationResponse
                                {
                                    response_code = "06",
                                    response_message = "Account profiling failed. Retry later",
                                    PendingAction = "Do nothing"
                                };
                            });
                        }
                        else
                        {
                            profileCreationResp = Task<SecondaryAuthenticationResponse>.Factory.StartNew(() =>
                            {
                                return new SecondaryAuthenticationResponse
                                {
                                    response_code = "06",
                                    response_message = "Account profiling failed. Retry later",
                                    PendingAction = "Pending Reversal"
                                };
                            });
                        }
                    } 
                    #endregion
                }
                else
                {
                    profileCreationResp = Task<SecondaryAuthenticationResponse>.Factory.StartNew(() =>
                    {
                        return new SecondaryAuthenticationResponse
                        {
                            response_code = "06",
                            response_message = "Account profiling failed unable to debit account. Retry later",
                            PendingAction = "Do nothing"
                        };
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                profileCreationResp = Task<SecondaryAuthenticationResponse>.Factory.StartNew(() =>
                {
                    return new SecondaryAuthenticationResponse
                    {
                        response_code = "06",
                        response_message = ex.Message
                    };
                });
            }
            return profileCreationResp;
        }

        public bool PushNotification(PushNotification request, out string ErrorMessage)
        {
            Logger.LogInfo("ISECService.PushNotificationService, input", request);

            ErrorMessage = "";

            try
            {
                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["ignore2FA"]))
                {
                    return true;
                }


                if (!string.IsNullOrEmpty(request.phone_number))
                {
                    request.phone_number = request.phone_number.Replace("+", "");
                    if (!request.phone_number.StartsWith("234"))
                    {
                        request.phone_number = string.Format("234{0}", request.phone_number.Substring(1, request.phone_number.Length - 1));
                    }
                }
                if (string.IsNullOrEmpty(request.request_type))
                {
                    request.request_type = RequestType.transaction.ToString();
                }

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                var baseAddress = System.Configuration.ConfigurationManager.AppSettings["PushNotificationURL"];

                Logger.LogInfo("ISECService.PushNotificationService, jsonMessage =", Newtonsoft.Json.JsonConvert.SerializeObject(request));

                SecondaryAuthenticationResponse content = null;

                using (var client = new HttpClient())
                {

                    client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["iSectimeout"]));
                    var response = client.PostAsJsonAsync<PushNotification>(baseAddress, request).Result;

                    content = response.Content.ReadAsAsync<SecondaryAuthenticationResponse>().Result;
                    content.response_message = content.response_code == "01" ? "Transaction Declined" : content.response_message;
                }

                Logger.LogInfo("ISECService.PushNotificationService, iSecResponse", Newtonsoft.Json.JsonConvert.SerializeObject(content));
                ErrorMessage = content.response_message;
                return content.response_code == "00";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ErrorMessage = ex.Message;
                return false;
            }
        }
    }
}
