using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using Blend.SterlingImplementation.Entites;
using AppZoneMiddleware.Shared.Utility;
using Blend.SterlingImplementation.ServiceUtilities;

namespace Blend.SterlingImplementation.CoreBankingService
{
    public class InvestmentService : IInvestmentService
    {
        public Task<FixedDepositResponse> RequestFixedDeposit(FixedDepositRequest request)
        {
            Logger.LogInfo("InvestmentService.RequestFixedDeposit, input", request);

            FixedDepositResponse response = new FixedDepositResponse();
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(request, out br);
                if (!tokenValid)
                {
                    response = new FixedDepositResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<FixedDepositResponse>.Factory.StartNew(() => response);
                }

                FixedDepositRequestXML clientRequest = new FixedDepositRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    customerid = request.CustomerNumber,
                    amount = request.Amount,
                    currency = request.Currency,
                    rate = request.Rate,
                    changeperiod = request.ChangePeriod,
                    effectivedate = request.EffectiveDate,
                    payinacct = request.PayInAcct,
                    payoutacct1 = request.PayOutAcct1,
                    payoutacct2 = request.PayOutAcct2,
                    payoutacct3 = request.PayOutAcct3

                };

                switch (request.InterestType)
                {
                    case InterestType.WithInterest:
                        clientRequest.RequestType = "927";
                        break;
                    case InterestType.WithoutInterest:
                        clientRequest.RequestType = "926";
                        break;
                    default:
                        break;
                }
                FixedDepositResponseXML serverResponse = new ServiceUtilities.IBSBridgeProcessor<FixedDepositRequestXML, FixedDepositResponseXML>().Processor(clientRequest, true) as FixedDepositResponseXML;

                if (serverResponse.ResponseCode == "00")
                {
                    response.ResponseCode = serverResponse.ResponseCode;
                    response.ResponseDescription = serverResponse.ResponseText;
                    response.ArrangementID = serverResponse.arrangementid;
                }
                else
                {
                    response.ResponseCode = "EP96";
                    response.ResponseDescription = serverResponse.ResponseText;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response.ResponseCode = "MW06";
                response.ResponseDescription = "Unable to process request";
            }
            Logger.LogInfo("InvestmentService.RequestFixedDeposit, Response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<TreasuryBillResponse> RequestTreasuryBill(TreasuryBillRequest request)
        {
            Logger.LogInfo("AccountServices.RequestTreasuryBill, input", request);

            TreasuryBillResponse response = new TreasuryBillResponse();
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(request, out br);
                if (!tokenValid)
                {
                    response = new TreasuryBillResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<TreasuryBillResponse>.Factory.StartNew(() => response);
                }

                TreasuryBillRequestXML clientRequest = new TreasuryBillRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "910",
                    NUBAN = request.NUBAN,
                    tenor = request.tenor,
                    f_value = request.f_value
                };
                SterlingBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<TreasuryBillRequestXML, SterlingBaseResponse>().Processor(clientRequest, true) as SterlingBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response.ResponseCode = serverResponse.ResponseCode;
                    response.ResponseDescription = serverResponse.ResponseText;
                }
                else
                {
                    response.ResponseCode = "EP96";
                    response.ResponseDescription = serverResponse.ResponseText;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response.ResponseCode = "MW06";
                response.ResponseDescription = "Unable to process request";
            }
            Logger.LogInfo("AccountServices.RequestTreasuryBill, Response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<TerminateDepositResponse> TerminateFixedDeposit(TerminateDepositRequest request)
        {
            Logger.LogInfo("AccountServices.TerminateFixedDeposit, input", request);

            TerminateDepositResponse response = new TerminateDepositResponse();
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(request, out br);
                if (!tokenValid)
                {
                    response = new TerminateDepositResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<TerminateDepositResponse>.Factory.StartNew(() => response);
                }

                TerminateFixedDepositXML clientRequest = new TerminateFixedDepositXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "925",
                    arrangementid = request.arrangementid
                };
                SterlingBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<TerminateFixedDepositXML, SterlingBaseResponse>().Processor(clientRequest, true) as SterlingBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response.ResponseCode = serverResponse.ResponseCode;
                    response.ResponseDescription = serverResponse.ResponseText;
                }
                else
                {
                    response.ResponseCode = "EP96";
                    response.ResponseDescription = serverResponse.ResponseText;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response.ResponseCode = "MW06";
                response.ResponseDescription = "Unable to process request";
            }
            Logger.LogInfo("AccountServices.TerminateFixedDeposit, Response", response);
            return Task.Factory.StartNew(() => response);
        }
    }
}
