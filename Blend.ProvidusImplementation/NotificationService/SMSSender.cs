using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.ProvidusImplementation.CoreBankingService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blend.ProvidusImplementation.NotificationService
{
    public class SMSSender : ISMSSender
    {
        public Task<SMSResponse> Send(SMSRequest request)
        {
            Logger.LogInfo("SMSSender.Send, input", request.Message);
            AccountDetails customerAccount = GetAccount(request.customer_id);

            if (customerAccount != null)
            {
                SendSMS(customerAccount.PHONE, request.Message);
                return Task<SMSResponse>.Factory.StartNew(() => new SMSResponse { ResponseCode = "00", ResponseDescription = "SUCCESSFUL" });
            }
            else
            {
                Logger.LogInfo("SMSSender -> Send ", "No Accounts found for CustID: " + request.customer_id);
                return Task<SMSResponse>.Factory.StartNew(() => new SMSResponse { ResponseCode = "06", ResponseDescription = "No Accounts found for CustID: " + request.customer_id });
            }
        }

        private void SendSMS(string phoneNumber, string message)
        {
            Logger.LogInfo("SMSSender.SendSMS, " + phoneNumber + " ", message);
            var SMSApi = ConfigurationManager.AppSettings["SMSAPI"];

            string clearedPhoneNumber = phoneNumber.Replace("+234", "0");
            Logger.LogInfo("SMSSender.SendSMS, api URL", SMSApi);
            try
            {
                using (var client = new HttpClient())
                {
                    var payload = new { recipient = clearedPhoneNumber, message };

                    var payloadString = JsonConvert.SerializeObject(payload);

                    Logger.LogInfo("SMSSender -> Send -> SendSMS ", "Sending " + payloadString + " to SMS API");
                    var response = client.PostAsJsonAsync(SMSApi, payload).Result;

                    Logger.LogInfo("SMSSender.ResponseStatusCode", response.StatusCode.ToString());

                    if (response.IsSuccessStatusCode)
                    {
                        Logger.LogInfo("SMSSender -> Send -> SendSMS ", payloadString + " Sent to SMS API");
                        Logger.LogInfo("SMSSender.Response", response.Content.ReadAsAsync<string>().Result);
                    }
                    else
                    {
                        Logger.LogInfo("SMSSender.Response", response.Content.ReadAsAsync<string>().Result);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogInfo("SMSSender -> Send -> Exception ", ex.Message);
            }
        }


        private AccountDetails GetAccount(string customerId)
        {
            var CBAQuery = new { CustomerID = customerId };

            string message = "Calling CBA Service for Accounts with CustID " + customerId;

            Logger.LogInfo("SMSSender -> Send -> GetAccount ", message);

            string queryJsonString = JsonConvert.SerializeObject(CBAQuery);

            var queryResult = new CoreBankingService.AccountInquiryService().GetAccountsWithCustomerID(new AccountRequest { CustomerID = customerId });

            var accountDetail = queryResult.Result.AccountInformation.FirstOrDefault();

            return accountDetail;
        }
    }
}
