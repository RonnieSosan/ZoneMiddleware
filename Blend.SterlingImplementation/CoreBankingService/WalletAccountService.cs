using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Extension;
using AppZoneMiddleware.Shared.Utility;
using Blend.SterlingImplementation.Persistence;
using Blend.SterlingImplementation.Entites;
using Blend.SterlingImplementation.ServiceUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.CoreBankingService
{
    public class WalletAccountService
    {
        private string SterlingWalletURL = System.Configuration.ConfigurationManager.AppSettings.Get("SterlingSpayURL");

        public Task<WalletResponse> CreateWalletAccount(CreateWalletRequest request)
        {
            Logger.LogInfo("WalletAccountService,CreateWalletAccount Input", request);

            WalletResponse ApiResponse = null;
            try
            {
                SterlingWalletURL += "SBPMWalletAccountReq";

                string ApiRequest = string.Empty;
                request.RequestType = "108";
                request.Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));

                var theRequest = new WalletRequest()
                {
                    ADDR_LINE1 = request.ADDR_LINE1,
                    ADDR_LINE2 = request.ADDR_LINE2,
                    BIRTHDATE = request.BIRTHDATE,
                    CATEGORYCODE = request.CATEGORYCODE,
                    CUST_STATUS = request.CUST_STATUS,
                    CUST_TYPE = request.CUST_TYPE,
                    EMAIL = request.EMAIL,
                    FIRSTNAME = request.FIRSTNAME,
                    GENDER = request.GENDER,
                    LASTNAME = request.LASTNAME,
                    MARITAL_STATUS = request.MARITAL_STATUS,
                    MOBILE = request.MOBILE,
                    NATIONALITY = request.NATIONALITY,
                    Referenceid = request.Referenceid,
                    RequestType = request.RequestType,
                    RESIDENCE = request.RESIDENCE,
                    SECTOR = request.SECTOR,
                    TARGET = request.TARGET,
                    TITLE = request.TITLE,
                    Translocation = request.Translocation,
                };

                ApiResponse = new RESTProcessor<WalletRequest, WalletResponse>(SterlingWalletURL).DoPOST(theRequest, true, false) as WalletResponse;

                ApiResponse.ResponseCode = ApiResponse.response;
                ApiResponse.ResponseDescription = ApiResponse.message;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ApiResponse = new WalletResponse
                {
                    ResponseCode = "MW96",
                    ResponseDescription = "Unable to process request: " + ex.Message
                };
            };

            return Task.Factory.StartNew(() => ApiResponse);
        }

        public Task<WalletResponse> GetWalletBalance(WalletBalanceRequest request)
        {
            Logger.LogInfo("WalletAccountService,GetWalletBalance Input", request);

            WalletResponse ApiResponse = null;
            try
            {
                SterlingWalletURL += "SBPMWalletBalReq";

                string ApiRequest = string.Empty;
                request.RequestType = "108";
                request.Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));
                
                ApiResponse = new RESTProcessor<WalletBalanceRequest, WalletResponse>(SterlingWalletURL).DoPOST(request, true, false) as WalletResponse;

                ApiResponse.ResponseCode = ApiResponse.response;
                ApiResponse.ResponseDescription = ApiResponse.message;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ApiResponse = new WalletResponse
                {
                    ResponseCode = "MW96",
                    ResponseDescription = "Unable to process request: " + ex.Message
                };
            };

            return Task.Factory.StartNew(() => ApiResponse);
        }

        public Task<WalletDetails> GetWalletDetails(GetWalletDetails request)
        {
            Logger.LogInfo("WalletAccountService,GetWalletDetails Input", request);

            WalletDetails details = new WalletDetails();
            try
            {
                SterlingWalletURL += "SBPMWalletDetReq";

                string ApiRequest = string.Empty;
                request.RequestType = "108";
                request.Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));
                
                details = new RESTProcessor<GetWalletDetails, WalletDetails>(SterlingWalletURL).DoPOST(request, true, false) as WalletDetails;

                details.ResponseCode = details.response;
                details.ResponseDescription = details.message;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                details = new WalletDetails
                {
                    ResponseCode = "MW96",
                    ResponseDescription = "Unable to process request: " + ex.Message
                };
            };

            return Task.Factory.StartNew(() => details);
        }

        public Task<WalletTransactionResponse> DoWalletTransaction(WalletTransactionRequest request)
        {
            Logger.LogInfo("WalletAccountService,DoWalletTransaction Input", request);

            WalletTransactionResponse ApiResponse = null;
            try
            {
                SterlingWalletURL += "SBPMWalletRequest";

                string ApiRequest = string.Empty;
                request.RequestType = "108";
                request.Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));

                ApiResponse = new RESTProcessor<WalletTransactionRequest, WalletTransactionResponse>(SterlingWalletURL).DoPOST(request, true, false) as WalletTransactionResponse;

                ApiResponse.ResponseCode = ApiResponse.response;
                ApiResponse.ResponseDescription = ApiResponse.message;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ApiResponse = new WalletTransactionResponse
                {
                    ResponseCode = "MW96",
                    ResponseDescription = "Unable to process request: " + ex.Message
                };
            };

            return Task.Factory.StartNew(() => ApiResponse);
        }
    }
}
