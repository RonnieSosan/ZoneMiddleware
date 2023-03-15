using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.ProvidusImplementation.ProfileService;
using AppZoneMiddleware.Shared.Entites;
using Blend.ProvidusImplementation.NotificationService;

namespace Blend.ProvidusImplementation.CoreBankingService
{
    public class AccountServices : IAccountServices
    {
        ProvidusCBAMiddleware.BanksClient CBA_Client = new ProvidusCBAMiddleware.BanksClient();

        public Task<BlockFundResponse> BlockFund(BlockFundRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<FundAccountResponse> FundAccount(FundAccountRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<OpenAccountResponse> OpenAccount(OpenAccountRequest openAccount)
        {
            Logger.LogInfo("OpenAccountService.OpenAccount, input", openAccount);

            OpenAccountResponse openAccountResponse = new OpenAccountResponse();
            try
            {
                OpenAccountService.AccountClient addAccountServiceClient = new OpenAccountService.AccountClient();
                string jsonfordatgy = Newtonsoft.Json.JsonConvert.SerializeObject(openAccount);

                Logger.LogInfo("OpenAccountService.OpenAccount, abou to call service: ", addAccountServiceClient.Endpoint.Address.ToString());
                string responseMessage = addAccountServiceClient.addAccount(openAccount.ref_num, openAccount.bvn, openAccount.mobile_no, openAccount.email, openAccount.bra_code, openAccount.account_type, openAccount.surname, openAccount.first_name,
                    openAccount.account_name, openAccount.address_line1, openAccount.address_line2, openAccount.id_image_url, openAccount.photo_image_url, openAccount.date_of_birth, openAccount.gender);

                Logger.LogInfo("OpenAccountService.OpenAccount, response from service: ", responseMessage);

                if (responseMessage == "00")
                {
                    if (openAccount.MailRequest != null)
                    {
                        new MailService().SendMail(openAccount.MailRequest);
                    }
                    openAccountResponse.ResponseCode = "00";
                    openAccountResponse.ResponseDescription = "SUCCESSFUL";
                }

                else if (responseMessage == "01")
                {
                    openAccountResponse.ResponseCode = "01";
                    openAccountResponse.ResponseDescription = "REFERENCE NUMBER EXISTS";
                }

                else if (responseMessage == "02")
                {
                    openAccountResponse.ResponseCode = "02";
                    openAccountResponse.ResponseDescription = "INVALID BRANCH CODE";
                }

                else if (responseMessage == "03")
                {
                    openAccountResponse.ResponseCode = "03";
                    openAccountResponse.ResponseDescription = "UNKNOWN ERROR";
                }

            }
            catch (Exception ex)
            {
                openAccountResponse = new OpenAccountResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
            }

            Logger.LogInfo("OpenAccountService.OpenAccount, response: ", openAccountResponse);

            return Task<OpenAccountResponse>.Factory.StartNew(() => openAccountResponse);
        }

        public Task<PNDResponse> PlacePND(PNDRequest PNDRequest)
        {
            Logger.LogInfo("CBAService.PlacePND, input :", PNDRequest);

            PNDResponse response = null;
            try
            {
                // Do token validation
                string errormsg = string.Empty;
                string serializedTokenRequest = Newtonsoft.Json.JsonConvert.SerializeObject(PNDRequest);
                UserProfileResponse authResponse = new TokenManager().NonTransactionalTokenAuthentication(serializedTokenRequest);

                if (authResponse.ResponseCode == "06")
                {
                    PNDResponse failedResponse = new PNDResponse()
                    {
                        ResponseCode = authResponse.ResponseCode,
                        ResponseDescription = authResponse.ResponseDescription,
                    };

                    Newtonsoft.Json.JsonConvert.SerializeObject(failedResponse);
                    Logger.LogInfo("CBAService.PlacePND, failedResponse ", failedResponse);

                    return Task<PNDResponse>.Factory.StartNew(() => failedResponse);
                }
                // token valid

                string _phone = PNDRequest.PhoneNumber.Replace("+", "");

                PushNotification isecRequest = new PushNotification()
                {
                    account_number = PNDRequest.AccountNumber,
                    customer_id = Convert.ToString(PNDRequest.CustomerNumber),
                    purpose = "Place PND",
                    request_type = RequestType.transfer.ToString(),
                    transaction_amount = "0.00",
                    transaction_beneficiary = "",
                    phone_number = _phone.StartsWith("234") ? _phone : string.Format("234{0}", _phone.Substring(1, _phone.Length - 1)),

                };

                bool isAuthorized = PNDRequest.isMobile ? true : new ISECNotificationService().PushNotification(isecRequest, out string error);

                bool pndPlaced = false;

                if (isAuthorized)
                {
                    string serviceResponse = CBA_Client.getAccountWithPadding(PNDRequest.AccountNumber);
                    Logger.LogInfo("CBAService.PlacePND, ServiceResponse ", serviceResponse);

                    PNDRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<PNDRequest>(serviceResponse);
                    //pnd = new EntitySystem<PNDRequest>(ProcessorKey).GetEntity(queryString);
                    
                    if (RunBanksRestriction(PNDRequest, out errormsg))
                    {
                        pndPlaced = true;
                        response = new PNDResponse
                        {
                            ResponseCode = "00",
                            ResponseDescription = "Successful"
                        };
                        if (PNDRequest.MailRequest != null)
                        {
                            new MailService().SendMail(PNDRequest.MailRequest);
                        }
                    }
                }
                if (isAuthorized == false || pndPlaced == false)
                {
                    response = new PNDResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = errormsg
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            Logger.LogInfo("CBAService.PlacePND, response ", response);
            return Task<PNDResponse>.Factory.StartNew(() => response);
        }
       
        public Task<StopChequeResponse> StopCheque(StopChequeRequest stopCheque)
        {
            StopChequeResponse response;
            try
            {
                Logger.LogInfo("CBAService.StopCheque, input :", stopCheque);

                // Do token validation
                UserProfileResponse authResponse = new TokenManager().TransactionalTokenAuthentication(Newtonsoft.Json.JsonConvert.SerializeObject(stopCheque));

                Newtonsoft.Json.JsonConvert.SerializeObject(authResponse);
                Logger.LogInfo("CBAService.StopCheque, failedResponse ", authResponse);

                if (authResponse.ResponseCode == "06")
                {

                    response = new StopChequeResponse
                    {
                        ResponseCode = authResponse.ResponseCode,
                        ResponseDescription = authResponse.ResponseDescription
                    };
                    return Task<StopChequeResponse>.Factory.StartNew(() => response);
                }
                // token valid

                //string AccountQuery = string.Format(CommonUtility.Configuration.ConfigurationManager.ConvertAcctNumber.Replace(":ACCOUNT_NUMBER", "'{0}'"), request.AccountNumber);
                //string accountKey = new EntitySystem<object>(ProcessorKey).RetrieveBySpecificProperty(AccountQuery, "Account_Key");
                string accountKey = CBA_Client.get21DigitAccountNo(stopCheque.AccountNumber);

                decimal chkNumber = 0M;
                string RegID = "";
                decimal status = 0M;
                string outputMessage = "";
                string outDefInfo = "";
                decimal err = 0M;
                decimal reqLanInd = 1;
                decimal ChannelID = 1; // required from institution
                string ChannelRefNum = "123456"; //required from institiution
                decimal SourceCode = 30;


                string _phone = stopCheque.PhoneNumber.Replace("+", "");
                PushNotification request = new PushNotification()
                {
                    account_number = stopCheque.AccountNumber,
                    customer_id = stopCheque.customer_id,
                    purpose = "Transfer",
                    request_type = RequestType.transaction.ToString(),
                    transaction_amount = "",
                    transaction_beneficiary = "",
                    phone_number = _phone.StartsWith("234") ? _phone : string.Format("234{0}", _phone.Substring(1, _phone.Length - 1)),

                };

                bool isAuthorized = stopCheque.isMobile ? true : new ISECNotificationService().PushNotification(request, out outputMessage);

                if (isAuthorized)
                {
                    ProvidusPostingService.BanksMiddleware_PE07EDC00Client client = new ProvidusPostingService.BanksMiddleware_PE07EDC00Client();
                    client.e07edcd0(accountKey, SourceCode, chkNumber, ref reqLanInd, ref ChannelID, ref ChannelRefNum, ref RegID, ref status, ref outputMessage, ref outDefInfo, ref err);
                    client.Close();

                    Logger.LogInfo("CBAService.StopCheque, ACCOUNT_NUMBER= :", stopCheque.AccountNumber);
                    Logger.LogInfo("CBAService.StopCheque, accountKey= :", accountKey);
                    Logger.LogInfo("CBAService.StopCheque, App error :", outputMessage);
                    Logger.LogInfo("CBAService.StopCheque, status :", status);
                    Logger.LogInfo("CBAService.StopCheque, error :", err);

                }
                response = new StopChequeResponse
                {
                    ResponseCode = status == 0 ? "00" : "06",
                    ResponseDescription = status == 0 ? "Succesful" : outputMessage
                };

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                response = new StopChequeResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "Failed: " + ex.Message
                };
            }

            return Task<StopChequeResponse>.Factory.StartNew(() => response);
        }

        public Task<UnBlockFundResponse> UnBlockFund(UnBlockFundRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateBvnResponse> UpdateBvn(UpdateBVNRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UpgradeResponse> UpgradeAccount(UpgradeRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<UploadResponse> UploadDocument(UploadRequest request)
        {
            throw new NotImplementedException();
        }

        private Boolean RunBanksRestriction(PNDRequest pnd, out string ServiceMsg)
        {
            PNDRequest request = pnd;

            decimal fBraCode = request.BranchCode;
            decimal fCusNum = Convert.ToDecimal(request.CustomerNumber);
            decimal fCurCode = request.CurrencyCode;
            decimal fLedCode = request.LedgerCode;
            string fTextRest = request.Restriction_message;
            decimal fSubAcctCode = request.SubAccountCode;
            decimal fRestCode = request.RestrictionCode;

            string outReqId_out = "";
            decimal outRetStatus_out = 0;
            decimal reqLanInd_inout = 1;
            decimal inpChannelId_inout = 1;
            string inpChannelRefNum_inout = "1";
            decimal dbAppErr_out = 0;
            decimal outStatus_out = 0;
            string outDetInfo_out = "";
            decimal AcctRestSeq_out = 0;
            string outMsgTxt_out = "";

            PNDService.BanksMiddleware_PE04RES00Client client = new PNDService.BanksMiddleware_PE04RES00Client();
            client.e04res10(ref reqLanInd_inout, ref inpChannelId_inout, ref inpChannelRefNum_inout, fBraCode, fCusNum, fCurCode, fLedCode, fSubAcctCode, fRestCode,
                            fTextRest, ref AcctRestSeq_out, ref outRetStatus_out, ref outReqId_out, ref outStatus_out, ref outMsgTxt_out, ref outDetInfo_out, ref dbAppErr_out);

            Logger.LogInfo("RunBanksMiddleware.runBanksRestriction,outRetStatus_out= ", outRetStatus_out);
            Logger.LogInfo("RunBanksMiddleware.runBanksRestriction, request.RestrictionCode= ", request.RestrictionCode);


            client.Close();
            if (outRetStatus_out == 0)
            {
                ServiceMsg = outDetInfo_out;
                return true;
            }
            else
            {
                ServiceMsg = outDetInfo_out;
                return false;
            }
        }
    }
}
