using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using Blend.SterlingImplementation.Entites;
using System.Xml.Serialization;
using AppZoneMiddleware.Shared.Utility;
using System.IO;
using Blend.SharedServiceImplementation;
using Blend.SterlingImplementation.ServiceUtilities;

namespace Blend.SterlingImplementation.CoreBankingService
{
    public class FundTransferService : IBankTransfer
    {
        readonly bool NIPDemoMode;

        public FundTransferService()
        {
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("NIPDemoMode"), out NIPDemoMode);
        }

        public Task<FundsTransferResponse> InterBankTransfer(FundsTransferRequest TransferRequest)
        {
            Logger.LogInfo("FundTransferService.InterBankTransfer", TransferRequest);
            FundsTransferResponse transResonse = null;

            try
            {
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidatePinFromExternalSource(TransferRequest, out BaseResponse br);
                if (!tokenValid)
                {
                    // TODO: Uncomment the commented lines below.
                    //transResonse = new FundsTransferResponse
                    //{
                    //    ResponseCode = "MW01",
                    //    ResponseDescription = br.ResponseDescription,
                    //};
                    //return Task<FundsTransferResponse>.Factory.StartNew(() => transResonse);
                }

                if(string.IsNullOrWhiteSpace(TransferRequest.FromAccount) || (TransferRequest.FromAccount.Trim().Length != 10 && TransferRequest.FromAccount.Trim().Length != 13))
                {
                    transResonse = new FundsTransferResponse
                    {
                        ResponseCode = "MW01",
                        ResponseDescription = "The 'From' Account must be a NUBAN or WALLET account number.",
                    };
                    return Task<FundsTransferResponse>.Factory.StartNew(() => transResonse);
                }

                if (NIPDemoMode)
                {
                    transResonse = new FundsTransferResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "TRANSACTION SUCCESSFUL"
                    };
                    Logger.LogInfo("FundTransferService.InterBankTransfer, Input:", transResonse);
                    return Task.Factory.StartNew(() => transResonse);
                }

                // Do Inter-Bank Name Inquiry
                NameInquiryRequest niReq = new NameInquiryRequest()
                {
                    myDestinationBankCode = TransferRequest.BeneficiaryBank,
                    DestinationAccountNumber = TransferRequest.ToAccount,
                    AuthToken = TransferRequest.AuthToken,
                    CustomerNumber = TransferRequest.CustomerNumber,
                    customer_id = TransferRequest.customer_id,
                    isMobile = TransferRequest.isMobile,
                    MailRequest = TransferRequest.MailRequest,
                    myAccountNumber = TransferRequest.FromAccount,
                    myChannelCode = TransferRequest.RequestChannel,
                    Passkey = TransferRequest.Passkey,
                    PhoneNumber = TransferRequest.PhoneNumber,
                    PIN = TransferRequest.PIN,
                    RequestChannel = TransferRequest.RequestChannel,
                };
                NameInquiryResponse niResp = new AccountInquiryService().InterBankNameInquiry(niReq).Result;
                string sessionID = (niResp != null && niResp.ResponseCode == "00" ? niResp.SessionID : string.Empty);

                if(string.IsNullOrWhiteSpace(sessionID))
                {
                    transResonse = new FundsTransferResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Name Inquiry Failed!"
                    };
                    Logger.LogInfo("FundTransferService.InterBankTransfer, result:", transResonse);
                    return Task.Factory.StartNew(() => transResonse);
                }

                SterlingBaseResponse fundsResponse = new SterlingBaseResponse();
                XMLIBTransferRequest request = new XMLIBTransferRequest()
                {
                    FromAccount = TransferRequest.FromAccount,
                    ToAccount = TransferRequest.ToAccount,
                    PaymentReference = TransferRequest.MyPaymentReference, // "Prime";
                    Amount = TransferRequest.Amount.ToString(),
                    DestinationBankCode = TransferRequest.BeneficiaryBank,
                    NEResponse = "00",
                    BenefiName = TransferRequest.BeneficiaryName,
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "101",
                    SessionID = sessionID,
                };


                fundsResponse = new ServiceUtilities.IBSBridgeProcessor<XMLIBTransferRequest, SterlingBaseResponse>().Processor(request, true) as SterlingBaseResponse;

