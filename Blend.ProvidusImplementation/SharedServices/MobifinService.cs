using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using System.Security.Cryptography;
using System.IO;
using System.Net.Http;
using Blend.ProvidusImplementation.Custom_Service_Reference;

namespace Blend.ProvidusImplementation.SharedServices
{
    public class MobifinService : IAirtimeTopup
    {
        //TODO: this is for LIVE
        //LoginID= "78969543";
        //PublicKey = "49076582"

        string LoginID = System.Configuration.ConfigurationManager.AppSettings.Get("LoginID"); // "56238865";
        string PublicKey = System.Configuration.ConfigurationManager.AppSettings.Get("PublicKey");// "83785693";
        string SystemServiceID = System.Configuration.ConfigurationManager.AppSettings.Get("SystemServiceID");//  "2";
        string Email = System.Configuration.ConfigurationManager.AppSettings.Get("Email"); //"Pbcsupport@appzonegroup.com";

        public Task<AirtimeResponse> DoAirtimeTopUp(AirtimeRequest request)
        {
            Logger.LogInfo("MobinFnService.DoMobileTopup, input", request);

            AirtimeResponse response = null;
            string responseString = "";

            if (request.ZoneOriginated)
            {
                Logger.LogInfo("BillPaymentService.DOBillsPayement", "About to post to recharge for Zone |" + request);

                response = BeginRecharge(request.RechargeAmount, Convert.ToString((Int32)request.BatchId), request.PhoneNumber);
                return Task<AirtimeResponse>.Factory.StartNew(() => response);
            }
            string trxChannel = request.isMobile ? "Mobile" : "Online";

            AppzoneApiProcessor api = new AppzoneApiProcessor("Blend", "Transfer", "SameBankTransfer");
            IntraBankTransactionRequest postingRequest = new IntraBankTransactionRequest
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
    
            string postingResponseString = "";
            try { 

                postingResponseString = api.CallService<IntraBankTransactionResponse>(postingRequestString);

                IntraBankTransactionResponse postingResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<IntraBankTransactionResponse>(postingResponseString);

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
                        IntraBankTransactionRequest reversalRequest = new IntraBankTransactionRequest
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
                        string reversalResponseString = api.CallService<IntraBankTransactionResponse>(reversalRequestString);
                        IntraBankTransactionResponse reversalResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<IntraBankTransactionResponse>(reversalResponseString);

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
                Logger.LogError(ex);
                return Task<AirtimeResponse>.Factory.StartNew(() => response);
            }
            responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            Logger.LogInfo("MobifnService.DoMobileTopUp, response", responseString);
            return Task<AirtimeResponse>.Factory.StartNew(() => response);
        }

