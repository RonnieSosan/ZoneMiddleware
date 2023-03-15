using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;

namespace AppZoneMiddleware.Shared.Services
{
    public class AirtimeTopup : IAirtimeTopup
    {
        string LoginID = System.Configuration.ConfigurationManager.AppSettings["LoginID"]; // "56238865";
        string PublicKey = System.Configuration.ConfigurationManager.AppSettings["PublicKey"];// "83785693";
        string ANIValue = "";
        string SystemServiceID = System.Configuration.ConfigurationManager.AppSettings["SystemServiceID"];//  "2";
        string Email = System.Configuration.ConfigurationManager.AppSettings["Email"]; //"Pbcsupport@appzonegroup.com";

        public Task<AirtimeResponse> DoAirtimeTopUp(AirtimeRequest request)
        {
            Logger.LogInfo("MobinFnService.DoMobileTopup, input", input);

            AirtimeResponse response = null;
            string responseString = "";

            if (request.ZoneOriginated)
            {
                Logger.LogInfo("BillPaymentService.DOBillsPayement", "About to post to recharge for Zone |" + input);

                response = BeginRecharge(request.RechargeAmount, Convert.ToString((Int32)request.BatchId), request.PhoneNumber);
                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                return responseString;
            }
            string trxChannel = request.isMobile ? "Mobile" : "Online";

            //call providusAPI to do posting for providus recharge request
            ProvidusPrimeAPI.ProvidusPrimeMiddlewareSoapClient api = new ProvidusPrimeAPI.ProvidusPrimeMiddlewareSoapClient();
            ProvidusTransactionRequest postingRequest = new ProvidusTransactionRequest
            {

                DebitAccount = request.CustomerAccount,
                CreditAccount1 = request.RechargeSuspenceAccount,
                FromAccountName = request.CustomerAccountName,
                ToAccountName = "Not Applicable",
                Amount = request.RechargeAmount / 100, // change from Kobo to naira
                Remakcs = string.Format("{0}: Recharge <{1}> <{2}> ", trxChannel, request.BatchId, request.PhoneNumber),
                STAN = "21",
                RRN = "11",
                PhoneNumber = request.PhoneNumber,
                customer_id = request.customer_id,
                PIN = request.PIN,
                AuthToken = request.AuthToken,
                isMobile = request.isMobile,
            };

            string postingRequestString = Newtonsoft.Json.JsonConvert.SerializeObject(postingRequest);
            int timeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["iSectimeout"]);
            api.InnerChannel.OperationTimeout = new TimeSpan(0, timeout, 20); //2min10sec
            string postingResponseString = "";
            try
            {
                postingResponseString = api.SameBankTransfer(postingRequestString);

                ProvidusTransactionResponse postingResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ProvidusTransactionResponse>(postingResponseString);

                Logger.LogInfo("MobiFnService.DOMobileTopup", "tranResponse: " + postingResponse.ResponseDescription);

                //if debit is successful, call mobifn to top up.
                if (postingResponse.ResponseCode == "00")
                {
                    Logger.LogInfo("MobiFnService.DOMobileTopup", "About to recharge");

                    response = BeginRecharge(request.RechargeAmount, Convert.ToString((int)request.BatchId), request.PhoneNumber);

                    Logger.LogInfo("MobifnService.DoMobileTopUp, MobifnResponse", response.ResponseDescription);

                    if (response.ResponseCode == "000" || response.ResponseCode == "00")
                    {
                        response.Status = RechargeStatus.Successful;
                    }
                    else //post reversal, reverse the DR/CR accounts. Attempt only once.
                    {
                        ProvidusTransactionRequest reversalRequest = new ProvidusTransactionRequest
                        {
                            DebitAccount = request.RechargeSuspenceAccount,
                            CreditAccount1 = request.CustomerAccount,
                            Amount = request.RechargeAmount / 100,  //change from Kobo to naira
                            Remakcs = string.Format("Rev:Online: Recharge <{0}> <{1}> ", request.BatchId, request.PhoneNumber),
                            STAN = "0",
                            RRN = "0",
                            TranType = TranType.Reversal,
                            PhoneNumber = request.PhoneNumber,
                            customer_id = request.customer_id,
                            PIN = request.PIN,
                            AuthToken = request.AuthToken,
                            isMobile = request.isMobile,
                        };
                        string reversalRequestString = Newtonsoft.Json.JsonConvert.SerializeObject(reversalRequest);
                        string reversalResponseString = api.SameBankTransfer(reversalRequestString);
                        ProvidusTransactionResponse reversalResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ProvidusTransactionResponse>(reversalResponseString);

                        if (reversalResponse.ResponseCode == "00")
                        {
                            response.Status = RechargeStatus.RechargeFailed; //Recharge failed, take no further action
                                                                             // response.ResponseDescription += "||Unable to contact Telco to Top up. Please retry after some time.";
                            response.ResponseDescription = "Unable to complete top up request. Kindly retry";
                        }
                        else
                        {
                            response.Status = RechargeStatus.PendingReversal; //Recharge failed, reversal failed. Log reversal and retry later
                            response.ResponseDescription = "Top up failed, Reversal failed.";
                        }
                    }
                }
                else
                {
                    response = new AirtimeResponse
                    {
                        Status = RechargeStatus.DebitFailed,
                        ResponseCode = postingResponse.ResponseCode,
                        ResponseDescription = "unable to post debit: " + postingResponse.ResponseDescription,
                    };
                }
                api.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new AirtimeResponse
                {
                    Status = RechargeStatus.DebitFailed,
                    ResponseCode = "06",
                    ResponseDescription = "Could not debit account",
                };
                responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                Logger.LogError(ex);
                return responseString;
            }
            responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            Logger.LogInfo("MobifnService.DoMobileTopUp, response", responseString);
            return responseString;
        }
    }
}
