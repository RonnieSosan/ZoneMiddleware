using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.ProvidusImplementation.ProfileService;
using System.IO;
using Blend.ProvidusImplementation.NotificationService;
using System.Diagnostics;
using AppZoneMiddleware.Shared.Utility.EntitySystem;
using AppZoneMiddleware.Shared.Utility.Configuration;

namespace Blend.ProvidusImplementation.CoreBankingService
{
    public class AccountInquiryService : IAccountInquiry
    {
        ProvidusCBAMiddleware.BanksClient CBA_Client = new ProvidusCBAMiddleware.BanksClient();

        string ProcessorKey = "CBA";
        bool isDemo = false;
        string providusBankCode = System.Configuration.ConfigurationManager.AppSettings.Get("ProvidusBankCode");
        string oldBankCode = "999037";
        double MobileDailyLimit = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings.Get("MobileDailyLlimit"));
        public AccountInquiryService()
        {
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("Isdemo"), out isDemo);
        }

        public NFPOutwardService.AuthHeader authHeather = new NFPOutwardService.AuthHeader()
        {
            Password = System.Configuration.ConfigurationManager.AppSettings.Get("NIPPassword"),
            UserName = System.Configuration.ConfigurationManager.AppSettings.Get("NIPUserName")
        };

        public Task<BalanceResponse> BalanceEnquiry(AccountRequest accountRequest)
        {
            Logger.LogInfo("AccountValidationService.BalanceEnquiry, input :", accountRequest);
            BalanceResponse response = null;
            string bal = GetBalance(accountRequest.AccountNumber);

            response = new BalanceResponse
            {
                ResponseCode = "00",
                ResponseDescription = "SUCCESSFUL",
                Balance = bal,
                AccountNumber = accountRequest.AccountNumber
            };

            Logger.LogInfo("AccountValidationService.BalanceEnquiry, Account | balance", string.Format("{0} | {1}", accountRequest.AccountNumber, bal));
            return Task<BalanceResponse>.Factory.StartNew(() => response);
        }

        public Task<BVNInquiryResponse> DoBVNInquiry(BVNInquiryRequest bvnInquiryRequest)
        {
            Logger.LogInfo("AccountValidationService.DoBVNEnquiry, input", bvnInquiryRequest);
            BVNInquiryResponse response = null;
            string serviceResponse = CBA_Client.getAccountsWithBVN(bvnInquiryRequest.BVN);

            IList<BVNAccounts> BVNAccounts = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<BVNAccounts>>(serviceResponse);

            if (BVNAccounts != null && BVNAccounts.Count > 0)
            {
                response = new BVNInquiryResponse
                {
                    AccountList = BVNAccounts,
                    ResponseCode = "00",
                    ResponseDescription = "Successful",
                };
            }
            else
            {
                response = new BVNInquiryResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "Failed to retrieve account on BVN",
                };
            }

            Logger.LogInfo("AccountValidationService.DoBVNEnquiry, response", response);
            return Task<BVNInquiryResponse>.Factory.StartNew(() => response);

        }

        public Task<CustomerAccountsResponse> GetAccountsWithCustomerID(AccountRequest accountRequest)
        {
            Logger.LogInfo("CBAService.GetAccountsWithCustomerID, input :", accountRequest);

            Task<CustomerAccountsResponse> response = null;
            try
            {
                accountRequest.CustomerID = accountRequest.CustomerID == null ? accountRequest.customer_id : accountRequest.CustomerID;

                string serviceResponse = CBA_Client.getAccountsWithCusNum(accountRequest.CustomerID);
                Logger.LogInfo("CBAService.GetAccountsWithCustomerID, ResponseFromService ", serviceResponse);

                IList<AccountDetails> AccountInformation = Newtonsoft.Json.JsonConvert.DeserializeObject<IList<AccountDetails>>(serviceResponse);

                if (AccountInformation == null || AccountInformation.Count == 0 || serviceResponse == "INVALID ACCOUNT")
                {
                    response = Task<CustomerAccountsResponse>.Factory.StartNew(() =>
                    {
                        return new CustomerAccountsResponse
                        {
                            AccountInformation = null,
                            ResponseCode = "06",
                            ResponseDescription = "Account not found"
                        };
                    });

                }
                else
                {
                    foreach (var item in AccountInformation)
                    {
                        item.emailAddress = string.IsNullOrEmpty(item.emailAddress) ? item.emailAddress : item.emailAddress.Replace("\r\r\n", string.Empty);
                        item.PHONE = string.IsNullOrEmpty(item.PHONE) ? item.PHONE : item.PHONE.Replace("\r\r\n", string.Empty);
                    }

                    response = Task<CustomerAccountsResponse>.Factory.StartNew(() =>
                    {
                        return new CustomerAccountsResponse
                        {
                            AccountInformation = AccountInformation,
                            ResponseDescription = "Success",
                            ResponseCode = "00",
                        };
                    });

                    string extendTokenResponse = new TokenManager().ExtendToken(accountRequest.CustomerID);
                    Logger.LogInfo("CBAService.GetAccountsWithCustomerID, ExtendTokenResponse ", extendTokenResponse);

                    if (accountRequest.MailRequest != null && !(string.IsNullOrEmpty(accountRequest.Passkey)))
                    {
                        accountRequest.MailRequest.customer_id = accountRequest.CustomerID;
                        new MailService().SendMailToCustomer(accountRequest.MailRequest);
                    }
                }
            }
            catch (Exception ex)
            {
                response = Task<CustomerAccountsResponse>.Factory.StartNew(() =>
                {
                    return new CustomerAccountsResponse
                    {
                        ResponseDescription = ex.Message,
                        ResponseCode = "06",
                    };
                });
                Logger.LogError(ex);
            }
            Logger.LogInfo("CBAService.GetAccountsWithCustomerID, response :", response);
            return response;
        }

        public Task<NameInquiryResponse> SameBankNameInquiry(AccountRequest accountRequest)
        {
            string accountName = CBA_Client.getAccountName(accountRequest.AccountNumber);
            Logger.LogInfo("CBAService.SameBankEnquiry, client call, service response", accountName);
            NameInquiryResponse response = new NameInquiryResponse
            {
                ResponseCode = "00",
                ResponseDescription = "SUCCESSFUL",
                AccountName = accountName,
                AccountNumber = accountRequest.AccountNumber
            };
            return Task<NameInquiryResponse>.Factory.StartNew(() => response);
        }

        public Task<AccountResponse> ValidateAccountNumber(AccountRequest accountRequest)
        {
            Logger.LogInfo("CBAService.ValidateAccountNumber, input :", accountRequest);
            AccountResponse response = null;

            try
            {


                string input = Newtonsoft.Json.JsonConvert.SerializeObject(accountRequest);
                // Do token validation
                UserProfileResponse authResponse = new TokenManager().NonTransactionalTokenAuthentication(input);

                if (authResponse.ResponseCode == "06")
                {
                    response = new AccountResponse()
                    {
                        ResponseCode = authResponse.ResponseCode,
                        ResponseDescription = authResponse.ResponseDescription,
                    };

                    Logger.LogInfo("CBAService.ValidateAccountNumber, failedResponse ", response);
                    return Task<AccountResponse>.Factory.StartNew(() => response);
                }
                // token valid

                string ServiceResposne = CBA_Client.getAccountWithAccountNo(accountRequest.AccountNumber);
                Logger.LogInfo("CBAService.ValidateAccountNumber, ResponsefromService ", ServiceResposne);

                AccountDetails AccountInformation = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountDetails>(ServiceResposne);

                if (AccountInformation == null || ServiceResposne == "INVALID ACCOUNT")
                {
                    response = new AccountResponse()
                    {
                        AccountInformation = null,
                        ResponseCode = "06",
                        ResponseDescription = "Account not found"
                    };
                }
                else
                {
                    AccountInformation.FullName = AccountInformation.AccountName;

                    try
                    {
                        decimal custid = 0;
                        decimal.TryParse(AccountInformation.CustomerID, out custid);
                        AccountInformation.ACCOUNTBALANCE = GetBalance(accountRequest.AccountNumber);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                        response = new AccountResponse() //if it fails ehile getting balance, still return ok
                        {

                            AccountInformation = AccountInformation,
                            ResponseDescription = "Success",
                            ResponseCode = "00",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                //if it fails ehile getting balance, still return ok
                response = new AccountResponse()
                {
                    ResponseDescription = "Failed: " + ex.Message,
                    ResponseCode = "00",
                };
            }

            return Task<AccountResponse>.Factory.StartNew(() => response);
        }

        private string GetBalance(string AccountNumber)
        {
            string accountBal = "";
            try
            {
                accountBal = CBA_Client.getAccountBalance(AccountNumber);
                Logger.LogInfo("CBAService.GetBalance, ResponseFromService :", accountBal);

            }
            catch (Exception ex)
            {
                Logger.LogInfo("CBAService.GetBalance, ex :", ex.ToString());
            }
            return accountBal;
        }

        public Task<NameInquiryResponse> InterBankNameInquiry(NameInquiryRequest nameEnquiryRequest)
        {
            Logger.LogInfo("NIPService.DoNIPNameInquiry, name enquiry:", nameEnquiryRequest);

            NameInquiryResponse response = new NameInquiryResponse
            {
                ResponseCode = "06",
                ResponseDescription = "No matching account found"
            };
            string accountName = "";
            try
            {
                AccountRequest getCustomerAcctNum = new AccountRequest()
                {
                    CustomerID = nameEnquiryRequest.customer_id,
                };

                Logger.LogInfo("NIPService.DoNIPNameInquiry, try to get Acount for:", getCustomerAcctNum);
                nameEnquiryRequest.myAccountNumber = (GetAccountsWithCustomerID(getCustomerAcctNum)).Result.AccountInformation.ToList().FirstOrDefault().AccountNumber;
                Logger.LogInfo("NIPService.DoNIPNameInquiry, ACCOUHNT NUMBER RETRIEVED:", nameEnquiryRequest.myAccountNumber);


                if (nameEnquiryRequest.myDestinationBankCode == providusBankCode || nameEnquiryRequest.myDestinationBankCode == oldBankCode)
                {
                    accountName = SameBankNameInquiry(new AccountRequest { AccountNumber = nameEnquiryRequest.DestinationAccountNumber }).Result.AccountName;

                    response = new NameInquiryResponse
                    {
                        AccountName = accountName,
                        AccountNumber = nameEnquiryRequest.myAccountNumber,
                        ResponseCode = string.IsNullOrEmpty(accountName) ? "06" : "00",
                        ResponseDescription = string.IsNullOrEmpty(accountName) ? "Account does not exist" : "Successful"
                    };
                }
                else
                {
                    nameEnquiryRequest.myChannelCode = nameEnquiryRequest.isMobile ? "03" : "02";
                    Logger.LogInfo("NIPService.NameEnq, ChannelCode ", nameEnquiryRequest.myChannelCode);
                    if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["NIPTest"]))
                    {

                        #region NewNFP Live Implementation
                        NFPOutwardService.NFPOutwardService_BranchSoapClient client = new NFPOutwardService.NFPOutwardService_BranchSoapClient();
                        Logger.LogInfo("NIPService.DoNIPNameInquiry, about to call endpoint ", client.Endpoint.Address);
                        accountName = client.RIB_NameEnquirySingleitem(authHeather, nameEnquiryRequest.myDestinationBankCode, nameEnquiryRequest.DestinationAccountNumber, nameEnquiryRequest.myChannelCode, nameEnquiryRequest.myAccountNumber);

                        #endregion
                    }
                    else
                    {
                        #region NewNFP Test Implementation
                        NipWrapperService.NipWrapperServiceSoapClient client = new NipWrapperService.NipWrapperServiceSoapClient();
                        Logger.LogInfo("NIPService.DoNIPNameInquiry, about to call endpoint ", client.Endpoint.Address);
                        accountName = client.nameenquirysingleitem(nameEnquiryRequest.myDestinationBankCode, nameEnquiryRequest.DestinationAccountNumber, nameEnquiryRequest.myChannelCode, nameEnquiryRequest.myAccountNumber);

                        #endregion

                    }

                    Logger.LogInfo("NIPService.DoNIPNameInquiry, Response From NIP ", accountName);

                    //run some checks
                    if (!string.IsNullOrEmpty(accountName))
                    {
                        string[] responses = accountName.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);


                        if (responses != null && responses[0] == "00" && responses.Count() > 1)
                        {
                            Logger.LogInfo("NIPService.DoNIPNameInquiry, responses.Count(): ", responses.Count());
                            response = new NameInquiryResponse
                            {
                                AccountName = string.IsNullOrEmpty(responses[1]) ? "--" : responses[1],
                                AccountNumber = nameEnquiryRequest.myAccountNumber,
                                ResponseCode = "00",
                                ResponseDescription = "Successfull"
                            };

                            Logger.LogInfo("NIPService.DoNIPNameInquiry, Response ", response);
                            return Task<NameInquiryResponse>.Factory.StartNew(() => response);
                        }

                        if (responses != null && responses[0] == "00" && responses.Count() == 1)
                        {
                            Logger.LogInfo("NIPService.DoNIPNameInquiry, responses.Count(): ", responses.Count());
                            response = new NameInquiryResponse
                            {
                                AccountName = "--",
                                AccountNumber = nameEnquiryRequest.myAccountNumber,
                                ResponseCode = "00",
                                ResponseDescription = "Successfull"
                            };
                        }
                        else
                        {
                            response = new NameInquiryResponse
                            {
                                ResponseCode = "06",
                                ResponseDescription = responses[0]
                            };
                        }
                    }
                    else
                    {
                        response = new NameInquiryResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "FAILED: no resposne from service"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                response = new NameInquiryResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "An Error occurred"
                };
            }

            Logger.LogInfo("NIPService.DoNIPNameInquiry, Response ", response);
            return Task<NameInquiryResponse>.Factory.StartNew(() => response); ;
        }

        public Task<NIPResponse> DoNIPBVNInquiry(NIPRequest BVNInquiry)
        {
            Logger.LogInfo("NIPService.DoNIPBVNInquiry, DoNIPBVNInquiry input =", BVNInquiry);

            NIPResponse nipResponse = new NIPResponse
            {
                ResponseCode = "06",
                ResponseDescription = "no record found"
            };

            try
            {
                if (BVNInquiry.BVN == "12345678901" && isDemo) // this is for test
                {
                    nipResponse = new NIPResponse
                    {
                        FirstName = "Adekunle",
                        LastName = "Chukwuma",
                        MiddleName = "Ciroma",
                        DateOfBirth = DateTime.Now.ToShortDateString(),
                        PhoneNumber = "08123456789",
                        BVN = BVNInquiry.BVN,
                        EmailAddress = "oraji@appzonegroup.com",
                        ResponseCode = "00",
                        ResponseDescription = "Successfull",
                    };
                }
                else if (!string.IsNullOrEmpty(BVNInquiry.BVN))
                {
                    NIPBVNService.ValidatorClient nipClient = new NIPBVNService.ValidatorClient();
                    string[] values = nipClient.getAll(BVNInquiry.BVN);

                    if (values != null && values.Count() > 0)
                    {
                        values.ToList().ForEach(i => Trace.TraceInformation(i.ToString()));

                        BVNInquiryRequest bvnRequest = new BVNInquiryRequest()
                        {
                            BVN = BVNInquiry.BVN
                        };


                        BVNInquiryResponse bvnInqResponse = DoBVNInquiry(bvnRequest).Result;

                        Logger.LogInfo("NIPService.DoNIPBVNInquiry, response from BVN enq", bvnInqResponse);

                        if (bvnInqResponse.ResponseCode == "00")
                        {
                            nipResponse = new NIPResponse
                            {
                                FirstName = values[0],
                                LastName = values[2],
                                MiddleName = values[1],
                                DateOfBirth = values[3],
                                PhoneNumber = values[4],
                                BVN = BVNInquiry.BVN,

                                ResponseCode = "01",
                                ResponseDescription = "Successfull",
                            };
                        }
                        else
                        {
                            nipResponse = new NIPResponse
                            {
                                FirstName = values[0],
                                LastName = values[2],
                                MiddleName = values[1],
                                DateOfBirth = values[3],
                                PhoneNumber = values[4],
                                BVN = BVNInquiry.BVN,
                                ResponseCode = "00",
                                ResponseDescription = "Successfull",
                            };
                        }
                    }

                    Logger.LogInfo("NIPService.DoNIPBVNInquiry, FirstName =", nipClient.getFirstName(BVNInquiry.BVN));
                    nipClient.Close();
                }
                else
                {
                    new MailService().SendMail(BVNInquiry.MailRequest);
                }
                if (BVNInquiry.MailRequest != null)
                {
                    new MailService().SendMail(BVNInquiry.MailRequest);
                }
            }
            catch (Exception ex)
            {
                nipResponse = new NIPResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = ex.Message
                };
                Logger.LogError(ex);
            }

            Logger.LogInfo("NIPService.DoNIPBVNInquiry, response", nipResponse);
            return Task<NIPResponse>.Factory.StartNew(() => nipResponse);
        }

        public Task<AccountStatementResponse> GetMiniStatement(AccountStatementRequest statementRequest)
        {
            Logger.LogInfo("AccountStatementService.GetminiStatement, Input", statementRequest);
            AccountStatementResponse response = null;

            try
            {
                // Do token validation
                UserProfileResponse authResponse = (new TokenManager().NonTransactionalTokenAuthentication(Newtonsoft.Json.JsonConvert.SerializeObject(statementRequest)));

                if (authResponse.ResponseCode == "06")
                {
                    response = new AccountStatementResponse()
                    {
                        ResponseCode = authResponse.ResponseCode,
                        ResponseDescription = authResponse.ResponseDescription,
                    };

                    Logger.LogInfo("NIPService.DoFundTransfer, failedResponse ", response);

                    return Task<AccountStatementResponse>.Factory.StartNew(() => response);
                }
                // token valid
                string queryString = ConfigurationManager.GetAccountStatement
                    .Replace(":NUBAN_NO", string.Format("{0}", statementRequest.AccountNumber))
                    .Replace(":Start_date_ddmmyyyy", string.Format("{0:ddMMyyyy}", statementRequest.DateFrom))
                    .Replace(":End_date_ddmmyyyy", string.Format("{0:ddMMyyyy}", statementRequest.DateTo));

                Logger.LogInfo("GetminiStatement", queryString);

                IList<AccountStatementDetails> statement = new EntitySystem<AccountStatementDetails>(ProcessorKey).GetEntityList(queryString);

                if (statement != null && statement.Count > 0)
                {
                    foreach (var entry in statement)
                    {
                        entry.TransactionDate = Convert.ToDateTime(entry.TransactionDate).ToString("MM/dd/yyyy");
                        entry.ValueDate = Convert.ToDateTime(entry.ValueDate).ToString("MM/dd/yyyy");
                        string temp = entry.Remarks.Replace("<", "[");
                        string finalTemp = temp.Replace(">", "]");
                        string QuoteRemoveTemp = entry.Remarks.Replace("'", "");
                        string removeFinal = temp.Replace("'", "");
                        entry.Remarks = finalTemp;
                    }
                    response = new AccountStatementResponse
                    {
                        AccountStatements = statement,
                        ResponseCode = "00",
                        ResponseDescription = "Successful"
                    };
                }
                else
                {
                    response = new AccountStatementResponse
                    {
                        AccountStatements = null,
                        ResponseCode = "06",
                        ResponseDescription = "No result found"
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new AccountStatementResponse
                {
                    AccountStatements = null,
                    ResponseCode = "06",
                    ResponseDescription = ex.Message
                };
            }
            Logger.LogInfo("AccountStatementService.GetminiStatement, Response", response);
            return Task<AccountStatementResponse>.Factory.StartNew(() => response);
        }

        public Task<CreditCheckResponse> CreditBureauCheck(CreditCheckRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExistingAccountHolderResponse> FetchExistingAccountHolderData(ExistingAccountHolderRequest accountRequest)
        {
            throw new NotImplementedException();
        }

        //public Task<OpenAccountResponse> OpenAccount(OpenAccountRequest openAccountRequest)
        //{
        //    Logger.LogInfo("OpenAccountService.OpenAccount, input", openAccountRequest);

        //    OpenAccountResponse openAccountResponse = new OpenAccountResponse();
        //    try
        //    {
        //        AddAccountService.AccountClient addAccountServiceClient = new AddAccountService.AccountClient();
        //        string jsonfordatgy = Newtonsoft.Json.JsonConvert.SerializeObject(openAccountRequest);

        //        Logger.LogInfo("OpenAccountService.OpenAccount, abou to call service: ", addAccountServiceClient.Endpoint.Address.ToString());
        //        string responseMessage = addAccountServiceClient.addAccount(openAccountRequest.ref_num, openAccountRequest.bvn, openAccountRequest.mobile_no, openAccountRequest.email, openAccountRequest.bra_code, openAccountRequest.account_type, openAccountRequest.surname, openAccountRequest.first_name,
        //            openAccountRequest.account_name, openAccountRequest.address_line1, openAccountRequest.address_line2, openAccountRequest.id_image_url, openAccountRequest.photo_image_url, openAccountRequest.date_of_birth, openAccountRequest.gender);

        //        Logger.LogInfo("OpenAccountService.OpenAccount, response from service: ", responseMessage);

        //        if (responseMessage == "00")
        //        {
        //            if (openAccountRequest.MailRequest != null)
        //            {

        //                new MailService().SendMail(openAccountRequest.MailRequest);
        //            }
        //            openAccountResponse.ResponseCode = "00";
        //            openAccountResponse.ResponseDescription = "SUCCESSFUL";
        //        }

        //        else if (responseMessage == "01")
        //        {
        //            openAccountResponse.ResponseCode = "01";
        //            openAccountResponse.ResponseDescription = "REFERENCE NUMBER EXISTS";
        //        }

        //        else if (responseMessage == "02")
        //        {
        //            openAccountResponse.ResponseCode = "02";
        //            openAccountResponse.ResponseDescription = "INVALID BRANCH CODE";
        //        }

        //        else if (responseMessage == "03")
        //        {
        //            openAccountResponse.ResponseCode = "03";
        //            openAccountResponse.ResponseDescription = "UNKNOWN ERROR";
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        openAccountResponse = new OpenAccountResponse()
        //        {
        //            ResponseCode = "06",
        //            ResponseDescription = "FAILED: " + ex.Message,
        //        };
        //    }

        //    Logger.LogInfo("OpenAccountService.OpenAccount, response: ", openAccountResponse);

        //    return Task<OpenAccountResponse>.Factory.StartNew(() => openAccountResponse);
        //}
    }
}
