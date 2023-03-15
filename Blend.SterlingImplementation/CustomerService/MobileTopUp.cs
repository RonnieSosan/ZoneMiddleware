using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.SterlingImplementation.Entites;
using Blend.SterlingImplementation.ServiceUtilities;

namespace Blend.SterlingImplementation.CustomerService
{
    public class MobileTopUp : IAirtimeTopup
    {
        public Task<AirtimeResponse> DoAirtimeTopUp(AirtimeRequest request)
        {
            Logger.LogInfo("MobileTopUp.DoAirtimeTopUp, Request:", request);
            AirtimeResponse response = null;
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidatePinFromExternalSource(request, out br);
                if (!tokenValid)
                {
                    response = new AirtimeResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<AirtimeResponse>.Factory.StartNew(() => response);
                }


                AirtimeExtraData extras = Newtonsoft.Json.JsonConvert.DeserializeObject<AirtimeExtraData>(request.ExtraParams);

                //Validate and update mobile number for international format phone numbers
                string myMobile = request.PhoneNumber.StartsWith("0") ? "234" + request.PhoneNumber.Remove(0, 1) : request.PhoneNumber;
                string myBeneficiaryMobile = extras.BeneficiaryPhoneNumber.StartsWith("0") ? "234" + extras.BeneficiaryPhoneNumber.Remove(0, 1) : extras.BeneficiaryPhoneNumber;

                AirtimeRequestXML clientRequest = new AirtimeRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "932",
                    Amount = request.RechargeAmount.ToString(),
                    NUBAN = request.CustomerAccount,
                    Mobile = myMobile,
                    Beneficiary = myBeneficiaryMobile,
                    Type = extras.Type
                };

                switch (request.BatchId)
                {
                    case Telco.Airtel:
                        clientRequest.NetworkID = "3";
                        break;
                    case Telco.Etisalat:
                        clientRequest.NetworkID = "1";
                        break;
                    case Telco.Visafone:
                        response = new AirtimeResponse
                        {
                            ResponseCode = "MW96",
                            ResponseDescription = "This telco is disabled"
                        };
                        break;
                    case Telco.MTN:
                        clientRequest.NetworkID = "4";
                        break;
                    case Telco.Glo:
                        clientRequest.NetworkID = "2";
                        break;
                    default:
                        response = new AirtimeResponse
                        {
                            ResponseCode = "MW96",
                            ResponseDescription = "Invalid Telco Type"
                        };
                        break;
                }

                if (response != null)
                {
                    return Task.Factory.StartNew(() => response);
                }

                SterlingBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<AirtimeRequestXML, SterlingBaseResponse>().Processor(clientRequest, true) as SterlingBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new AirtimeResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
                else
                {
                    response = new AirtimeResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new AirtimeResponse
                {
                    ResponseCode = "MW96",
                    ResponseDescription = "Unable to process request"
                };
            }

            Logger.LogInfo("MobileTopUp.DoAirtimeTopUp, Response:", response);
            return Task.Factory.StartNew(() => response);
        }
    }
}
