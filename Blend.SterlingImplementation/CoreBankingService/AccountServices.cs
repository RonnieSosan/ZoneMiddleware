using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.SterlingImplementation.Entites;
using System.Xml.Serialization;
using System.IO;
using Blend.SterlingImplementation.ServiceUtilities;

namespace Blend.SterlingImplementation.CoreBankingService
{
    public class AccountServices : IAccountServices
    {
        int webServiceAppId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebServiceAppId"]);
        public string spayUrl = System.Configuration.ConfigurationManager.AppSettings.Get("SterlingSpayURL");
        public bool openAccountDemo;

        public AccountServices()
        {
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("OpenAccountDemo"), out openAccountDemo);
        }
        public Task<OpenAccountResponse> OpenAccount(OpenAccountRequest openAccount)
        {
            Logger.LogInfo("OpenAccountService.OpenAccount, input", openAccount);
            AccountOpeningResponseXML xmlResponseObj = new AccountOpeningResponseXML();
            if (openAccountDemo)
            {
                return Task.Factory.StartNew(() => new OpenAccountResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "SUCCESS",
                    AccountNo = "0030440723",
                    CustomerID = "908389"
                });
            }

            string accProductCode = string.Empty;
            switch (openAccount.DesiredAccountClass)
            {
                case OpenAccountClass.Tier1Savings:
                    accProductCode = string.Empty;
                    break;
                case OpenAccountClass.Tier2Savings:
                    accProductCode = "CLASSIC.SAV"; // Classic savings account
                    break;
                case OpenAccountClass.Current:
                    accProductCode = "CLASSIC.ACCT";    // For opening current account, use 'productCode' value of 'CLASSIC.ACCT'
                    break;
                default:
                    accProductCode = string.Empty;
                    break;
            }

            if(string.IsNullOrWhiteSpace(openAccount.DesiredCurrencyCode))
            {
                openAccount.DesiredCurrencyCode = "NGN";
            }

            AccountOpeningRequestXML clientRequest = new AccountOpeningRequestXML()
            {
                ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                RequestType = "915",
                Title = openAccount.Title,
                mobile = openAccount.PhoneNumber,
                DateOfBirth = openAccount.date_of_birth,  //mm-dd-yyyy
                FirstName = openAccount.first_name,
                LastName = openAccount.last_name,
                MiddleName = openAccount.middle_name,
                Gender = openAccount.gender,
                email = openAccount.email,
                AddressHome = openAccount.address_line1,
                bvn = openAccount.bvn != null ? openAccount.bvn : string.Empty,
                productCode = accProductCode,
                CurrencyCode = openAccount.DesiredCurrencyCode,
            };