                if (fundsResponse.ResponseCode == "00" && TransferRequest.MailRequest != null)
                {
                    Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailRequest>(Newtonsoft.Json.JsonConvert.SerializeObject(TransferRequest.MailRequest)));
                }

                transResonse = new FundsTransferResponse
                {
                    ResponseCode = fundsResponse.ResponseCode,
                    ResponseDescription = fundsResponse.ResponseText
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                transResonse = new FundsTransferResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "An error occoured while processing request"
                };
            }

            Logger.LogInfo("FundTransferService.InterBankTransfer, result:", transResonse);
            return Task.Factory.StartNew(() => transResonse);
        }

        public LifestyleDebitResponse LifeStyleDebit(LifestyleDebitRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IntraBankTransactionResponse> SameBankTransfer(IntraBankTransactionRequest theTransaction)
        {
            Logger.LogInfo("FundTransferService.SameBankTransfer", theTransaction);
            IntraBankTransactionResponse transResonse = null;
            try
            {
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidatePinFromExternalSource(theTransaction, out BaseResponse br);
                if (!tokenValid)
                {
                    transResonse = new IntraBankTransactionResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<IntraBankTransactionResponse>.Factory.StartNew(() => transResonse);
                }

                if (string.IsNullOrWhiteSpace(theTransaction.DebitAccount) || theTransaction.DebitAccount.Contains("+") || (theTransaction.DebitAccount.Trim().Length != 10 && theTransaction.DebitAccount.Trim().Length != 13))
                {
                    transResonse = new IntraBankTransactionResponse
                    {
                        ResponseCode = "MW01",
                        ResponseDescription = "The 'Debit' Account must be a NUBAN account number or a phone number of the form: 2348012345678. NB: There is should be no '+' sign.",
                    };
                    return Task<IntraBankTransactionResponse>.Factory.StartNew(() => transResonse);
                }

                theTransaction.DebitAccount = theTransaction.DebitAccount.Trim();
                if (theTransaction.DebitAccount.Length == 10)
                {
                    Logger.LogInfo("FundTransferService.SameBankTransfer", "Posting for NUBAN");
                    SterlingBaseResponse fundsResponse = new SterlingBaseResponse();
                    XMLTransferRequest request = new XMLTransferRequest()
                    {
                        FromAccount = theTransaction.DebitAccount,
                        ToAccount = theTransaction.CreditAccount1,
                        PaymentReference = theTransaction.RRN, // "Prime";
                        Amount = theTransaction.Amount.ToString(),
                        ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                        RequestType = "102"
                    };

                    fundsResponse = new ServiceUtilities.IBSBridgeProcessor<XMLTransferRequest, SterlingBaseResponse>().Processor(request, true) as SterlingBaseResponse;

                    if (fundsResponse.ResponseCode == "00" && theTransaction.MailRequest != null)
                    {
                        Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailRequest>(Newtonsoft.Json.JsonConvert.SerializeObject(theTransaction.MailRequest)));
                    }

                    transResonse = new IntraBankTransactionResponse
                    {
                        ResponseCode = fundsResponse.ResponseCode,
                        ResponseDescription = fundsResponse.ResponseText
                    };
                }
                else
                {
                    Logger.LogInfo("FundTransferService.SameBankTransfer", "Posting for Wallet");
                    WalletTransactionRequest wtReq = new WalletTransactionRequest()
                    {
                        amt = theTransaction.Amount.ToString(),
                        exp_code = "100",
                        frmacct = theTransaction.DebitAccount,
                        paymentRef = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                        Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                        remarks = theTransaction.Narration,
                        RequestType = "108",
                        tellerid = "1111",
                        toacct = theTransaction.CreditAccount1,
                        Translocation = "6.587363,7.494737"
                    };

                    WalletTransactionResponse wtResp = new WalletAccountService().DoWalletTransaction(wtReq).Result;

                    if (wtResp.ResponseCode == "00" && theTransaction.MailRequest != null)
                    {
                        Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailRequest>(Newtonsoft.Json.JsonConvert.SerializeObject(theTransaction.MailRequest)));
                    }

                    transResonse = new IntraBankTransactionResponse
                    {
                        ResponseCode = wtResp.ResponseCode,
                        ResponseDescription = wtResp.ResponseDescription,
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                transResonse = new IntraBankTransactionResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "An error occoured while processing request"
                };
            }

            Logger.LogInfo("FundTransferService.SameBankTransfer", transResonse);
            return Task<IntraBankTransactionResponse>.Factory.StartNew(() => transResonse);
        }
    }
}
