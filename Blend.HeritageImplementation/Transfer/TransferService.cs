using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Utility;
using Blend.DefaultImplementation.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blend.HeritageImplementation.Transfer
{
    public class TransferService : IBankTransfer
    {
        string transferURL = System.Configuration.ConfigurationManager.AppSettings.Get("LifestyleTransferURL");
        string chainMessageBroker = System.Configuration.ConfigurationManager.AppSettings.Get("ChainMessageBrokerURL");
        string base64Key = "2344asdfWPOuARSFUs10Lm==";
        public Task<FundsTransferResponse> InterBankTransfer(FundsTransferRequest TransferRequest)
        {
            throw new NotImplementedException();
        }

        public LifestyleDebitResponse LifeStyleDebit(LifestyleDebitRequest request)
        {
            Logger.LogInfo("TransferService.LifeStyleDebit.Input", request);
            LifestyleDebitResponse trxResponse = new LifestyleDebitResponse();

            var lifestyleTransferObj = new
            {
                AuthHeader = new
                {
                    UserName = "hbc_HBCM_Conet",
                    Password = "hBnG?_M06_t35t"
                },
                myLifestyleTrnsferRequest = new Utility.Crypto().EncryptString(JsonConvert.SerializeObject(new JObject
                {
                    { "SourceAccountNumber", request.AccountNumber },
                    { "uniqueIdentifier", request.CustomerNumber },
                    { "LifestylemerchantId" , request.MerchantId },
                    { "Narration", request.Narration },
                    { "RefId", request.RefId },
                    { "transactionPin", request.PIN },
                    { "channelName", "Mobile" },
                    { "Platform", "Blend" },
                    { "Amount", request.Amount },
                    { "Currency", request.Currency }
                }), base64Key)
            };
            var soapRequestMessage = new { OperationName = "BlendOmniInterfaceSoapClient", MethodName = "LifestyleTrsf", args = lifestyleTransferObj };

            Logger.LogInfo("TransferService.LifeStyleDebit.API request Message", soapRequestMessage);

            ApiProxyRequest httpContentMessage = new ApiProxyRequest
            {
                EndPointURL = transferURL,
                AuthData = new AuthenticationData { },
                Context = new PayLoadData { IsSoap = true, Headers = new PayLoadDataHeader[] { }, Body = JsonConvert.SerializeObject(soapRequestMessage) },
                HttpMethod = null,
            };

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage();

                    httpRequestMessage = new HttpRequestMessage() { RequestUri = new Uri(chainMessageBroker), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(httpContentMessage), Encoding.UTF8, "application/json") };

                    HttpResponseMessage httpResponse = client.SendAsync(httpRequestMessage).Result;
                    var rawResponse = httpResponse.Content.ReadAsStringAsync().Result;

                    Logger.LogInfo("TransferService.LifeStyleDebit.API response Message", rawResponse);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        try
                        {

                            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(rawResponse);
                            if (Convert.ToString(jsonObject["ResponseCode"]) == "00")
                            {
                                JObject heritageTransferResponse = JsonConvert.DeserializeObject<JObject>(new Utility.Crypto().DecryptString(Convert.ToString(jsonObject["Body"]), base64Key));
                                JObject lifestyleTransferResponse = JsonConvert.DeserializeObject<JObject>(Convert.ToString(heritageTransferResponse["StatusResponse"]));
                                trxResponse = new LifestyleDebitResponse { ResponseCode = Convert.ToString(lifestyleTransferResponse["ResponseCode"]), ResponseDescription = Convert.ToString(lifestyleTransferResponse["ResponseText"]) };
                            }
                            else
                            {
                                trxResponse = new LifestyleDebitResponse { ResponseCode = Convert.ToString(jsonObject["ResponseCode"]), ResponseDescription = Convert.ToString(jsonObject["ResponseDescription"]) };
                            }
                        }
                        catch (JsonException jsonEx)
                        { trxResponse = new LifestyleDebitResponse { ResponseCode = "06", ResponseDescription = jsonEx.Message }; }
                    }
                    else
                    {
                        // This will result in an Exception if the HTTP Status Code is not successful. 
                        try
                        {

                            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(rawResponse);
                            trxResponse = new LifestyleDebitResponse { ResponseCode = "06", ResponseDescription = $"Filed Server Response ({httpResponse.StatusCode}): {Convert.ToString(jsonObject["Message"])}" };
                        }
                        catch (Exception)
                        {
                            trxResponse = new LifestyleDebitResponse { ResponseCode = "06", ResponseDescription = rawResponse };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to process request");
            }

            return trxResponse;
        }

        public Task<IntraBankTransactionResponse> SameBankTransfer(IntraBankTransactionRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
