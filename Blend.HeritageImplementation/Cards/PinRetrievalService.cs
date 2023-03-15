using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using Blend.HeritageImplementation.Utility;
using AppZoneMiddleware.Shared.Utility;

namespace Blend.HeritageImplementation.Cards
{
    public class PinRetrievalService : IPinService
    {
        public string PASSWORD = System.Configuration.ConfigurationManager.AppSettings.Get("TWIG_PASSWORD");
        public string USERNAME = System.Configuration.ConfigurationManager.AppSettings.Get("TWIG_USERNAME");
        public string NEXT_TOKEN;
        public string ENCRYPTEDPIN;
        public PinRetrievalService()
        {
        }
        public PinRetrievalResponse PinRetrieval(PinRetrievalRequest Request)
        {
            PinRetrievalResponse response = new PinRetrievalResponse();
            Logger.LogInfo("PinRetrievalService.PinRetrieval.Request:", Request);
            try
            {

                PinService.IpinServiceSoapClient pinClient = new PinService.IpinServiceSoapClient();
                PinService.LogonResp clientResponse = pinClient.Logon(USERNAME, PASSWORD);
                if (clientResponse.Successful == true)
                {
                    NEXT_TOKEN = clientResponse.NextChallenge;
                    ENCRYPTEDPIN = new CryptoGraphy().GenerateEncryptedPassword(NEXT_TOKEN, "TWIGPASS");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                string exceptionString = ex.InnerException != null ? ", Inner Exception: " + ex.InnerException.Message : "";
                response = new PinRetrievalResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = ex.Message + exceptionString
                };
                return response;
            }

            if (ENCRYPTEDPIN != string.Empty)
            {

                PinService.IpinServiceSoapClient pinClient = new PinService.IpinServiceSoapClient();
                PinService.OperationResp operationResponse = pinClient.RetrievePIN(ENCRYPTEDPIN, NEXT_TOKEN, Request.Bin, Request.Last4Pan, Request.AccountNumber, Request.ExpDate);
                Logger.LogInfo("PinRetrievalService.PinRetrieval.ServiceResponse:", operationResponse);

                if (operationResponse.Successful == true)
                {
                    if (operationResponse.Result.Contains("ERROR"))
                    {
                        response = new PinRetrievalResponse()
                        {
                            ResponseCode = "06",
                            ResponseDescription = operationResponse.Result,
                            PIN = operationResponse.Result,
                        };
                    }
                    else
                    {
                        response = new PinRetrievalResponse()
                        {
                            ResponseCode = "00",
                            ResponseDescription = "SUCCESFUL",
                            PIN = "",
                        };
                    }
                    
                  

                }
                else
                {
                    response = new PinRetrievalResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = operationResponse.ErrorMessage
                    };
                }
            }
            Logger.LogInfo("PinRetrievalService.PinRetrieval.Response:", response);
            return response;
        }
    }
}
