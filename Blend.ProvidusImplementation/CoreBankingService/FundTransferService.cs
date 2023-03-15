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
using System.Diagnostics;
using System.IO;
using Blend.ProvidusImplementation.NotificationService;

namespace Blend.ProvidusImplementation.CoreBankingService
{
    public class FundTransferService : IBankTransfer
    {
        bool isDemo = false;
        bool useDailyLimit = false;
        string providusBankCode = System.Configuration.ConfigurationManager.AppSettings.Get("ProvidusBankCode");
        string oldBankCode = "999037";
        double MobileDailyLimit = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings.Get("MobileDailyLlimit"));
        public FundTransferService()
        {
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("Isdemo"), out isDemo);
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("UseDailyLimit"), out useDailyLimit);
        }

        public NFPOutwardService.AuthHeader authHeather = new NFPOutwardService.AuthHeader()
        {
            Password = System.Configuration.ConfigurationManager.AppSettings.Get("NIPPassword"),
            UserName = System.Configuration.ConfigurationManager.AppSettings.Get("NIPUserName"),
        };
        ProvidusCBAMiddleware.BanksClient CBA_Client = new ProvidusCBAMiddleware.BanksClient();

        public Task<FundsTransferResponse> InterBankTransfer(FundsTransferRequest TransferRequest)
        {
            Logger.LogInfo("NIPService.DoFundTransfer, input =", TransferRequest);

            FundsTransferResponse transferResponse = null;
            try
            {
                #region NewNFP Implementation
                NFPOutwardService.NFPOutwardService_BranchSoapClient client = new NFPOutwardService.NFPOutwardService_BranchSoapClient();
                #endregion


                // Do token validation
                UserProfileResponse authResponse = new TokenManager().TransactionalTokenAuthentication(Newtonsoft.Json.JsonConvert.SerializeObject(TransferRequest));

                if (authResponse.ResponseCode == "06")
                {
                    FundsTransferResponse response = new FundsTransferResponse()
                    {
                        ResponseCode = authResponse.ResponseCode,
                        ResponseDescription = authResponse.ResponseDescription,
                    };

                    Logger.LogInfo("NIPService.DoFundTransfer, failedResponse ", response);

                    return Task<FundsTransferResponse>.Factory.StartNew(() => response);
                }

                // token valid
                NameInquiryRequest nameEnqRequest = new NameInquiryRequest()
                {
                    isMobile = TransferRequest.isMobile,
                    myChannelCode = TransferRequest.ChannelCode,
                    DestinationAccountNumber = TransferRequest.ToAccount,
                    myDestinationBankCode = TransferRequest.BeneficiaryBank,
                    AuthToken = TransferRequest.AuthToken,
                    PIN = TransferRequest.PIN,
                    myAccountNumber = TransferRequest.FromAccount,
                    customer_id = TransferRequest.customer_id,
                };

                //do name enq for corresponding fund transfer
                NameInquiryResponse nameEnqResponse = new AccountInquiryService().InterBankNameInquiry(nameEnqRequest).Result;
                if (nameEnqResponse.ResponseCode != "00")
                {
                    transferResponse = new FundsTransferResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Account Number invalid",
                    };

                    string failedNameEnq = Newtonsoft.Json.JsonConvert.SerializeObject(transferResponse);
                    Logger.LogInfo("NIPService.GetNIBBSCharge, response = ", failedNameEnq);
                    return Task<FundsTransferResponse>.Factory.StartNew(() => transferResponse); ;
                }
                TransferRequest.BeneficiaryName = nameEnqResponse.AccountName;
                Logger.LogInfo("NIPService.DoFundTransfer, BenefitiaryNameResponse ", TransferRequest.BeneficiaryName);

                string trxChannel = TransferRequest.isMobile ? "Mobile" : "Online";
                TransferRequest.ChannelCode = TransferRequest.isMobile ? "03" : "02";
                //string narrationFormat = string.Format("{0}: NIP Transfer from {1} to {2}  {3}  payRef: {4}   {5} ", trxChannel, fundsTransfer.OriginatorName, fundsTransfer.BeneficiaryName, fundsTransfer.BeneficiaryBankName, fundsTransfer.MyPaymentReference, fundsTransfer.Remarks); //Online: NIP Transfer < Remarks >
                string narrationFormat = string.Format("{0}: To {1}|{2} {3} ", trxChannel, TransferRequest.BeneficiaryBankName, TransferRequest.BeneficiaryName, TransferRequest.Remarks); //Online: NIP Transfer < Remarks >;
                TransferRequest.Narration = string.IsNullOrEmpty(TransferRequest.Narration) ? narrationFormat : TransferRequest.Narration;


                Logger.LogInfo("NIPService.DoFundTransfer, Narration Of Transfer: ", TransferRequest.Narration);

                string _phone = TransferRequest.PhoneNumber.Replace("+", "");
                PushNotification request = new PushNotification()
                {
                    account_number = TransferRequest.FromAccount,
                    customer_id = TransferRequest.customer_id,
                    purpose = "Transfer",
                    request_type = RequestType.transfer.ToString(),
                    transaction_amount = TransferRequest.Amount.ToString(),
                    transaction_beneficiary = TransferRequest.ToAccount,
                    phone_number = _phone.StartsWith("234") ? _phone : string.Format("234{0}", _phone.Substring(1, _phone.Length - 1)),

                };
                string error = "";
                bool isAuthorized = TransferRequest.isMobile ? true : new ISECNotificationService().PushNotification(request, out error);
                //send 2FA here            
                if (isAuthorized)
                {
                    if (TransferRequest.BeneficiaryBank == providusBankCode || TransferRequest.BeneficiaryBank == oldBankCode)
                    {
                        IntraBankTransactionRequest pRequest = new IntraBankTransactionRequest
                        {
                            Amount = decimal.Parse(TransferRequest.Amount),
                            CreditAccount1 = TransferRequest.ToAccount,
                            DebitAccount = TransferRequest.FromAccount,
                            TranType = TranType.CustomerPosting,
                            RRN = "0",
                            STAN = "0"
                        };
                        if (PostTran(pRequest, ref error))
                        {
                            transferResponse = new FundsTransferResponse
                            {
                                ResponseCode = "00",
                                PaymentRef = "",
                                ResponseDescription = "sucessful"
                            };
                        }
                    }
                    else
                    {
                        #region NEW TRANSFER IMPLEMENTATION FOR NIP

                        //Check customers transaction limit
                        if (useDailyLimit && string.IsNullOrEmpty(TransferRequest.DailyLimit.ToString()))
                        {
                            ProvidusCBAMiddleware.BanksClient cbaClient = new ProvidusCBAMiddleware.BanksClient();
                            var customersNipTotalTrnx = cbaClient.getTotalNIPOutward(TransferRequest.ToAccount) + Convert.ToDouble(TransferRequest.Amount);

                            Logger.LogInfo("NIPService.DoFundTransfer, Customer Net NIP ", customersNipTotalTrnx);

                            if (TransferRequest.isMobile && customersNipTotalTrnx > MobileDailyLimit)
                            {
                                Logger.LogInfo("NIPService.DoFundTransfer, NIP client Mobile", "Transfer Limit Reached");
                                transferResponse = new FundsTransferResponse
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "Transfer Limit Reached",
                                };

                                string limitReachedResult = Newtonsoft.Json.JsonConvert.SerializeObject(transferResponse);
                                Logger.LogInfo("NIPService.GetNIBBSCharge, response = ", limitReachedResult);
                                return Task<FundsTransferResponse>.Factory.StartNew(() => transferResponse); ;
                            }
                            else
                            {

                                if (customersNipTotalTrnx > Convert.ToDouble(TransferRequest.DailyLimit))
                                {
                                    Logger.LogInfo("NIPService.DoFundTransfer, NIP client ", "Transfer Limit Reached");
                                    transferResponse = new FundsTransferResponse
                                    {
                                        ResponseCode = "06",
                                        ResponseDescription = "Transfer Limit Reached",
                                    };

                                    string limitReachedResult = Newtonsoft.Json.JsonConvert.SerializeObject(transferResponse);
                                    Logger.LogInfo("NIPService.GetNIBBSCharge, response = ", limitReachedResult);
                                    return Task<FundsTransferResponse>.Factory.StartNew(() => transferResponse); ;
                                }
                            }
                        }

                        Logger.LogInfo("NIPService.DoFundTransfer, NIP client ", client.Endpoint.Address);
                        Logger.LogInfo("NIPService.DoFundTransfer, About to call NIP TRANSFER ", TransferRequest.ChannelCode);
                        //Additional field benefitiaryBankName
                        string resposne = client.RIB_Fundtransfersingleitem_dc(authHeather, TransferRequest.BeneficiaryBank, TransferRequest.ChannelCode,
                                                    TransferRequest.ToAccount, TransferRequest.BeneficiaryName, TransferRequest.OriginatorName, TransferRequest.Narration,
                                                        TransferRequest.MyPaymentReference, TransferRequest.Amount, TransferRequest.FromAccount);

                        Logger.LogInfo("NIPService.DoFundTransfer, NIP Response ", resposne);
                        client.Close();
                        #endregion
                        if (string.IsNullOrEmpty(resposne))
                        {
                            transferResponse = new FundsTransferResponse
                            {
                                ResponseCode = "06",
                                PaymentRef = "",
                                ResponseDescription = "FAILED: No response from server",
                            };
                        }
                        else
                        {
                            string[] NIPResponse = resposne.Split('|');

                            Logger.LogInfo("NIPService.DoFundTransfer, resposne =", resposne);

                            if (NIPResponse[0] == "00")
                            {
                                transferResponse = new FundsTransferResponse
                                {
                                    ResponseCode = NIPResponse[0],
                                    PaymentRef = string.IsNullOrEmpty(NIPResponse[1]) ? "" : NIPResponse[1],
                                    ResponseDescription = "sucessful"
                                };
                            }
                            if (NIPResponse[0] == "61")
                            {
                                transferResponse = new FundsTransferResponse
                                {
                                    ResponseCode = NIPResponse[0],
                                    PaymentRef = NIPResponse[1],
                                    ResponseDescription = "Limit Exceeded"
                                };
                            }
                            else
                            {
                                transferResponse = new FundsTransferResponse
                                {
                                    ResponseCode = NIPResponse[0],
                                    PaymentRef = NIPResponse[1],
                                    ResponseDescription = "FAILED"
                                };
                            }
                        }
                    }
                }

                if (transferResponse == null)
                {
                    transferResponse = new FundsTransferResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = error ?? "Unable to send"
                    };
                }
            }
            catch (Exception ex)
            {
                transferResponse = new FundsTransferResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
                Logger.LogError(ex);
            }
            return Task<FundsTransferResponse>.Factory.StartNew(() => transferResponse); ;
        }

        public Task<IntraBankTransactionResponse> SameBankTransfer(IntraBankTransactionRequest transferRequest)
        {
            Logger.LogInfo("CBAService.DoBankTransfer, input", transferRequest);
           
            Task<IntraBankTransactionResponse> AuthResponse = null;
            UserProfileResponse authResponse = AuthenticateSameBankTransfer(transferRequest);

            if (authResponse.ResponseCode == "06")
            {
                AuthResponse = Task<IntraBankTransactionResponse>.Factory.StartNew(() =>
                {
                    return new IntraBankTransactionResponse
                    {
                        ResponseCode = authResponse.ResponseCode,
                        ResponseDescription = authResponse.ResponseDescription,
                    };
                });
                Logger.LogInfo("CBAService.DoBankTransfer, failedResponse ", AuthResponse);

                return AuthResponse;
            }

            // token valid
            string narrationFormat = string.Empty;

            string trxChannel = transferRequest.isMobile ? "Mobile" : "Online";
            if (transferRequest.Remakcs.Contains("Recharge"))
            {
                narrationFormat = string.Format("{0} on {1}  {2}", transferRequest.Remakcs, transferRequest.FromAccountName, transferRequest.DebitAccount); //Online: Trf from <Initiating Account> to <Beneficiary Account>/<Remarks>
            }
            else
            {
                narrationFormat = string.Format("{0}: Trf from {1}  {2}  to  {3} - {4}   {5}", trxChannel, transferRequest.FromAccountName, transferRequest.DebitAccount, transferRequest.ToAccountName, transferRequest.CreditAccount1, transferRequest.Remakcs); //Online: Trf from <Initiating Account> to <Beneficiary Account>/<Remarks>
            }

            Logger.LogInfo("CBAService.DoBankTransfer, TransactionNarration: ", narrationFormat);

            transferRequest.Narration = transferRequest.Narration == string.Empty || transferRequest.Narration == null ? narrationFormat : transferRequest.Narration;
            string error = "";
            string ResponseCode = "06";
            string ResponseDescription = "unable to Post";
            bool isAuthorized = false;
            bool result = false;

            switch (transferRequest.TranType)
            {
                case TranType.GLPosting:
                case TranType.Reversal:
                    isAuthorized = true;
                    Logger.LogInfo("CBAService.DOBankTransfer", "no authentication");
                    result = PostTran(transferRequest, ref error);
                    break;

                default:
                    //send 2FA here
                    if (!string.IsNullOrEmpty(transferRequest.PhoneNumber))
                    {
                        string _phone = transferRequest.PhoneNumber.Replace("+", "");
                        PushNotification request = new PushNotification()
                        {
                            account_number = transferRequest.DebitAccount,
                            customer_id = transferRequest.customer_id,
                            purpose = "Transfer",
                            request_type = RequestType.transfer.ToString(),
                            transaction_amount = transferRequest.Amount.ToString(),
                            transaction_beneficiary = transferRequest.CreditAccount1,
                            phone_number = _phone.StartsWith("234") ? _phone : string.Format("234{0}", _phone.Substring(1, _phone.Length - 1)),

                        };
                        isAuthorized = transferRequest.isMobile ? true : new ISECNotificationService().PushNotification(request, out error);

                        if (isAuthorized)
                        {
                            Logger.LogInfo("CBAService.PostTran", "Authorized");
                            result = PostTran(transferRequest, ref error);
                        }
                    }
                    else
                    {
                        error = "phone number not supplied";
                    }
                    if (isAuthorized == false)
                    {
                        ResponseCode = "06";
                        ResponseDescription = error;
                    }
                    break;
            }

            if (result)
            {
                ResponseCode = "00";
                ResponseDescription = "Successful";
            }
            else
            {
                ResponseCode = "06";
                ResponseDescription = error;
            }

            AuthResponse = Task<IntraBankTransactionResponse>.Factory.StartNew(() =>
            {
                return new IntraBankTransactionResponse
                {
                    ResponseCode = ResponseCode,
                    ResponseDescription = ResponseDescription,
                };
            });

            Logger.LogInfo("CBAService.DoBankTransfer, Response", authResponse);

            return AuthResponse;
        }

        internal UserProfileResponse AuthenticateSameBankTransfer(IntraBankTransactionRequest transferRequest)
        {
            Logger.LogInfo("CBAService.AuthenticateSameBankTransfer, Input", transferRequest);
            UserProfileResponse authResponse = null;

            //if customer PIN is null check if benefitiary accoutn belongs to the same customer
            if (string.IsNullOrEmpty(transferRequest.PIN) && transferRequest.TranType != TranType.Reversal && transferRequest.TranType != TranType.GLPosting)
            {
                AccountRequest customer = new AccountRequest()
                {
                    CustomerID = transferRequest.customer_id,
                };

                string jsonCustomer = Newtonsoft.Json.JsonConvert.SerializeObject(customer);
                List<AccountDetails> accounts = new AccountInquiryService().GetAccountsWithCustomerID(customer).Result.AccountInformation.ToList();

                //benefitiary account number check from list of customer accounts
                if (accounts.Where(x => x.AccountNumber == transferRequest.CreditAccount1) != null)
                {
                    Logger.LogInfo("CBAService.DoBankTransfer, Transaction to selfe ", "No Pin required");
                    authResponse = new TokenManager().NonTransactionalTokenAuthentication(Newtonsoft.Json.JsonConvert.SerializeObject(transferRequest));
                }
                else
                {
                    authResponse = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "AUTHENTICATION FAILED: benefitiary account does not belong to transfer initiator"
                    };
                }
            }
            else
            {
                ///for reversal transactions that dont have token or pin
                if (transferRequest.TranType == TranType.Reversal || transferRequest.TranType == TranType.GLPosting)
                {
                    Logger.LogInfo("CBAService.DoBankTransfer, Reversal/GLPosting ", "No Pin required");

                    authResponse = new UserProfileResponse()
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Reversal: No token/ Pin required"
                    };
                }
                else
                {
                    // Do token validation if Customer inputs PIN
                    authResponse = new TokenManager().TransactionalTokenAuthentication(Newtonsoft.Json.JsonConvert.SerializeObject(transferRequest));
                }
            }
            Logger.LogInfo("CBAService.AuthenticateSameBankTransfer, response", authResponse);
            return authResponse;
        }

        internal bool PostTran(IntraBankTransactionRequest tran, ref string error)
        {
            bool enable = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableFinancialTransaction"]);
            if (enable == false) return false;
            //  return false;

            string DeviceID = "1002";// required from istitution
            string AuthCode = "558755"; // static value Authorisation code give by the bank for respone
            decimal TransType = 1; //trsansaction type to determine if posting is within the same customer {0} or different customers {1}
            decimal reqLanInd = 1;
            decimal ChannelID = 1; // required from institution
            string ChannelRefNum = GetChannelRefNum(); //"1234599"; // "123456"; //required from institiution
            string SourceCode = "30";  //required from the institution
            decimal fromBal = 0;
            decimal toBal = 0;
            decimal traSeq = 0;
            string outReqId = "";
            decimal Returnstatus = 0;
            string procData = "";
            error = "";
            string cardNumber = "";
            decimal AppError = 0; // incdicates if error is application{1} or database {0}
            string SwitchDate = string.Format("{0:ddMMyyyy}", DateTime.Now); //"16022016";


            ProvidusPostingService.BanksMiddleware_PE07EDC00Client client = new ProvidusPostingService.BanksMiddleware_PE07EDC00Client();
            try
            {
                //string fromAccountQuery = string.Format(CommonUtility.Configuration.ConfigurationManager.ConvertAcctNumber.Replace(":ACCOUNT_NUMBER", "'{0}'"), tran.DebitAccount);
                //string toAccountQuery = string.Format(CommonUtility.Configuration.ConfigurationManager.ConvertAcctNumber.Replace(":ACCOUNT_NUMBER", "'{0}'"), tran.CreditAccount1);

                //string AcctFrom = new EntitySystem<object>(ProcessorKey).RetrieveBySpecificProperty(fromAccountQuery, "Account_Key");
                //string toAccount = new EntitySystem<object>(ProcessorKey).RetrieveBySpecificProperty(toAccountQuery, "Account_Key");

                string AcctFrom = CBA_Client.get21DigitAccountNo(tran.DebitAccount);
                string toAccount = CBA_Client.get21DigitAccountNo(tran.CreditAccount1);

                Logger.LogInfo("CBAService.PostTran, AcctFr", AcctFrom);
                Logger.LogInfo("CBAService.PostTran, toAccount", toAccount);
                Logger.LogInfo("CBAService.PostTran", "About to post to expense");

                string AcctTo = toAccount ?? tran.CreditAccount1;

                int tranID = Process.GetCurrentProcess().Id;
                Logger.LogInfo("CBAService.PostTran", "tranID =" + tranID);

                Logger.LogInfo("CBAService.PostTran", "channelRefNum =" + ChannelRefNum);
                //push everything to creditAccount first so customer will get one alert
                client.e07edc05(AcctFrom, AcctTo, tran.Amount, SourceCode, DeviceID, TransType, tran.Narration, tran.Narration, cardNumber, tran.STAN, SwitchDate, tranID.ToString(), AuthCode,
                           ref reqLanInd, ref ChannelID, ref ChannelRefNum, ref fromBal, ref toBal, ref traSeq, ref outReqId, ref Returnstatus, ref error, ref procData, ref AppError);

                Logger.LogInfo("CBAService.PostTran, Response from credit account :", procData);
                Logger.LogInfo("CBAService.PostTran, App error :", AppError);
                Logger.LogInfo("CBAService.PostTran, Returnstatus :", Returnstatus);
                Logger.LogInfo("CBAService.PostTran, procData :", procData);
                Logger.LogInfo("CBAService.PostTran, error :", error);

                //if income is to be taken, take from creditAccount
                if (tran.SplitAccount != null && tran.SplitAccount.Count() > 0 && Returnstatus == 0)
                {

                    foreach (var account in tran.SplitAccount.Keys)
                    {
                        Logger.LogInfo("CBAService.PostTran :", "About to post to split account" + account);
                        //string accountQuery = string.Format(CommonUtility.Configuration.ConfigurationManager.ConvertAcctNumber.Replace(":ACCOUNT_NUMBER", "'{0}'"), account);
                        //string _account = new EntitySystem<object>(ProcessorKey).RetrieveBySpecificProperty(accountQuery, "Account_Key");
                        string _account = CBA_Client.get21DigitAccountNo(account);
                        string destAccount = _account ?? account; //take the converted value if not null
                        decimal amount = tran.SplitAccount[account];
                        ChannelRefNum = GetChannelRefNum();

                        Logger.LogInfo("CBAService.PostTran, AcctFr", AcctFrom);
                        Logger.LogInfo("CBAService.PostTran, toAccount", toAccount);
                        Logger.LogInfo("CBAService.PostTran", "About to post to expense");

                        client.e07edc05(AcctFrom, destAccount, amount, SourceCode, DeviceID, TransType, tran.Remakcs, tran.Remakcs, cardNumber, tran.STAN, SwitchDate, tran.RRN, AuthCode,
                               ref reqLanInd, ref ChannelID, ref ChannelRefNum, ref fromBal, ref toBal, ref traSeq, ref outReqId, ref Returnstatus, ref error, ref procData, ref AppError);

                        Logger.LogInfo("CBAService.PostTran, Response from credit account :", procData);
                        Logger.LogInfo("CBAService.PostTran, App error :", AppError);
                        Logger.LogInfo("CBAService.PostTran, Returnstatus :", Returnstatus);
                        Logger.LogInfo("CBAService.PostTran, procData :", procData);
                        Logger.LogInfo("CBAService.PostTran, error :", error);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                error = ex.Message;
                return false;
            }
            finally
            {
                client.Close();
            }

            return Returnstatus == 0;
        }

        public Task<NIPBankResponse> NIPBanks()
        {
            NIPBankResponse nipResponse = null;

            try
            {
                NFPOutwardService.NFPOutwardService_BranchSoapClient client = new NFPOutwardService.NFPOutwardService_BranchSoapClient();

                var response = client.GetNFPBanks();

                List<NIPBanks> NIPBanks = new List<NIPBanks>();

                foreach (var item in response)
                {
                    string[] result = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (result.Count() != 2) continue;

                    NIPBanks.Add(
                    new NIPBanks
                    {
                        Name = result[0],
                        BankCode = result[1],
                    });
                }
                nipResponse = new NIPBankResponse
                {
                    TheBanks = NIPBanks,
                    ResponseCode = NIPBanks.Count > 0 ? "00" : "06",
                    ResponseDescription = NIPBanks.Count > 0 ? "Successful" : "System error has occurred"
                };
            }
            catch
            {
                if (isDemo)
                {
                    nipResponse = new NIPBankResponse
                    {
                        TheBanks = new List<NIPBanks> { new NIPBanks { BankCode = "011", Name = "FIRST BANK OF NIGERIA" }, new NIPBanks { BankCode = "033", Name = "UNITED BANK FOR AFRICA" } },
                        ResponseCode = "00",
                        ResponseDescription = "Successful"
                    };
                }
                else
                {
                    nipResponse = new NIPBankResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "System error has occurred"
                    };
                }
            }
            return Task<NIPBankResponse>.Factory.StartNew(() => nipResponse);
        }

        string GetChannelRefNum()
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["ChannelRefNum"];
            long lastId = 0;

            //Int64.TryParse(File.ReadAllText(path), out lastId);
            //lastId++;
            //File.WriteAllText(path, lastId.ToString());
            using (StreamReader reader = new StreamReader(path))
            {
                Int64.TryParse(reader.ReadLine(), out lastId);
                lastId++;
            }
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(lastId);
            }
            return lastId.ToString();
        }

        public LifestyleDebitResponse LifeStyleDebit(LifestyleDebitRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
