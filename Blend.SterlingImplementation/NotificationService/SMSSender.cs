using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Newtonsoft.Json;
using Blend.SterlingImplementation.Entites;
using System.Xml.Serialization;
using System.IO;

namespace Blend.SterlingImplementation.NotificationService
{
    public class SMSSender : ISMSSender
    {
        int webServiceAppId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebServiceAppId"]);

        public Task<SMSResponse> Send(SMSRequest request)
        {
            Logger.LogInfo("SMSSender.Send, input", request);
            string PhoneNumber = string.Empty;
            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                PhoneNumber = GetAccount(request.customer_id).PHONE;
                if (PhoneNumber == null)
                {
                    Logger.LogInfo("SMSSender -> Send ", "No Phone Number found for CustID: " + request.customer_id);
                    return Task<SMSResponse>.Factory.StartNew(() => new SMSResponse { ResponseCode = "06", ResponseDescription = "No Phone Number found for CustID: " + request.customer_id });
                }
            }

            SMSResponse smsResponse = new SMSResponse();
            smsResponse = SendSMS(request.PhoneNumber, request.Message);
            return Task<SMSResponse>.Factory.StartNew(() => smsResponse);

        }

        private SMSResponse SendSMS(string phoneNumber, string message)
        {
            Logger.LogInfo("SMSSender.SendSMS, " + phoneNumber + " ", message);
            string clearedPhoneNumber = phoneNumber.Replace("+234", "0");
            SMSResponse smsResponse;
            SMSResponseXML xmlResponseObj = new SMSResponseXML();
            SMSRequestXML clientMessage = new SMSRequestXML()
            {
                ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                RequestType = "217",
                gsm = clearedPhoneNumber,
                Msg = message,
            };
            try
            {
                xmlResponseObj = new ServiceUtilities.IBSBridgeProcessor<SMSRequestXML, SMSResponseXML>().Processor(clientMessage, true) as SMSResponseXML;
                if (xmlResponseObj.ResponseCode == "00")
                {
                    smsResponse = new SMSResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = xmlResponseObj.ResponseText
                    };
                    Logger.LogInfo("SMSSender -> Send -> SendSMS ", clientMessage.ToString() + " Sent to SMS API");
                    Logger.LogInfo("SMSSender.Response", xmlResponseObj);
                }
                else
                {
                    smsResponse = new SMSResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = xmlResponseObj.ResponseText
                    };
                    Logger.LogInfo("SMSSender.Response", xmlResponseObj);
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                smsResponse = new SMSResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "An Error occured while sending sms."
                };
                Logger.LogInfo("SMSSender -> Send -> Exception ", ex.Message);
            }
            return smsResponse;
        }

        private AccountDetails GetAccount(string customerId)
        {
            string message = "Calling CBA Service for Accounts with CustID " + customerId;

            Logger.LogInfo("SMSSender -> Send -> GetAccount ", message);

            var queryResult = new CoreBankingService.AccountInquiryService().GetAccountsWithCustomerID(new AccountRequest { CustomerID = customerId });

            var accountDetail = queryResult.Result.AccountInformation.FirstOrDefault();

            return accountDetail;
        }
    }
}