        private AirtimeResponse BeginRecharge(decimal RechargeAmount, string BatchId, string PhoneNumber)
        {
            Logger.LogInfo("MobifnService.BeginRecharge, PhoneNumber", PhoneNumber);

            AirtimeResponse response = null;

            //Do airtime Top up
            AirtimeTopup.Reseller_iTopUp_wsdlPortClient client = new AirtimeTopup.Reseller_iTopUp_wsdlPortClient();

            string RequestId = string.Format("{0}a", NumberSeries());

            AirtimeTopup.FlexiRecharge_Request airRequest = new AirtimeTopup.FlexiRecharge_Request
            {
                Amount = RechargeAmount.ToString(), // expected to be inkobo
                Email = Email,
                LoginId = LoginID,
                BatchId = BatchId,
                FromANI = PhoneNumber,
                ReferalNumber = PhoneNumber,
                RequestId = RequestId,
                SystemServiceID = SystemServiceID
            };

            AirtimeTopup.FlexiRecharge_Response airtimeResponse = null;

            try
            {
                airRequest.Checksum = GenerateCheckSumForRechargeRequest(airRequest);
                airtimeResponse = client.FlexiRecharge(airRequest);
                response = new AirtimeResponse
                {
                    ResponseCode = airtimeResponse.ResponseCode,
                    ResponseDescription = airtimeResponse.ResponseDescription,
                    // ProviderResponse = airtimeResponse.ProviderResponse,
                    ConfirmationCode = airtimeResponse.ConfirmationCode,
                    AuditNo = airtimeResponse.AuditNo
                };

                Logger.LogInfo("MobiFnService.BeginRecharge", " recharge response " + airtimeResponse.ResponseCode);
            }
            catch (Exception ex) //incase of error
            {
                var queryRequest = new AirtimeTopup.FlexiTransactionDetail_Request
                {
                    Checksum = airRequest.Checksum,
                    LoginId = LoginID,
                    RequestId = RequestId
                };
                client.Close();

                client = new AirtimeTopup.Reseller_iTopUp_wsdlPortClient();
                var RechargeQuery = client.FlexiTransactionDetail(queryRequest);

                response = new AirtimeResponse
                {
                    ResponseCode = RechargeQuery.ResponseCode,
                    ResponseDescription = RechargeQuery.ResponseDescription,
                    // ProviderResponse = airtimeResponse.ProviderResponse,
                    ConfirmationCode = RechargeQuery.ConfirmationCode,
                    AuditNo = RechargeQuery.AuditNo
                };
                Logger.LogInfo("MobiFnService.FailureConfirmation", " recharge Failed Confirmation " + airtimeResponse.ResponseCode + " " + airtimeResponse.ResponseDescription);
                Logger.LogError(ex);
            }
            finally
            {
                client.Close();
            }

            return response;
        }

        long NumberSeries()
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["RequestIDTracker"];
            long lastId = 0;
            using (StreamReader reader = new StreamReader(path))
            {
                Int64.TryParse(reader.ReadLine(), out lastId);
                lastId++;
            }
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(lastId);
            }
            return lastId;
            // return Interlocked.Increment(ref orderNumber);
        }

        private string GenerateCheckSumForBalanceRequest(AirtimeTopup.ResellerBalance_Request req)
        {
            string toReturn = string.Empty;
            string toUse = String.Format("{0}|{1}|{2}", LoginID, req.TillDate, PublicKey);
            toUse = DoSHA1Hash(toUse).ToLower();
            toUse = DoMD5Hash(toUse).ToLower();
            toReturn = toUse;
            return toReturn;
        }

        string GenerateCheckSumForRechargeRequest(AirtimeTopup.FlexiRecharge_Request request)
        {
            string toReturn = string.Empty;

            string toUse = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", request.LoginId, request.RequestId, request.BatchId, request.SystemServiceID, request.ReferalNumber, request.Amount, request.FromANI, request.Email, PublicKey);
            Logger.LogInfo("GenerateCheckSumForRechargeRequest", toUse);
            toUse = DoSHA1Hash(toUse).ToLower();
            toUse = DoMD5Hash(toUse).ToLower();
            toReturn = toUse;
            return toReturn;
        }

        string DoSHA1Hash(string clear)
        {
            byte[] clearBytes;
            byte[] computedHash;

            clearBytes = ASCIIEncoding.ASCII.GetBytes(clear);
            computedHash = new SHA1CryptoServiceProvider().ComputeHash(clearBytes);

            return ByteArrayToString(computedHash);
        }
        private string ByteArrayToString(byte[] array)
        {

            StringBuilder output = new StringBuilder(array.Length);

            for (int index = 0; index < array.Length; index++)
            {
                output.Append(array[index].ToString("X2"));
            }
            return output.ToString();
        }
        string DoMD5Hash(string clear)
        {
            byte[] clearBytes;
            byte[] computedHash;

            clearBytes = ASCIIEncoding.ASCII.GetBytes(clear);
            computedHash = new MD5CryptoServiceProvider().ComputeHash(clearBytes);

            return ByteArrayToString(computedHash);
        }
 
    }
}
