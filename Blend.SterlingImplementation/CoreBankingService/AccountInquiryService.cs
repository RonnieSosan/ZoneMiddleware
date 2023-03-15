using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using Blend.SterlingImplementation.Entites;
using System.Xml.Serialization;
using System.IO;
using AppZoneMiddleware.Shared.Utility;
using Blend.SterlingImplementation.ServiceUtilities;

namespace Blend.SterlingImplementation.CoreBankingService
{
    public class AccountInquiryService : IAccountInquiry
    {
        int webServiceAppId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebServiceAppId"]);
        bool demomode;
        bool NIPDemo;
        bool AccountValiationDemo;
        public AccountInquiryService()
        {
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("BVNDemoMode"), out demomode);
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("NIPDemoMode"), out NIPDemo);
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("AccountValiationDemo"), out AccountValiationDemo);
        }

        public Task<BalanceResponse> BalanceEnquiry(AccountRequest accountRequest)
        {
            Logger.LogInfo("AccountInquiryService.BalanceEnquiry, input", accountRequest);
            String balance = string.Empty;
            BalanceResponse response = null;
            try
            {
                XMLBalanceEnqResponse balanceEnqResponse = new XMLBalanceEnqResponse();
                XMLBalanceEnqRequest clientRequest = new XMLBalanceEnqRequest
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "201",
                    Account = accountRequest.AccountNumber
                };

                balanceEnqResponse = new ServiceUtilities.IBSBridgeProcessor<XMLBalanceEnqRequest, XMLBalanceEnqResponse>().Processor(clientRequest, true) as XMLBalanceEnqResponse;

                if (balanceEnqResponse.ResponseCode == "00")
                {
                    response = new BalanceResponse
                    {
                        ResponseCode = balanceEnqResponse.ResponseCode,
                        ResponseDescription = balanceEnqResponse.ResponseText,
                        Balance = balanceEnqResponse.Available,
                        AccountNumber = accountRequest.AccountNumber
                    };
                }
                else
                {
                    response = new BalanceResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Unable to process request",
                        Balance = "",
                        AccountNumber = accountRequest.AccountNumber
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new BalanceResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = ex.Message,
                    Balance = "",
                    AccountNumber = accountRequest.AccountNumber
                };
            }

            return Task.Factory.StartNew(() => response);
        }

        public Task<BVNInquiryResponse> DoBVNInquiry(BVNInquiryRequest bvnInquiryRequest)
        {
            throw new NotImplementedException();
        }

        public Task<NIPResponse> DoNIPBVNInquiry(NIPRequest BVNInquiry)
        {
            Logger.LogInfo("AccountInquiryService.DoBVNInquiry, input", BVNInquiry);
            NIPResponse response = new NIPResponse();
            if (demomode)
            {
                response = new NIPResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "SUCCESSFUL",
                    BVN = BVNInquiry.BVN,
                    FirstName = "BABAJIDE",
                    MiddleName = "RONALD",
                    LastName = "SOSAN",
                    PhoneNumber = BVNInquiry.BVN,
                    DateOfBirth = "20-Mar-95",
                };

                Logger.LogInfo("AccountInquiryService.DoBVNInquiry", response);
                return Task.Factory.StartNew(() => response);
            }
            try
            {
                BVNValidationResponseXML bvnResponse = new BVNValidationResponseXML();
                BVNValidationRequestXML bvnValidationRequest = new BVNValidationRequestXML()
                {
                    RequestType = "905",
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    Bvn = BVNInquiry.BVN
                };

                bvnResponse = new ServiceUtilities.IBSBridgeProcessor<BVNValidationRequestXML, BVNValidationResponseXML>().Processor(bvnValidationRequest, true) as BVNValidationResponseXML;

                if (bvnResponse.ResponseCode == "00")
                {
                    response = new NIPResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL",
                        BVN = bvnResponse.Bvn,
                        FirstName = bvnResponse.FirstName,
                        MiddleName = bvnResponse.MiddleName,
                        LastName = bvnResponse.LastName,
                        PhoneNumber = bvnResponse.PhoneNumber,
                        DateOfBirth = bvnResponse.DateOfBirth,
                    };
                }
                else
                {
                    response = new NIPResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Could not validate BVN"
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new NIPResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "An error occoured while processing request"
                };
            }
            Logger.LogInfo("AccountInquiryService.DoBVNInquiry", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<CustomerAccountsResponse> GetAccountsWithCustomerID(AccountRequest accountRequest)
        {
            Logger.LogInfo("AccountInquiryService.GetAccountsWithCustomerID, input", accountRequest);
            CustomerAccountRequest customerRequest = new CustomerAccountRequest();
            CustomerAccountResponse custResponse = new CustomerAccountResponse();
            CustomerAccountsResponse customerAccountResponse;
            try
            {
                if (AccountValiationDemo)
                {
                    var account = new AccountDetails
                    {
                        AccountName = "ONAWUNMI WALE BOSALINO",
                        CustomerID = "13782505",
                        AccountNumber = "0066032525",
                        ACCOUNTBALANCE = "0.00",
                        AccountProductCode = "6009",
                        SEX = "Male",
                        TITLE = "Otunba",
                        AccountStatus = "AUTH-FWD",
                        AccountType = "SAVINGS",
                        emailAddress = "WONAWUNMI@APPZONEGROUP.COM",
                        PHONE = "08034730365",
                        LedgerCode = "6009"
                    };

                    customerAccountResponse = new CustomerAccountsResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL"
                    };

                    List<AccountDetails> accountDetails = new List<AccountDetails>();
                    accountDetails.Add(account);
                    customerAccountResponse.AccountInformation = accountDetails;
                    Logger.LogInfo("AccountInquiryService.GetAccountsWithCustomerID", customerAccountResponse);
                    return Task.Factory.StartNew(() => customerAccountResponse);
                }


                customerRequest.RequestType = "907";
                customerRequest.ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));
                customerRequest.customerID = accountRequest.CustomerID;

                custResponse = new ServiceUtilities.IBSBridgeProcessor<CustomerAccountRequest, CustomerAccountResponse>().Processor(customerRequest, true) as CustomerAccountResponse;

                #region Construct Response Object

                if (custResponse.ResponseCode == "00")
                {
                    customerAccountResponse = new CustomerAccountsResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL"
                    };
                    List<AccountDetails> details = new List<AccountDetails>();
                    foreach (var item in custResponse.Records)
                    {
                        var res = ValidateAccountNumber(new AccountRequest { AccountNumber = item.NUBAN });
                        AccountResponse respAccount = res.Result;
                        respAccount.AccountInformation.LedgerCode = item.LEDCODE;
                        details.Add(respAccount.AccountInformation);
                    }
                    customerAccountResponse.AccountInformation = details;

                }
                else
                {
                    customerAccountResponse = new CustomerAccountsResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "FAILED"
                    };
                }
                #endregion

                Logger.LogInfo("AccountInquiryService.GetAccountsWithCustomerID", customerAccountResponse);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                customerAccountResponse = new CustomerAccountsResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "An error occoured while processing request"
                };
            }

            return Task.Factory.StartNew(() => customerAccountResponse);
        }

        public Task<AccountStatementResponse> GetMiniStatement(AccountStatementRequest statementRequest)
        {
            throw new NotImplementedException();
        }

        public Task<NameInquiryResponse> InterBankNameInquiry(NameInquiryRequest nameEnquiryRequest)
        {
            Logger.LogInfo("AccountInquiryService.InterBankNameInquiry, input", nameEnquiryRequest);
            XMLInterBankNameEnqResponse nameEnqResponse = new XMLInterBankNameEnqResponse();
            NameInquiryResponse response;

            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(nameEnquiryRequest, out br);
                if (!tokenValid)
                {
                    response = new NameInquiryResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<NameInquiryResponse>.Factory.StartNew(() => response);
                }

                if (NIPDemo)
                {
                    response = new NameInquiryResponse
                    {
                        AccountName = "SOULE OLUWATONI AYOKUNLE",
                        AccountNumber = nameEnquiryRequest.DestinationAccountNumber,
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL"
                    };
                    return Task.Factory.StartNew(() => response);
                }

                XMLinterBankNameEnqRequest nameEnqRequest = new XMLinterBankNameEnqRequest()
                {
                    RequestType = "105",
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    DestinationBankCode = nameEnquiryRequest.myDestinationBankCode,
                    ToAccount = nameEnquiryRequest.DestinationAccountNumber,
                };

                nameEnqResponse = new ServiceUtilities.IBSBridgeProcessor<XMLinterBankNameEnqRequest, XMLInterBankNameEnqResponse>().Processor(nameEnqRequest, true) as XMLInterBankNameEnqResponse;

                if (nameEnqResponse.ResponseCode == "00")
                {
                    response = new NameInquiryResponse
                    {
                        AccountName = nameEnqResponse.ResponseText,
                        AccountNumber = nameEnquiryRequest.DestinationAccountNumber,
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL",
                        SessionID = nameEnqResponse.SessionID,
                    };
                }
                else
                {
                    response = new NameInquiryResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "FAILED"
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new NameInquiryResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "An error occoured while processing request"
                };
            }

            Logger.LogInfo("AccountInquiryService.InterBankNameInquiry response ", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<NameInquiryResponse> SameBankNameInquiry(AccountRequest accountRequest)
        {
            Logger.LogInfo("AccountInquiryService.SameBankNameInquiry, input", accountRequest);
            NameInquiryResponse response = null;

            try
            {
                if (string.IsNullOrWhiteSpace(accountRequest.AccountNumber) || accountRequest.AccountNumber.Contains("+") || (accountRequest.AccountNumber.Trim().Length != 10 && accountRequest.AccountNumber.Trim().Length != 13))
                {
                    response = new NameInquiryResponse
                    {
                        ResponseCode = "MW01",
                        ResponseDescription = "The 'Debit' Account must be a NUBAN account number or a phone number of the form: 2348012345678. NB: There is should be no '+' sign.",
                    };
                    return Task<NameInquiryResponse>.Factory.StartNew(() => response);
                }

                accountRequest.AccountNumber = accountRequest.AccountNumber.Trim();
                if (accountRequest.AccountNumber.Length == 10)
                {
                    Logger.LogInfo("AccountInquiryService.SameBankNameInquiry", "Name Inquiry for NUBAN");
                    AccountEnquiryRequest acctRequest = new AccountEnquiryRequest();
                    AccountEnquiryResponse accountResponse = new AccountEnquiryResponse();
                    acctRequest.FromAccount = accountRequest.AccountNumber;
                    acctRequest.ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));
                    acctRequest.RequestType = "319";

                    accountResponse = new ServiceUtilities.IBSBridgeProcessor<AccountEnquiryRequest, AccountEnquiryResponse>().Processor(acctRequest, true) as AccountEnquiryResponse;

                    if (accountResponse.ResponseCode == "00")
                    {
                        response = new NameInquiryResponse
                        {
                            ResponseCode = accountResponse.ResponseCode,
                            ResponseDescription = accountResponse.ResponseText,
                            AccountName = accountResponse.CustomerName,
                            AccountNumber = accountRequest.AccountNumber
                        };
                    }
                    else
                    {
                        response = new NameInquiryResponse
                        {
                            ResponseCode = accountResponse.ResponseCode,
                            ResponseDescription = accountResponse.ResponseText,
                            //AccountName = accountResponse.CustomerName,   // Commented out since it's not successful
                            //AccountNumber = accountRequest.AccountNumber
                        };
                    }
                }
                else
                {
                    Logger.LogInfo("AccountInquiryService.SameBankNameInquiry", "Name Inquiry for Wallet");
                    GetWalletDetails wdReq = new GetWalletDetails()
                    {
                        Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                        RequestType = "108",
                        nuban = accountRequest.AccountNumber,
                    };

                    WalletDetails wdResp = new WalletAccountService().GetWalletDetails(wdReq).Result;

                    if (wdResp.ResponseCode == "00" && wdResp.data != null)
                    {
                        response = new NameInquiryResponse
                        {
                            ResponseCode = wdResp.ResponseCode,
                            ResponseDescription = wdResp.ResponseDescription,
                            AccountName = wdResp.data.AccountName,
                            AccountNumber = wdResp.data.AccountNumber,
                        };
                    }
                    else
                    {
                        response = new NameInquiryResponse
                        {
                            ResponseCode = (wdResp.ResponseCode == "00" ? "EP06" : wdResp.ResponseCode),
                            ResponseDescription = wdResp.ResponseDescription,
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
                    ResponseDescription = " Unable to process request",
                    AccountName = "",
                    AccountNumber = accountRequest.AccountNumber
                };

            }
            Logger.LogInfo("AccountInquiryService.SameBankNameInquiry response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<AccountResponse> ValidateAccountNumber(AccountRequest accountRequest)
        {
            Logger.LogInfo("AccountInquiryService.ValidateAccountNumber, input", accountRequest);
            AccountResponse response = new AccountResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(accountRequest.AccountNumber) || accountRequest.AccountNumber.Contains("+") || (accountRequest.AccountNumber.Trim().Length != 10 && accountRequest.AccountNumber.Trim().Length != 13))
                {
                    response = new AccountResponse
                    {
                        ResponseCode = "MW01",
                        ResponseDescription = $"The Account [{accountRequest.AccountNumber}] must be a NUBAN account number or a phone number of the form: 2348012345678. NB: There is should be no '+' sign.",
                    };
                    return Task<AccountResponse>.Factory.StartNew(() => response);
                }

                accountRequest.AccountNumber = accountRequest.AccountNumber.Trim();
                if (accountRequest.AccountNumber.Length == 10)
                {
                    Logger.LogInfo("AccountInquiryService.ValidateAccountNumber", "Name Inquiry for NUBAN");
                    AccountEnquiryRequest acctRequest = new AccountEnquiryRequest();
                    AccountEnquiryResponse accountResponse = new AccountEnquiryResponse();
                    acctRequest.FromAccount = accountRequest.AccountNumber;
                    acctRequest.ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));
                    acctRequest.RequestType = "319";

                    accountResponse = new ServiceUtilities.IBSBridgeProcessor<AccountEnquiryRequest, AccountEnquiryResponse>().Processor(acctRequest, true) as AccountEnquiryResponse;

                    if (accountResponse.ResponseCode == "00")
                    {
                        response = new AccountResponse
                        {
                            ResponseCode = "00",
                            ResponseDescription = "SUCCESSFUL",
                            AccountInformation = new AccountDetails
                            {
                                AccountNumber = accountRequest.AccountNumber,
                                AccountName = accountResponse.CustomerName,
                                FullName = accountResponse.CustomerName,
                                ACCOUNTBALANCE = accountResponse.AccountBalance,
                                AccountProductCode = accountResponse.ProductCode,
                                AccountCurrency = accountResponse.Currency,
                                AccountStatus = accountResponse.Status,
                                CustomerID = accountResponse.CustomerID,
                                emailAddress = accountResponse.Email,
                                PHONE = accountResponse.Phone,
                                AccountType = accountResponse.AccountGroup
                            }
                        };
                    }
                    else
                    {
                        response = new AccountResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "Could not validate account"
                        };
                    }
                }
                else
                {
                    Logger.LogInfo("AccountInquiryService.ValidateAccountNumber", "Name Inquiry for Wallet");
                    GetWalletDetails wdReq = new GetWalletDetails()
                    {
                        Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                        RequestType = "108",
                        nuban = accountRequest.AccountNumber,
                    };

                    WalletDetails wdResp = new WalletAccountService().GetWalletDetails(wdReq).Result;

                    if (wdResp.ResponseCode == "00" && wdResp.data != null)
                    {
                        response = new AccountResponse
                        {
                            ResponseCode = "00",
                            ResponseDescription = "SUCCESSFUL",
                            AccountInformation = new AccountDetails
                            {
                                AccountNumber = wdResp.data.AccountNumber,
                                AccountName = wdResp.data.AccountName,
                                FullName = wdResp.data.fullname,
                                ACCOUNTBALANCE = wdResp.data.AccountBalance,
                                AccountProductCode = wdResp.data.categorycode,
                                AccountCurrency = wdResp.data.currencycode,
                                AccountStatus = wdResp.data.AccountStatus,
                                CustomerID = wdResp.data.customerid,
                                emailAddress = wdResp.data.email,
                                PHONE = wdResp.data.phone,
                                AccountType = wdResp.data.categorycode
                            }
                        };
                    }
                    else
                    {
                        response = new AccountResponse
                        {
                            ResponseCode = (wdResp.ResponseCode == "00" ? "EP06" : wdResp.ResponseCode),
                            //ResponseDescription = "Could not wallet number"
                            ResponseDescription = wdResp.ResponseDescription,
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new AccountResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "An error occoured while processing request"
                };
            }

            Logger.LogInfo("AccountInquiryService.ValidateAccountNumber, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<CreditCheckResponse> CreditBureauCheck(CreditCheckRequest request)
        {
            Logger.LogInfo("AccountServices.CreditBureauCheck, input", request);

            CreditCkeckRequestXML ibsRequest = null;
            CreditCheckResponse response = null;
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(request, out br);
                if (!tokenValid)
                {
                    response = new CreditCheckResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<CreditCheckResponse>.Factory.StartNew(() => response);
                }

                CustomerAccountsResponse accountInqResponse = new AccountInquiryService().GetAccountsWithCustomerID(new AccountRequest { CustomerID = request.CustomerNumber }).Result;
                if (accountInqResponse.ResponseCode != "00")
                {
                    response = new CreditCheckResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Unable to retrieve account"
                    };
                    return Task.Factory.StartNew(() => response);
                }
                string AccountNumber = accountInqResponse.AccountInformation.FirstOrDefault().AccountNumber;

                ibsRequest = new CreditCkeckRequestXML()
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "934",
                    nuban = AccountNumber,
                    BVN = request.BVN,
                    dob = request.dob,
                    gender = request.gender,
                    mobile = request.customer_id
                };

                SterlingBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<CreditCkeckRequestXML, SterlingBaseResponse>().Processor(ibsRequest, true) as SterlingBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new CreditCheckResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
                else
                {
                    response = new CreditCheckResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new CreditCheckResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Unable to update bvn"
                };
            }
            Logger.LogInfo("AccountServices.CreditBureauCheck, response", request);
            return Task.Factory.StartNew(() => response);
        }

        public Task<ExistingAccountHolderResponse> FetchExistingAccountHolderData(ExistingAccountHolderRequest accountRequest)
        {
            Logger.LogInfo("AccountInquiryService.FetchExistingAccountHolderData, input", accountRequest);
            ExistingAccountHolderResponse response = null;

            if(string.IsNullOrWhiteSpace(accountRequest.BVN) && string.IsNullOrWhiteSpace(accountRequest.PhoneNumber))
            {
                response = new ExistingAccountHolderResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Customer's BVN and/or Phone Number is required.",
                };

                return Task.Factory.StartNew(() => response);
            }

            try
            {
                ExistingAccountHolderRequestXML clientRequest = new ExistingAccountHolderRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "1003",
                    BVN = (string.IsNullOrWhiteSpace(accountRequest.BVN) ? string.Empty : accountRequest.BVN.Trim()),
                    PhoneNo = (string.IsNullOrWhiteSpace(accountRequest.PhoneNumber) ? string.Empty : accountRequest.PhoneNumber.Trim()),
                };
                ExistingAccountHolderResponseXML respDataXML = new ExistingAccountHolderResponseXML();
                respDataXML = new ServiceUtilities.IBSBridgeProcessor<ExistingAccountHolderRequestXML, ExistingAccountHolderResponseXML>().Processor(clientRequest, true) as ExistingAccountHolderResponseXML;

                if (respDataXML.ResponseCode == "00")
                {
                    response = new ExistingAccountHolderResponse
                    {
                        ResponseCode = respDataXML.ResponseCode,
                        ResponseDescription = respDataXML.ResponseText,                        
                    };

                    if (respDataXML.TheFDlist != null && respDataXML.TheFDlist.Count() > 0)
                    {
                        List<ExistingSterlingAccounts> TheAccountsList = new List<ExistingSterlingAccounts>();
                        foreach (var item in respDataXML.TheFDlist)
                        {
                            TheAccountsList.Add(new ExistingSterlingAccounts()
                            {
                                BVN = Utils.TrimOutUnknownCharacters(item.BVN),
                                CustomerID = Utils.TrimOutUnknownCharacters(item.customerid),
                                DateOfBirth = item.Dateofbirth,
                                EMail = Utils.TrimOutUnknownCharacters(item.Email),
                                FirstName = Utils.TrimOutUnknownCharacters(item.FirstName),
                                Gender = Utils.TrimOutUnknownCharacters(item.Gender),
                                IsIDAvailable = item.IsIDAvailable,
                                IsPassportPhotoAvailable = item.IsPassportPhotoAvailable,
                                IsReferenceAvailable = item.IsReferenceAvailable,
                                IsSignatureAvailable = item.IsSignatureAvailable,
                                IsUtilityBillAvailable = item.IsUtilityBillAvailable,
                                LastName = Utils.TrimOutUnknownCharacters(item.LastName),
                                MiddleName = Utils.TrimOutUnknownCharacters(item.MiddleName),
                                NUBAN = Utils.TrimOutUnknownCharacters(item.nuban),
                                PhoneNumber = item.Phone,
                                ResidentialAddress = Utils.TrimOutUnknownCharacters(item.ResidentialAddress),
                            });
                        }
                        response.TheAccountsList = TheAccountsList.ToArray();
                    }
                    else
                    {
                        response.ResponseCode = "06";
                        response.ResponseDescription = "No matching account data was found.";
                    }
                }
                else
                {
                    if (respDataXML.ResponseCode == "92x" && respDataXML.ResponseText.ToLowerInvariant().Contains("No account".ToLowerInvariant()))
                    {
                        response = new ExistingAccountHolderResponse
                        {
                            ResponseCode = "00",
                            ResponseDescription = respDataXML.ResponseText,
                            TheAccountsList = new ExistingSterlingAccounts[] { },
                        };
                    }
                    else
                    {
                        response = new ExistingAccountHolderResponse
                        {
                            ResponseCode = "EP06",
                            ResponseDescription = "Unable to process request.",
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new ExistingAccountHolderResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = ex.Message,
                };
            }

            Logger.LogInfo("AccountInquiryService.FetchExistingAccountHolderData, result", response);
            return Task.Factory.StartNew(() => response);
        }
    }
}