            try
            {
                xmlResponseObj = new ServiceUtilities.IBSBridgeProcessor<AccountOpeningRequestXML, AccountOpeningResponseXML>().Processor(clientRequest, true) as AccountOpeningResponseXML;

                if (xmlResponseObj.ResponseCode == "00")
                {
                    string cust_id = new AccountInquiryService().ValidateAccountNumber(new AccountRequest { AccountNumber = xmlResponseObj.AccountNo }).Result.AccountInformation.CustomerID;
                    Logger.LogInfo("OpenAccountService.OpenAccount.Response", xmlResponseObj);
                    return Task.Factory.StartNew(() => new OpenAccountResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = xmlResponseObj.ResponseText,
                        AccountNo = xmlResponseObj.AccountNo,
                        CustomerID = cust_id
                    });
                }
                else
                {
                    Logger.LogInfo("OpenAccountService.OpenAccount.Response", xmlResponseObj);
                    return Task.Factory.StartNew(() => new OpenAccountResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = xmlResponseObj.ResponseText
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Task.Factory.StartNew(() => new OpenAccountResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "An error occoured while processing request"
                });
            }
        }

        public Task<FundAccountResponse> FundAccount(FundAccountRequest req)
        {
            Logger.LogInfo("AccountServices.FundAccount, Input", req);
            FundAccountResponse response = new FundAccountResponse() { ResponseCode = "MW06", ResponseDescription = "System Hang!" };

            //bool useDemoAccountFor_FundAccountCardReq = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings.Get("UseDemoAccountFor_FundAccountCardReq"));
            //string demoAccountFor_FundAccountCardReq = System.Configuration.ConfigurationManager.AppSettings.Get("DemoAccountFor_FundAccountCardReq");
            //if(useDemoAccountFor_FundAccountCardReq)
            //{
            //    Logger.LogInfo("AccountServices.FundAccount, Using Demo Account", req);
            //}

            BaseResponse br = null;
            bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(req, out br);
            if (!tokenValid)
            {
                // TODO: Uncomment the commented lines below.
                //response = new FundAccountResponse
                //{
                //    ResponseCode = "01",
                //    ResponseDescription = br.ResponseDescription
                //};
                //return Task<FundAccountResponse>.Factory.StartNew(() => response);
            }
            
            if(string.IsNullOrWhiteSpace(req.PhoneNumber) || !req.PhoneNumber.StartsWith("+"))
            {
                response = new FundAccountResponse
                {
                    ResponseCode = "01",
                    ResponseDescription = "Phone Number is required and MUST include the country code. Sample: +2349012345678",
                };
                return Task<FundAccountResponse>.Factory.StartNew(() => response);
            }

            try
            {
                spayUrl += "FundAccountCardReq";
                string refID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));

                FundAccountRequestJSON apiRequest = new FundAccountRequestJSON()
                {
                    Referenceid = refID,
                    RequestType = "1025",
                    amount = req.Amount,
                    card_no = req.CardNo,
                    charge_auth = req.ChargeAuth,
                    CreditAccount = req.RecipientAccountNumberOrWallet,
                    cvv = req.CVV,
                    email = req.Email,
                    expiry_month = req.ExpiryMonth,
                    expiry_year = req.ExpiryYear,
                    fee = req.Fee,
                    firstname = req.FirstName,
                    lastname = req.LastName,
                    medium = req.Medium,
                    narration = req.Narration,
                    phonenumber = req.PhoneNumber,
                    pin = req.CardPIN,
                    recipient_account_number = req.RecipientAccountNumberOrWallet,
                    recipient_bank = req.RecipientBank,
                    redirecturl = req.RedirectURL,
                    TransactionReference = (string.IsNullOrWhiteSpace(req.TransactionReference) ? refID : req.TransactionReference),
                    Translocation = req.Translocation,
                };

                FundAccountResponseJSON apiResponse = new RESTProcessor<FundAccountRequestJSON, FundAccountResponseJSON>(spayUrl).DoPOST(apiRequest, true, false) as FundAccountResponseJSON;
                Logger.LogInfo("AccountServices.FundAccount, DoPOST Response => ", apiResponse);

                int respCode = -1;
                if (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.message) && Int32.TryParse(apiResponse.message.Trim(), out respCode) && respCode == 0)
                {
                    response = new FundAccountResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Success!",
                        ResponseDetails = null,
                    };

                    if (!string.IsNullOrWhiteSpace(apiResponse.response))
                    {
                        try
                        {
                            FundAccountResponseJSONDetails responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<FundAccountResponseJSONDetails>(Utils.ExtractJSON(apiResponse.response));
                            #region FundAccountResponseDetails
                            FundAccountResponseDetails respDetails = new FundAccountResponseDetails()
                            {
                                Status = responseData.status,
                                NetworkReferenceNumber = responseData.NetworkReferenceNumber,
                                RequestId = responseData.RequestId,
                                ResponseCode = responseData.ResponseCode,
                                Response_Description = responseData.Response_Description,
                                SettlementDate = responseData.SettlementDate,
                                SubmitDateTime = responseData.SubmitDateTime,
                                SystemTraceAuditNumber = responseData.SystemTraceAuditNumber,
                                TransactionReference = responseData.TransactionReference,
                                TransferType = responseData.TransferType,
                                ResponseData = null,
                            };
                            response.ResponseDetails = respDetails;

                            if (responseData.data != null)
                            {
                                FundAccountResponseData respData = new FundAccountResponseData()
                                {
                                    AuthUrl = responseData.data.authurl,
                                    ChargeMethod = responseData.data.chargeMethod,
                                    PendingValidation = responseData.data.pendingValidation,
                                    ResponseHTML = responseData.data.responsehtml,
                                    TransferDetails = null,
                                };
                                response.ResponseDetails.ResponseData = respData;

                                if (responseData.data.transfer != null)
                                {
                                    #region FundAccountResponseTransferDetails
                                    FundAccountResponseTransferDetails transfDetails = new FundAccountResponseTransferDetails()
                                    {
                                        Account = responseData.data.transfer.account,
                                        AccountId = responseData.data.transfer.accountId,
                                        AdditionalFields = responseData.data.transfer.additionalFields,
                                        AmountToCharge = responseData.data.transfer.amountToCharge,
                                        AmountToSend = responseData.data.transfer.amountToSend,
                                        BeneficiaryDetails = null,
                                        BeneficiaryId = responseData.data.transfer.beneficiaryId,
                                        CardId = responseData.data.transfer.cardId,
                                        ChargeCurrency = responseData.data.transfer.chargeCurrency,
                                        ChargedFee = responseData.data.transfer.chargedFee,
                                        DateCreated = responseData.data.transfer.createdAt,
                                        DateDeleted = responseData.data.transfer.deletedAt,
                                        DateUpdated = responseData.data.transfer.updatedAt,
                                        DisburseCurrency = responseData.data.transfer.disburseCurrency,
                                        ExchangeRate = responseData.data.transfer.exchangeRate,
                                        FirstName = responseData.data.transfer.firstName,
                                        FlutterChargeReference = responseData.data.transfer.flutterChargeReference,
                                        FlutterChargeResponseCode = responseData.data.transfer.flutterChargeResponseCode,
                                        FlutterChargeResponseMessage = responseData.data.transfer.flutterChargeResponseMessage,
                                        FlutterDisburseReference = responseData.data.transfer.flutterDisburseReference,
                                        FlutterDisburseResponseCode = responseData.data.transfer.flutterDisburseResponseCode,
                                        FlutterDisburseResponseMessage = responseData.data.transfer.flutterDisburseResponseMessage,
                                        ID = responseData.data.transfer.id,
                                        IPAddress = responseData.data.transfer.ip,
                                        LastName = responseData.data.transfer.lastName,
                                        LinkingReference = responseData.data.transfer.linkingReference,
                                        Medium = responseData.data.transfer.medium,
                                        MerchantCommission = responseData.data.transfer.merchantCommission,
                                        MerchantId = responseData.data.transfer.merchantId,
                                        Meta = new FundAccountResponseMetaDetails(),
                                        MoneywaveCommission = responseData.data.transfer.moneywaveCommission,
                                        NetDebitAmount = responseData.data.transfer.netDebitAmount,
                                        PhoneNumber = responseData.data.transfer.phoneNumber,
                                        R1 = responseData.data.transfer.r1,
                                        R2 = responseData.data.transfer.r2,
                                        ReceiptNumber = responseData.data.transfer.receiptNumber,
                                        RecipientPhone = responseData.data.transfer.recipientPhone,
                                        RedirectUrl = responseData.data.transfer.redirectUrl,
                                        Ref = responseData.data.transfer._ref,
                                        Source = responseData.data.transfer.source,
                                        SourceID = responseData.data.transfer.source_id,
                                        Status = responseData.data.transfer.status,
                                        SystemStatus = responseData.data.transfer.system_status,
                                        Type = responseData.data.transfer.type,
                                        UserId = responseData.data.transfer.userId,
                                    };
                                    response.ResponseDetails.ResponseData.TransferDetails = transfDetails;

                                    if (responseData.data.transfer.beneficiary != null)
                                    {
                                        #region FundAccountResponseBeneficiaryDetails
                                        FundAccountResponseBeneficiaryDetails bedefDetails = new FundAccountResponseBeneficiaryDetails()
                                        {
                                            AccountName = responseData.data.transfer.beneficiary.accountName,
                                            AccountNumber = responseData.data.transfer.beneficiary.accountNumber,
                                            BankCode = responseData.data.transfer.beneficiary.bankCode,
                                            BankName = responseData.data.transfer.beneficiary.bankName,
                                            Currency = responseData.data.transfer.beneficiary.currency,
                                            DateCreated = responseData.data.transfer.beneficiary.createdAt,
                                            DateDeleted = responseData.data.transfer.beneficiary.deletedAt,
                                            DateUpdated = responseData.data.transfer.beneficiary.updatedAt,
                                            ID = responseData.data.transfer.beneficiary.id,
                                            UserId = responseData.data.transfer.beneficiary.userId,
                                        };
                                        response.ResponseDetails.ResponseData.TransferDetails.BeneficiaryDetails = bedefDetails;
                                        #endregion
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(new Exception("In AccountServices.FundAccount. Exception while deserializing FundAccountResponseJSONDetails", ex));
                        }
                    }
                }
                else
                {
                    response = new FundAccountResponse
                    {
                        ResponseCode = "EP06",
                        ResponseDescription = $"Failed with code: {apiResponse.message}",
                    };
                }
            }
            catch (Exception)
            {
                response = new FundAccountResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Fund Account operation failed.",
                };
            }

            Logger.LogInfo("AccountServices.FundAccount, Response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<PNDResponse> PlacePND(PNDRequest Request)
        {
            throw new NotImplementedException();
        }

        public Task<StopChequeResponse> StopCheque(StopChequeRequest stopCheque)
        {
            throw new NotImplementedException();
        }

        public Task<BlockFundResponse> BlockFund(BlockFundRequest request)
        {
            Logger.LogInfo("AccountServices.BlockFund, input", request);

            BlockFundResponse response = new BlockFundResponse();
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(request, out br);
                if (!tokenValid)
                {
                    response = new BlockFundResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<BlockFundResponse>.Factory.StartNew(() => response);
                }

                BlockFundRequestXML clientRequest = new BlockFundRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "908",
                    NUBAN = request.AccountNumber,
                    Amount = request.Amount,
                };
                BlockFundResponseXML serverResponse = new ServiceUtilities.IBSBridgeProcessor<BlockFundRequestXML, BlockFundResponseXML>().Processor(clientRequest, true) as BlockFundResponseXML;

                if (serverResponse.ResponseCode == "00")
                {
                    response.ResponseCode = "00";
                    response.ResponseDescription = serverResponse.ResponseText;
                    response.LockID = serverResponse.LockID;
                }
                else
                {
                    response.ResponseCode = serverResponse.ResponseCode;
                    response.ResponseDescription = serverResponse.ResponseText;
                    response.LockID = serverResponse.LockID;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response.ResponseCode = "06";
                response.ResponseDescription = "Unable to process request";
            }
            Logger.LogInfo("AccountServices.BlockFund, input", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<UnBlockFundResponse> UnBlockFund(UnBlockFundRequest request)
        {
            Logger.LogInfo("AccountServices.BlockFund, input", request);

            UnBlockFundResponse response = new UnBlockFundResponse();
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(request, out br);
                if (!tokenValid)
                {
                    response = new UnBlockFundResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<UnBlockFundResponse>.Factory.StartNew(() => response);
                }

                UnBlockFundRequestXML clientRequest = new UnBlockFundRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "909",
                    LockID = request.LockID,
                };
                UnBlockFundResponseXML serverResponse = new ServiceUtilities.IBSBridgeProcessor<UnBlockFundRequestXML, UnBlockFundResponseXML>().Processor(clientRequest, true) as UnBlockFundResponseXML;

                if (serverResponse.ResponseCode == "00")
                {
                    response.ResponseCode = "00";
                    response.ResponseDescription = serverResponse.ResponseText;
                    response.LockID = serverResponse.LockID;
                }
                else
                {
                    response.ResponseCode = serverResponse.ResponseCode;
                    response.ResponseDescription = serverResponse.ResponseText;
                    response.LockID = serverResponse.LockID;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response.ResponseCode = "06";
                response.ResponseDescription = "Unable to process request";
            }
            Logger.LogInfo("AccountServices.BlockFund, input", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<UploadResponse> UploadDocument(UploadRequest request)
        {
            Logger.LogInfo("AccountServices.UploadDocument, input", request);

            UploadXML uploadXML = null;
            UploadResponse response = null;
            SterlingBaseResponse serverResponse = null;
            try
            {
                CustomerAccountsResponse accountInqResponse = new AccountInquiryService().GetAccountsWithCustomerID(new AccountRequest { CustomerID = request.CustomerNumber }).Result;
                if (accountInqResponse.ResponseCode != "00")
                {
                    response = new UploadResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Unable to retrieve account"
                    };
                    return Task.Factory.StartNew(() => response);
                }
                string AccountNumber = accountInqResponse.AccountInformation.FirstOrDefault().AccountNumber;
                uploadXML = new UploadXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    NUBAN = AccountNumber,
                    FileType = "jpg"
                };

                #region switch for upload type
                switch (request.UploadType)
                {
                    case UploadType.NationalID:
                        UploadIDXML xmlUpload = new UploadIDXML
                        {
                            ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                            NUBAN = AccountNumber,
                            ImageByte = Convert.FromBase64String(request.NationalIdentity),
                            FileType = "jpg",
                            RequestType = "928",
                            IDNO = request.IDNO
                        };
                        serverResponse = new ServiceUtilities.IBSBridgeProcessor<UploadXML, SterlingBaseResponse>().Processor(uploadXML, true) as SterlingBaseResponse;
                        break;
                    case UploadType.PictureUpload:
                        uploadXML.ImageByte = Convert.FromBase64String(request.Picture);
                        uploadXML.RequestType = "929";
                        break;
                    case UploadType.UtilityBill:
                        //uploadXML.ImageByte = Encoding.ASCII.GetBytes(request.UtilityBill);
                        uploadXML.ImageByte = Convert.FromBase64String(request.UtilityBill);
                        uploadXML.RequestType = "930";
                        break;
                    case UploadType.Signature:
                        //uploadXML.ImageByte = Encoding.ASCII.GetBytes(request.Signature);
                        uploadXML.ImageByte = Convert.FromBase64String(request.Signature);
                        uploadXML.RequestType = "940";
                        break;
                    default:
                        response = new UploadResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "Invalid upload type"
                        };
                        break;
                }
                #endregion

                if (response != null)
                {
                    return Task.Factory.StartNew(() => response);
                }
                else
                {
                    serverResponse = serverResponse != null ? serverResponse : new ServiceUtilities.IBSBridgeProcessor<UploadXML, SterlingBaseResponse>().Processor(uploadXML, true) as SterlingBaseResponse;

                    if (serverResponse.ResponseCode == "00")
                    {
                        response = new UploadResponse
                        {
                            ResponseCode = "00",
                            ResponseDescription = serverResponse.ResponseText
                        };
                    }
                    else
                    {
                        response = new UploadResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "Unable to upload file"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response.ResponseCode = "06";
                response.ResponseDescription = "Unable to process request";
            }
            Logger.LogInfo("AccountServices.UploadDocument, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<UpdateBvnResponse> UpdateBvn(UpdateBVNRequest request)
        {
            Logger.LogInfo("AccountServices.UpdateBvn, input", request);

            UpdateBvnXML ibsRequest = null;
            UpdateBvnResponse response = null;
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(request, out br);
                if (!tokenValid)
                {
                    response = new UpdateBvnResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<UpdateBvnResponse>.Factory.StartNew(() => response);
                }

                CustomerAccountsResponse accountInqResponse = new AccountInquiryService().GetAccountsWithCustomerID(new AccountRequest { CustomerID = request.CustomerNumber }).Result;
                if (accountInqResponse.ResponseCode != "00")
                {
                    response = new UpdateBvnResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Unable to retrieve account"
                    };
                    return Task.Factory.StartNew(() => response);
                }
                string AccountNumber = accountInqResponse.AccountInformation.FirstOrDefault().AccountNumber;

                ibsRequest = new UpdateBvnXML()
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "906",
                    NUBAN = AccountNumber,
                    Bvn = request.BVN
                };

                SterlingBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<UpdateBvnXML, SterlingBaseResponse>().Processor(ibsRequest, true) as SterlingBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new UpdateBvnResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL"
                    };
                }
                else
                {
                    response = new UpdateBvnResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new UpdateBvnResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Unable to update bvn"
                };
            }
            Logger.LogInfo("AccountServices.UpdateBvn, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<UpgradeResponse> UpgradeAccount(UpgradeRequest request)
        {
            Logger.LogInfo("AccountServices.UpgradeAccount, input", request);

            UpgradeAccountXML ibsRequest = null;
            UpgradeResponse response = null;
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(request, out br);
                if (!tokenValid)
                {
                    response = new UpgradeResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<UpgradeResponse>.Factory.StartNew(() => response);
                }

                CustomerAccountsResponse accountInqResponse = new AccountInquiryService().GetAccountsWithCustomerID(new AccountRequest { CustomerID = request.CustomerNumber }).Result;
                if (accountInqResponse.ResponseCode != "00")
                {
                    response = new UpgradeResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Unable to retrieve account"
                    };
                    return Task.Factory.StartNew(() => response);
                }
                string AccountNumber = accountInqResponse.AccountInformation.FirstOrDefault().AccountNumber;

                ibsRequest = new UpgradeAccountXML()
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "934",
                    acctnumber = AccountNumber,
                    acctBranch = "NG0020555",
                    newproduct = request.newproduct
                };

                SterlingBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<UpgradeAccountXML, SterlingBaseResponse>().Processor(ibsRequest, true) as SterlingBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new UpgradeResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL"
                    };
                }
                else
                {
                    response = new UpgradeResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new UpgradeResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Unable to update bvn"
                };
            }
            Logger.LogInfo("AccountServices.UpgradeAccount, response", request);
            return Task.Factory.StartNew(() => response);
        }
    }
}
