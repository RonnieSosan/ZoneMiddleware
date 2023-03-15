using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using System.IO;
using System.Security.Cryptography;
using Blend.SharedServiceImplementation.ServiceUtilities;

namespace Blend.SharedServiceImplementation.Services
{
    public class AirtimeTopUp : IAirtimeTopup
    {
        //TODO: this is for LIVE
        //LoginID= "78969543";
        //PublicKey = "49076582"

        string LoginID = System.Configuration.ConfigurationManager.AppSettings.Get("LoginID"); // "56238865";
        readonly string PublicKey = System.Configuration.ConfigurationManager.AppSettings.Get("PublicKey");// "83785693";
        string SystemServiceID = System.Configuration.ConfigurationManager.AppSettings.Get("SystemServiceID");//  "2";
        string Email = System.Configuration.ConfigurationManager.AppSettings.Get("Email"); //"Pbcsupport@appzonegroup.com";
        readonly bool isDemo = bool.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("isDemo"));
        IBankTransfer _BankTransfer;

        public AirtimeTopUp(IBankTransfer bankTransfer)
        {
            _BankTransfer = bankTransfer;
        }

        public Task<AirtimeResponse> DoAirtimeTopUp(AirtimeRequest request)
        {
            Logger.LogInfo("MobinFnService.DoMobileTopup, input", request);

            AirtimeResponse response = null;
            string responseString = "";

            string trxChannel = request.isMobile ? "Mobile" : "Online";

            var trx = new LifestyleDebitRequest
            {
                AccountNumber = request.CustomerAccount,
                Amount = Convert.ToString(request.RechargeAmount),
                Narration = string.Format("{0}: Recharge <{1}> <{2}> ", trxChannel, request.BatchId, request.PhoneNumber),
                RequestChannel = trxChannel,
                MerchantId = "999030190522232343",
                PIN = request.PIN,
                CustomerNumber = request.CustomerNumber,
                Currency = request.Currency,
                RefId = DateTime.Now.Ticks.ToString()
            };
            Logger.LogInfo("MobinFnService.DoMobileTopup, trxRequestMessage", trx);
            LifestyleDebitResponse trxResponse = _BankTransfer.LifeStyleDebit(trx);

            Logger.LogInfo("MobinFnService.DoMobileTopup, trx response", trxResponse);

            if (trxResponse.ResponseCode != "00")
            {
                response = new AirtimeResponse
                {
                    Status = RechargeStatus.DebitFailed,
                    ResponseCode = "06",
                    ResponseDescription = "Debit failed: " + trxResponse.ResponseDescription,
                };
                return Task<AirtimeResponse>.Factory.StartNew(() => response);
            }
            else
            {
                if (isDemo)
                {
                    response = new AirtimeResponse
                    {
                        ConfirmationCode = "R171009.0748.210244",
                        AuditNo = "16151778",
                        Status = RechargeStatus.Successful,
                        ResponseCode = "00",
                        ResponseDescription = "Recharge Successful",
                    };
                    if (request.MailRequest != null)
                    {
                        Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailRequest>(Newtonsoft.Json.JsonConvert.SerializeObject(request.MailRequest)));
                    }
                    return Task<AirtimeResponse>.Factory.StartNew(() => response);
                }

            }

            try
            {
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidatePinFromExternalSource(request);
                if (!tokenValid)
                {
                    response = new AirtimeResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = "Pin Authentication Failed",
                    };
                    return Task<AirtimeResponse>.Factory.StartNew(() => response);
                }



                SystemServiceID = (request.ServiceType == ServiceType.MobileTopUp ? (int)ServiceType.DataBundle : (int)ServiceType.DataBundle).ToString();

                response = BeginRecharge(request.RechargeAmount, Convert.ToString((int)request.BatchId), request.PhoneNumber, SystemServiceID);

                Logger.LogInfo("MobifnService.DoMobileTopUp, MobifnResponse", response.ResponseDescription);

                if (response.ResponseCode == "000" || response.ResponseCode == "00")
                {
                    if (request.MailRequest != null)
                    {
                        Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailRequest>(Newtonsoft.Json.JsonConvert.SerializeObject(request.MailRequest)));
                    }
                    response.Status = RechargeStatus.Successful;
                }
                else
                {
                    response.Status = RechargeStatus.PendingReversal; //Recharge failed, reversal failed. Log reversal and retry later
                    response.ResponseDescription = "Top up failed, Reversal failed.";
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new AirtimeResponse
                {
                    Status = RechargeStatus.DebitFailed,
                    ResponseCode = "06",
                    ResponseDescription = "Could not process request",
                };
                Logger.LogError(ex);
                return Task<AirtimeResponse>.Factory.StartNew(() => response);
            }
            responseString = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            Logger.LogInfo("MobifnService.DoMobileTopUp, response", responseString);
            return Task<AirtimeResponse>.Factory.StartNew(() => response);
        }

        private AirtimeResponse BeginRecharge(decimal RechargeAmount, string BatchId, string PhoneNumber, string SystemServiceID)
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

            AirtimeTopup.FixRecharge_Request dataRequest = new AirtimeTopup.FixRecharge_Request
            {
                SystemServiceID = SystemServiceID,
                BatchId = BatchId,
                Email = Email,
                LoginId = LoginID,
                FromANI = PhoneNumber,
                ReferalNumber = PhoneNumber,
                RequestId = RequestId,
            };

            AirtimeTopup.FlexiRecharge_Response airtimeResponse = null;
            AirtimeTopup.FixRecharge_Response dataResponse = null;
            try
            {
                string checkSum = GenerateCheckSumForRechargeRequest(airRequest);

                if (SystemServiceID == ((int)ServiceType.DataBundle).ToString())
                {
                    dataRequest.Checksum = checkSum;
                    dataResponse = client.FixRecharge(dataRequest);

                    response = new AirtimeResponse
                    {
                        ResponseCode = dataResponse.ResponseCode,
                        ResponseDescription = dataResponse.ResponseDescription,
                        // ProviderResponse = airtimeResponse.ProviderResponse,
                        ConfirmationCode = dataResponse.ConfirmationCode,
                        AuditNo = dataResponse.AuditNo
                    };

                }
                else
                {
                    airRequest.Checksum = checkSum;
                    airtimeResponse = client.FlexiRecharge(airRequest);

                    response = new AirtimeResponse
                    {
                        ResponseCode = airtimeResponse.ResponseCode,
                        ResponseDescription = airtimeResponse.ResponseDescription,
                        // ProviderResponse = airtimeResponse.ProviderResponse,
                        ConfirmationCode = airtimeResponse.ConfirmationCode,
                        AuditNo = airtimeResponse.AuditNo
                    };

                }


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
