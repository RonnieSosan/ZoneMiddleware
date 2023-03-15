using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.SterlingImplementation.Entites;
using AppZoneMiddleware.Shared.DAO;
using Blend.SterlingImplementation.CoreBankingService;
using Blend.SterlingImplementation.ServiceUtilities;

namespace Blend.SterlingImplementation.CardManagementServices
{
    public class CardServices : ICardServices
    {
        bool isdemo;
        public CardServices()
        {
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("HotListCardDemo"), out isdemo);
        }
        public Task<CardResponse> CardRequest(CardRequest cardRequest)
        {
            Logger.LogInfo("CardServices.CardRequest, Input", cardRequest);
            CardResponse response = null;
            try
            {
                string request = Newtonsoft.Json.JsonConvert.SerializeObject(cardRequest);

                SterlingBaseResponse cardRequestResponse = null;
                CardRequestXML clientRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<CardRequestXML>(request);
                clientRequest.AppID = cardRequest.CardType == "1" ? "1" : "2";
                clientRequest.RequestType = "106";
                clientRequest.ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));

                cardRequestResponse = new ServiceUtilities.IBSBridgeProcessor<CardRequestXML, SterlingBaseResponse>().Processor(clientRequest, true) as SterlingBaseResponse;
                if (cardRequestResponse.ResponseCode == "00")
                {
                    response = new CardResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Card Request successfully logged"
                    };
                }
                else
                {
                    response = new CardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Could not log card request"
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new CardResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "an error occured while processing request"
                };
            }
            return Task.Factory.StartNew(() => response);
        }

        public Task<HotlistCardResponse> HotlistCard(HotlistCardRequest hotlistCardRequest)
        {
            Logger.LogInfo("CardServices.HotlistCard, Input", hotlistCardRequest);

            HotlistCardResponse response = null;
            try
            {
                if (isdemo)
                {
                    response = new HotlistCardResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Card succesfully blocked"
                    };
                    Logger.LogInfo("CardServices.HotlistCard, response", response);
                    return Task.Factory.StartNew(() => response);
                }

                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(hotlistCardRequest, out br);
                if (!tokenValid)
                {
                    response = new HotlistCardResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<HotlistCardResponse>.Factory.StartNew(() => response);
                }

                #region Checks and Account Retrieval
                RetrieveCardResponse activeCards = GetActiveCards(new RetrieveCardRequest()
                {
                    AccountNumber = string.Empty,
                    AuthToken = hotlistCardRequest.AuthToken,
                    CustomerNumber = hotlistCardRequest.CustomerNumber,
                    customer_id = hotlistCardRequest.customer_id,
                    ExpDate = string.Empty,
                    isMobile = hotlistCardRequest.isMobile,
                    MailRequest = hotlistCardRequest.MailRequest,
                    PAN = hotlistCardRequest.CardPAN,
                    Passkey = hotlistCardRequest.Passkey,
                    PhoneNumber = hotlistCardRequest.PhoneNumber,
                    PIN = hotlistCardRequest.PIN,
                    RequestChannel = hotlistCardRequest.RequestChannel,
                }).Result;

                if (activeCards.ResponseCode != "00")
                {
                    response = new HotlistCardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Unable to fetch cards for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                if (activeCards.Cards == null || activeCards.Cards.Count <= 0)
                {
                    response = new HotlistCardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No card record was found for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                if (!activeCards.Cards.Any(x => string.Equals(x.pan, hotlistCardRequest.CardPAN)))
                {
                    response = new HotlistCardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No matching card record was found for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                var anAccount = activeCards.Cards.FirstOrDefault(x => string.Equals(x.pan, hotlistCardRequest.CardPAN) && x.TheAccount != null && !string.IsNullOrWhiteSpace(x.TheAccount.AccountNumber));
                if (anAccount == null)
                {
                    response = new HotlistCardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No matching account was found for this card."
                    };
                    return Task.Factory.StartNew(() => response);
                }
                string accountNumber = anAccount.TheAccount.AccountNumber;

                AccountResponse accountDetails = new AccountInquiryService().ValidateAccountNumber(new AccountRequest()
                {
                    AccountNumber = accountNumber,
                    AuthToken = hotlistCardRequest.AuthToken,
                    CustomerNumber = hotlistCardRequest.CustomerNumber,
                    customer_id = hotlistCardRequest.customer_id,
                    isMobile = hotlistCardRequest.isMobile,
                    MailRequest = hotlistCardRequest.MailRequest,
                    Passkey = hotlistCardRequest.Passkey,
                    PhoneNumber = hotlistCardRequest.PhoneNumber,
                    PIN = hotlistCardRequest.PIN,
                    RequestChannel = hotlistCardRequest.RequestChannel,
                    CustomerID = hotlistCardRequest.CustomerNumber,
                }).Result;

                if (accountDetails == null || accountDetails.ResponseCode != "00" || accountDetails.AccountInformation == null)
                {
                    response = new HotlistCardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Could not validate details of the account linked to this card."
                    };
                    return Task.Factory.StartNew(() => response);
                }
                #endregion

                DeactivateCardXML clientRequest = new DeactivateCardXML
                {
                    ReqeuestID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "1111",
                    AccountNumber = accountNumber,
                    Pan = hotlistCardRequest.CardPAN,
                    AccountType = accountDetails.AccountInformation.AccountType,
                };

                CardBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<DeactivateCardXML, CardBaseResponse>().Processor(clientRequest, true, true, Encoding.UTF8) as CardBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new HotlistCardResponse
                    {
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
                else
                {
                    response = new HotlistCardResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new HotlistCardResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Card Hotlist failed"
                };
            }
            Logger.LogInfo("CardServices.HotlistCard, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<CardResponse> VisaCardRequest(VisaCardRequest cardRequest)
        {
            Logger.LogInfo("CardServices.VisaCardRequest, Input", cardRequest);
            CardResponse response = null;
            try
            {
                string request = Newtonsoft.Json.JsonConvert.SerializeObject(cardRequest);
                SterlingBaseResponse cardRequestResponse = null;

                VisaCardRequestXML clientRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<VisaCardRequestXML>(request);
                clientRequest.AppID = "3";
                clientRequest.RequestType = "106";
                clientRequest.ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff"));

                cardRequestResponse = new ServiceUtilities.IBSBridgeProcessor<VisaCardRequestXML, SterlingBaseResponse>().Processor(clientRequest, true) as SterlingBaseResponse;
                if (cardRequestResponse.ResponseCode == "00")
                {
                    response = new CardResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Card Request successfully logged"
                    };
                }
                else
                {
                    response = new CardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Could not log card request"
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new CardResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "an error occured while processing request"
                };
            }
            return Task.Factory.StartNew(() => response);
        }

        public Task<ActivateCardResponse> ActivateCard(ActivateCardRequest ActivateCardRequest)
        {
            Logger.LogInfo("CardServices.ActivateCard, Input", ActivateCardRequest);
            ActivateCardResponse response = null;
            if (isdemo)
            {
                response = new ActivateCardResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "Card Activation Successful"
                };
                return Task.Factory.StartNew(() => response);
            }

            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(ActivateCardRequest, out br);
                if (!tokenValid)
                {
                    response = new ActivateCardResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<ActivateCardResponse>.Factory.StartNew(() => response);
                }

                ActivateCardRequestXML clientRequest = new ActivateCardRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "924",
                    exp = ActivateCardRequest.ExpDate,
                    pan = ActivateCardRequest.PAN,
                    SelectedPin = ActivateCardRequest.ActivationPin,
                    seq_nr = ActivateCardRequest.Seq_nr,
                };

                ActivateCardResponseXML serverResponse = new ServiceUtilities.IBSBridgeProcessor<ActivateCardRequestXML, ActivateCardResponseXML>().Processor(clientRequest, true) as ActivateCardResponseXML;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new ActivateCardResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Card Activation Successful"
                    };
                }
                else
                {
                    response = new ActivateCardResponse
                    {
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = string.Format("Activation Failed. Reason: {0}", !string.IsNullOrWhiteSpace(serverResponse.ResponseText) && serverResponse.ResponseText.Contains("|") ? serverResponse.ResponseText.Substring(serverResponse.ResponseText.LastIndexOf("|")) : "Unknown"),
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new ActivateCardResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "Activation Failed"
                };
            }


            Logger.LogInfo("CardServices.ActivateCard, response", response);
            return Task.Factory.StartNew(() => response);
        }

        /// <summary>
        /// Deprecated! Do not use!!! 
        /// This is kept here for reference purposes ONLY. It has been re-implemented to call IBS Bridge. 
        /// </summary>
        /// <param name="ActivateCardRequest"></param>
        /// <returns></returns>
        public Task<ActivateCardResponse> ActivateCard_SOAP(ActivateCardRequest ActivateCardRequest)
        {
            Logger.LogInfo("CardServices.ActivateCard_SOAP, Input", ActivateCardRequest);
            ActivateCardResponse response = null;
            if (isdemo)
            {
                response = new ActivateCardResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "Card Activation Successful"
                };
                return Task.Factory.StartNew(() => response);
            }
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(ActivateCardRequest, out br);
                if (!tokenValid)
                {
                    response = new ActivateCardResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<ActivateCardResponse>.Factory.StartNew(() => response);
                }

                using (SterlingCardService.CardsSoapClient cardClient = new SterlingCardService.CardsSoapClient())
                {
                    SterlingCardService.ArrayOfString responseFromService = cardClient.ActivateNewCard(ActivateCardRequest.PAN, ActivateCardRequest.Seq_nr, ActivateCardRequest.ExpDate, ActivateCardRequest.ActivationPin);
                    Logger.LogInfo("CardServices.ActivateCard, response from service", responseFromService);
                    if (responseFromService.FirstOrDefault() == "00")
                    {
                        response = new ActivateCardResponse
                        {
                            ResponseCode = "00",
                            ResponseDescription = "Card Activation Successful"
                        };
                    }
                    else
                    {
                        response = new ActivateCardResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "Activation Failed"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new ActivateCardResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "Activation Failed"
                };
            }


            Logger.LogInfo("CardServices.ActivateCard_SOAP, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<RetrieveCardResponse> GetActiveCards(RetrieveCardRequest cardRequest)
        {
            Logger.LogInfo("CardServices.GetActiveCards, Input", cardRequest);
            List<Card> cards = new List<Card>();

            RetrieveCardResponse response = null;
            if (isdemo)
            {
                cards.Add(new Card { expiry_date = "1812", pan = "5061104947862517319", seq_nr = "2" });
                response = new RetrieveCardResponse
                {
                    Cards = cards,
                    ResponseCode = "00",
                    ResponseDescription = "SUCCESSFUL",
                };
                Logger.LogInfo("CardServices.GetActiveCards, response", response);
                return Task.Factory.StartNew(() => response);
            }

            if (string.IsNullOrWhiteSpace(cardRequest.CustomerNumber))
            {
                response = new RetrieveCardResponse
                {
                    Cards = cards,
                    ResponseCode = "MW06",
                    ResponseDescription = "'CustomerNumber' is required.",
                };
                Logger.LogInfo("CardServices.GetActiveCards, response", response);
                return Task.Factory.StartNew(() => response);
            }

            try
            {
                GetActiveCardsByCustomerIDRequestXML clientRequest = new GetActiveCardsByCustomerIDRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "944",
                    CustomerID = cardRequest.CustomerNumber,
                };

                GetActiveCardsByCustomerIDResponseXML serverResponse = new ServiceUtilities.IBSBridgeProcessor<GetActiveCardsByCustomerIDRequestXML, GetActiveCardsByCustomerIDResponseXML>().Processor(clientRequest, true) as GetActiveCardsByCustomerIDResponseXML;

                if (serverResponse.ResponseCode == "00")
                {
                    if(string.IsNullOrWhiteSpace(serverResponse.ResponseText))
                    {
                        response = new RetrieveCardResponse
                        {
                            Cards = cards,
                            ResponseCode = "06",
                            ResponseDescription = serverResponse.ResponseText,
                        };
                        return Task.Factory.StartNew(() => response);
                    }

                    string[] CardRecords = serverResponse.ResponseText.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in CardRecords)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string[] cardDetails = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            if (cardDetails.Length == 4)
                            {
                                cards.Add(new Card { pan = cardDetails[0], expiry_date = cardDetails[1], seq_nr = cardDetails[2], TheAccount = new Account() { AccountNumber = cardDetails[3] } });
                            }
                            else
                            {
                                Logger.LogInfo("CardServices.GetActiveCards, Incomplete Card Details: ", item);
                            }
                        }
                    }

                    response = new RetrieveCardResponse
                    {
                        Cards = cards,
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL",
                    };
                }
                else
                {
                    response = new RetrieveCardResponse
                    {
                        //ResponseCode = "EP96",
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new RetrieveCardResponse
                {
                    Cards = null,
                    ResponseCode = "06",
                    ResponseDescription = "Failed to retrieve cards",
                };
            }

            return Task.Factory.StartNew(() => response);
        }
        
        /// <summary>
        /// Deprecated! Do not use!!! 
        /// This is kept here for reference purposes ONLY. It has been re-implemented to call IBS Bridge. 
        /// </summary>
        /// <param name="cardRequest"></param>
        /// <returns></returns>
        public Task<RetrieveCardResponse> GetActiveCards_SOAP(RetrieveCardRequest cardRequest)
        {
            Logger.LogInfo("CardServices.GetActiveCards_SOAP, Input", cardRequest);
            List<Card> cards = new List<Card>();

            RetrieveCardResponse response = null;
            if (isdemo)
            {
                cards.Add(new Card { expiry_date = "1812", pan = "5061104947862517319", seq_nr = "2" });
                response = new RetrieveCardResponse
                {
                    Cards = cards,
                    ResponseCode = "00",
                    ResponseDescription = "SUCCESSFUL",
                };
                Logger.LogInfo("CardServices.GetActiveCards_SOAP, response", response);
                return Task.Factory.StartNew(() => response);
            }

            try
            {
                using (SterlingCardService.CardsSoapClient CardClient = new SterlingCardService.CardsSoapClient())
                {
                    string responseFromService = CardClient.GetActiveCardsByCustomer(cardRequest.CustomerNumber);

                    Logger.LogInfo("CardServices.GetActiveCards_SOAP, response from GetActiveCardsByCustomer service", responseFromService);
                    string[] CardRecords = responseFromService.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in CardRecords)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string[] cardDetails = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            cards.Add(new Card { expiry_date = cardDetails[1], pan = cardDetails[0], seq_nr = cardDetails[2] });
                        }
                    }

                    response = new RetrieveCardResponse
                    {
                        Cards = cards,
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL",
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new RetrieveCardResponse
                {
                    Cards = null,
                    ResponseCode = "06",
                    ResponseDescription = "Failed to retrieve cards",
                };
            }

            return Task.Factory.StartNew(() => response);
        }

        public Task<RetrieveCardResponse> GetInActiveCards(RetrieveCardRequest cardRequest)
        {
            Logger.LogInfo("CardServices.GetInActiveCards, Input", cardRequest);
            List<Card> cards = new List<Card>();

            RetrieveCardResponse response = null;
            if (isdemo)
            {
                cards.Add(new Card { expiry_date = "1812", pan = "5061104947862517319", seq_nr = "2", TheAccount = new Account() { AccountNumber = "1234567890" } });
                response = new RetrieveCardResponse
                {
                    Cards = cards,
                    ResponseCode = "00",
                    ResponseDescription = "SUCCESSFUL",
                };
                Logger.LogInfo("CardServices.GetInActiveCards, response", response);
                return Task.Factory.StartNew(() => response);
            }

            if (string.IsNullOrWhiteSpace(cardRequest.CustomerNumber))
            {
                response = new RetrieveCardResponse
                {
                    Cards = cards,
                    ResponseCode = "MW06",
                    ResponseDescription = "'CustomerNumber' is required.",
                };
                Logger.LogInfo("CardServices.GetInActiveCards, response", response);
                return Task.Factory.StartNew(() => response);
            }

            try
            {
                GetActiveCardsByCustomerIDRequestXML clientRequest = new GetActiveCardsByCustomerIDRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "946",
                    CustomerID = cardRequest.CustomerNumber,
                };

                GetActiveCardsByCustomerIDResponseXML serverResponse = new ServiceUtilities.IBSBridgeProcessor<GetActiveCardsByCustomerIDRequestXML, GetActiveCardsByCustomerIDResponseXML>().Processor(clientRequest, true) as GetActiveCardsByCustomerIDResponseXML;

                if (serverResponse.ResponseCode == "00")
                {
                    if (string.IsNullOrWhiteSpace(serverResponse.ResponseText))
                    {
                        response = new RetrieveCardResponse
                        {
                            Cards = cards,
                            ResponseCode = "06",
                            ResponseDescription = serverResponse.ResponseText,
                        };
                        return Task.Factory.StartNew(() => response);
                    }

                    string[] CardRecords = serverResponse.ResponseText.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in CardRecords)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            string[] cardDetails = item.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            if (cardDetails.Length == 4)
                            {
                                cards.Add(new Card { pan = cardDetails[0], expiry_date = cardDetails[1], seq_nr = cardDetails[2], TheAccount = new Account() { AccountNumber = cardDetails[3] } });
                            }
                            else
                            {
                                Logger.LogInfo("CardServices.GetInActiveCards, Incomplete Card Details: ", item);
                            }
                        }
                    }

                    response = new RetrieveCardResponse
                    {
                        Cards = cards,
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL",
                    };
                }
                else
                {
                    response = new RetrieveCardResponse
                    {
                        //ResponseCode = "EP96",
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new RetrieveCardResponse
                {
                    Cards = null,
                    ResponseCode = "06",
                    ResponseDescription = "Failed to retrieve cards",
                };
            }

            return Task.Factory.StartNew(() => response);
        }

        public Task<ActivateFxTrxResponse> ActivateCountriesForTrnx(ActivateFxTrxRequest Request)
        {
            Logger.LogInfo("CardServices.ActivateCountriesForTrnx, Input", Request);
            ActivateFxTrxResponse response = null;

            if (isdemo)
            {
                response = new ActivateFxTrxResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "Card Activation (for Countries) Succeeded"
                };
                return Task.Factory.StartNew(() => response);
            }
            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(Request, out br);
                if (!tokenValid)
                {
                    // TODO: Uncomment the commented lines below.
                    //response = new ActivateFxTrxResponse
                    //{
                    //    ResponseCode = "01",
                    //    ResponseDescription = br.ResponseDescription,
                    //};
                    //return Task<ActivateFxTrxResponse>.Factory.StartNew(() => response);
                }

                #region Checks and Account Retrieval
                RetrieveCardResponse activeCards = GetActiveCards(new RetrieveCardRequest()
                {
                    AccountNumber = string.Empty,
                    AuthToken = Request.AuthToken,
                    CustomerNumber = Request.CustomerNumber,
                    customer_id = Request.customer_id,
                    ExpDate = string.Empty,
                    isMobile = Request.isMobile,
                    MailRequest = Request.MailRequest,
                    PAN = Request.PAN,
                    Passkey = Request.Passkey,
                    PhoneNumber = Request.PhoneNumber,
                    PIN = Request.PIN,
                    RequestChannel = Request.RequestChannel,
                }).Result;

                if (activeCards.ResponseCode != "00")
                {
                    response = new ActivateFxTrxResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Unable to fetch cards for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                if (activeCards.Cards == null || activeCards.Cards.Count <= 0)
                {
                    response = new ActivateFxTrxResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No card record was found for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                if (!activeCards.Cards.Any(x => string.Equals(x.pan, Request.PAN)))
                {
                    response = new ActivateFxTrxResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No matching card record was found for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                var anAccount = activeCards.Cards.FirstOrDefault(x => string.Equals(x.pan, Request.PAN) && x.TheAccount != null && !string.IsNullOrWhiteSpace(x.TheAccount.AccountNumber));
                if (anAccount == null)
                {
                    response = new ActivateFxTrxResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No matching account was found for this card."
                    };
                    return Task.Factory.StartNew(() => response);
                }
                string accountNumber = anAccount.TheAccount.AccountNumber;

                AccountResponse accountDetails = new AccountInquiryService().ValidateAccountNumber(new AccountRequest()
                {
                    AccountNumber = accountNumber,
                    AuthToken = Request.AuthToken,
                    CustomerNumber = Request.CustomerNumber,
                    customer_id = Request.customer_id,
                    isMobile = Request.isMobile,
                    MailRequest = Request.MailRequest,
                    Passkey = Request.Passkey,
                    PhoneNumber = Request.PhoneNumber,
                    PIN = Request.PIN,
                    RequestChannel = Request.RequestChannel,
                    CustomerID = Request.CustomerNumber,
                }).Result;

                if (accountDetails == null || accountDetails.ResponseCode != "00" || accountDetails.AccountInformation == null)
                {
                    response = new ActivateFxTrxResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Could not validate details of the account linked to this card."
                    };
                    return Task.Factory.StartNew(() => response);
                }
                #endregion

                ActivateFxTrxXML clientRequest = new ActivateFxTrxXML
                {
                    RequestID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "1127",
                    AccountNumber = accountNumber,
                    AccountType = accountDetails.AccountInformation.AccountType,
                    Countries = Request.Countries,
                    Pan = Request.PAN,
                    EndDate = Request.EndDate,
                    StartDate = Request.StartDate
                };

                CardBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<ActivateFxTrxXML, CardBaseResponse>().Processor(clientRequest, true, true, Encoding.UTF8) as CardBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new ActivateFxTrxResponse
                    {
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
                else
                {
                    response = new ActivateFxTrxResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new ActivateFxTrxResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Card Activation (for Countries) Failed.",
                };
            }

            Logger.LogInfo("CardServices.ActivateCountriesForTrnx, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<CardResponse> CreditCardRequest(CreditCardRequest Request)
        {
            Logger.LogInfo("CardServices.CreditCardRequest, Input", Request);

            CardResponse response = null;
            try
            {
                if (isdemo)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Card request successfully logged"
                    };
                    Logger.LogInfo("CardServices.CreditCardRequest, response", response);
                    return Task.Factory.StartNew(() => response);
                }

                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(Request, out br);
                if (!tokenValid)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<CardResponse>.Factory.StartNew(() => response);
                }

                if (string.IsNullOrWhiteSpace(Request.AccountNumber) || Request.AccountNumber.Trim().Length != 10)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = "Account number supplied MUST be a NUBAN number.",
                    };
                    return Task<CardResponse>.Factory.StartNew(() => response);
                }

                Request.AccountNumber = Request.AccountNumber.Trim();
                CreditCardRequestXML clientRequest = new CreditCardRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "935",
                    AccountId = Request.AccountNumber,
                    Title = Request.Title,
                    FirstName = Request.FirstName,
                    LastName = Request.LastName,
                };

                switch (Request.CardType)
                {
                    case CreditCardType.MasterCard:
                        clientRequest.IsMasterCard = "1";
                        clientRequest.IsVerveCard = "0";
                        break;
                    case CreditCardType.VisaCard:
                        clientRequest.IsMasterCard = "0";
                        clientRequest.IsVerveCard = "1";
                        break;
                    default:
                        return Task.Factory.StartNew(() => response = new CardResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "Invalid card type"
                        });
                }

                SterlingBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<CreditCardRequestXML, CardBaseResponse>().Processor(clientRequest, true, true, Encoding.UTF8) as SterlingBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new CardResponse
                    {
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
                else
                {
                    response = new CardResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new CardResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Credit Card Request failed"
                };
            }
            Logger.LogInfo("CardServices.CreditCardRequest, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<CardResponse> DebitCardRequest(CreditCardRequest Request)
        {
            Logger.LogInfo("CardServices.DebitCardRequest, Input", Request);

            CardResponse response = null;
            try
            {
                if (isdemo)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Card request successfully logged"
                    };
                    Logger.LogInfo("CardServices.DebitCardRequest, response", response);
                    return Task.Factory.StartNew(() => response);
                }

                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(Request, out br);
                if (!tokenValid)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<CardResponse>.Factory.StartNew(() => response);
                }

                if (string.IsNullOrWhiteSpace(Request.AccountNumber) || Request.AccountNumber.Trim().Length != 10)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = "Account number supplied MUST be a NUBAN number.",
                    };
                    return Task<CardResponse>.Factory.StartNew(() => response);
                }

                Request.AccountNumber = Request.AccountNumber.Trim();
                CreditCardRequestXML clientRequest = new CreditCardRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "935",
                    AccountId = Request.AccountNumber,
                    Title = Request.Title,
                    FirstName = Request.FirstName,
                    LastName = Request.LastName,
                };

                switch (Request.CardType)
                {
                    case CreditCardType.MasterCard:
                        clientRequest.IsMasterCard = "1";
                        clientRequest.IsVerveCard = "0";
                        break;
                    case CreditCardType.VisaCard:
                        clientRequest.IsMasterCard = "0";
                        clientRequest.IsVerveCard = "1";
                        break;
                    default:
                        return Task.Factory.StartNew(() => response = new CardResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "Invalid card type"
                        });
                }

                SterlingBaseResponse serverResponse = new ServiceUtilities.IBSBridgeProcessor<CreditCardRequestXML, SterlingBaseResponse>().Processor(clientRequest, true) as SterlingBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new CardResponse
                    {
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
                else
                {
                    response = new CardResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new CardResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Debit Card Request failed"
                };
            }
            Logger.LogInfo("CardServices.DebitCardRequest, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<ActivateChannelResponse> ActivateChannels(ActivateChannelRequest Request)
        {
            Logger.LogInfo("CardServices.ActivateChannels, Input", Request);
            ActivateChannelResponse response = null;

            if (isdemo)
            {
                Dictionary<CardChannel, string> dummyResponse = new Dictionary<CardChannel, string>();
                dummyResponse.Add(CardChannel.Atm, "00|successful");

                response = new ActivateChannelResponse
                {
                    ActivationResponse = dummyResponse,
                    ResponseCode = "00",
                    ResponseDescription = "SUCCESSFUL"

                };
                return Task<ActivateChannelResponse>.Factory.StartNew(() => response);
            }

            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(Request, out br);
                if (!tokenValid)
                {
                    response = new ActivateChannelResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<ActivateChannelResponse>.Factory.StartNew(() => response);
                }

                #region Checks and Account Retrieval
                RetrieveCardResponse activeCards = GetActiveCards(new RetrieveCardRequest()
                {
                    AccountNumber = string.Empty,
                    AuthToken = Request.AuthToken,
                    CustomerNumber = Request.CustomerNumber,
                    customer_id = Request.customer_id,
                    ExpDate = string.Empty,
                    isMobile = Request.isMobile,
                    MailRequest = Request.MailRequest,
                    PAN = Request.CardPAN,
                    Passkey = Request.Passkey,
                    PhoneNumber = Request.PhoneNumber,
                    PIN = Request.PIN,
                    RequestChannel = Request.RequestChannel,
                }).Result;

                if (activeCards.ResponseCode != "00")
                {
                    response = new ActivateChannelResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Unable to fetch cards for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                if (activeCards.Cards == null || activeCards.Cards.Count <= 0)
                {
                    response = new ActivateChannelResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No card record was found for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                if (!activeCards.Cards.Any(x => string.Equals(x.pan, Request.CardPAN)))
                {
                    response = new ActivateChannelResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No matching card record was found for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                var anAccount = activeCards.Cards.FirstOrDefault(x => string.Equals(x.pan, Request.CardPAN) && x.TheAccount != null && !string.IsNullOrWhiteSpace(x.TheAccount.AccountNumber));
                if (anAccount == null)
                {
                    response = new ActivateChannelResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No matching account was found for this card."
                    };
                    return Task.Factory.StartNew(() => response);
                }
                string accountNumber = anAccount.TheAccount.AccountNumber;

                AccountResponse accountDetails = new AccountInquiryService().ValidateAccountNumber(new AccountRequest()
                {
                    AccountNumber = accountNumber,
                    AuthToken = Request.AuthToken,
                    CustomerNumber = Request.CustomerNumber,
                    customer_id = Request.customer_id,
                    isMobile = Request.isMobile,
                    MailRequest = Request.MailRequest,
                    Passkey = Request.Passkey,
                    PhoneNumber = Request.PhoneNumber,
                    PIN = Request.PIN,
                    RequestChannel = Request.RequestChannel,
                    CustomerID = Request.CustomerNumber,
                }).Result;

                if(accountDetails == null || accountDetails.ResponseCode != "00" || accountDetails.AccountInformation == null)
                {
                    response = new ActivateChannelResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Could not validate details of the account linked to this card."
                    };
                    return Task.Factory.StartNew(() => response);
                }
                #endregion

                response = RunChannelActivation(Request.ActivationOptions, accountNumber, Request.CardPAN, accountDetails.AccountInformation.AccountType);

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new ActivateChannelResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Card Hotlist failed"
                };
            }

            Logger.LogInfo("CardServices.ActivateChannels, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public ActivateChannelResponse RunChannelActivation(Dictionary<CardChannel, bool> request, string AccountNumber, string PAN, string AcctType)
        {
            Logger.LogInfo("CardServices.RunChannelActivation, Input", request);
            Dictionary<CardChannel, string> response = new Dictionary<CardChannel, string>();
            ActivateChannelResponse service = null;

            try
            {
                foreach (var entry in request)
                {
                    CardActionRequestXML clientRequest = new CardActionRequestXML
                    {
                        ReqeuestID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                        AccountNumber = AccountNumber,
                        Pan = PAN,
                        AccountType = AcctType,
                    };

                    switch (entry.Key)
                    {
                        case CardChannel.Atm:
                            clientRequest.RequestType = entry.Value ? "1118" : "1117";
                            break;
                        case CardChannel.Pos:
                            clientRequest.RequestType = entry.Value ? "1139" : "1141";
                            break;
                        case CardChannel.Web:
                            clientRequest.RequestType = entry.Value ? "1136" : "1140";
                            break;
                        default:
                            break;
                    }

                    CardBaseResponse serverResponse = new IBSBridgeProcessor<CardActionRequestXML, CardBaseResponse>().Processor(clientRequest, true, true, Encoding.UTF8) as CardBaseResponse;
                    if (serverResponse.ResponseCode == "00")
                    {
                        response.Add(entry.Key, serverResponse.ResponseCode + "|" + serverResponse.ResponseText);
                    }
                }
                service = new ActivateChannelResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "Request Completed",
                    ActivationResponse = response
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                service = new ActivateChannelResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "unable to complete request",
                    ActivationResponse = response
                };
            }
            return service;
        }

        public Task<CardResponse> ActivateForeignTransactions(CardActionRequest Request)
        {
            Logger.LogInfo("CardServices.ActivateForeignTransactions, Input", Request);

            CardResponse response = null;
            try
            {
                if (isdemo)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Activation of Foreign Transactions on Card was successful."
                    };
                    Logger.LogInfo("CardServices.ActivateForeignTransactions, response", response);
                    return Task.Factory.StartNew(() => response);
                }

                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(Request, out br);
                if (!tokenValid)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = br.ResponseDescription,
                    };
                    return Task<CardResponse>.Factory.StartNew(() => response);
                }

                #region Checks and Account Retrieval
                RetrieveCardResponse activeCards = GetActiveCards(new RetrieveCardRequest()
                {
                    AccountNumber = string.Empty,
                    AuthToken = Request.AuthToken,
                    CustomerNumber = Request.CustomerNumber,
                    customer_id = Request.customer_id,
                    ExpDate = string.Empty,
                    isMobile = Request.isMobile,
                    MailRequest = Request.MailRequest,
                    PAN = Request.CardPAN,
                    Passkey = Request.Passkey,
                    PhoneNumber = Request.PhoneNumber,
                    PIN = Request.PIN,
                    RequestChannel = Request.RequestChannel,
                }).Result;

                if (activeCards.ResponseCode != "00")
                {
                    response = new CardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Unable to fetch cards for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                if (activeCards.Cards == null || activeCards.Cards.Count <= 0)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No card record was found for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                if (!activeCards.Cards.Any(x => string.Equals(x.pan, Request.CardPAN)))
                {
                    response = new CardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No matching card record was found for this customer."
                    };
                    return Task.Factory.StartNew(() => response);
                }

                var anAccount = activeCards.Cards.FirstOrDefault(x => string.Equals(x.pan, Request.CardPAN) && x.TheAccount != null && !string.IsNullOrWhiteSpace(x.TheAccount.AccountNumber));
                if (anAccount == null)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No matching account was found for this card."
                    };
                    return Task.Factory.StartNew(() => response);
                }
                string accountNumber = anAccount.TheAccount.AccountNumber;

                AccountResponse accountDetails = new AccountInquiryService().ValidateAccountNumber(new AccountRequest()
                {
                    AccountNumber = accountNumber,
                    AuthToken = Request.AuthToken,
                    CustomerNumber = Request.CustomerNumber,
                    customer_id = Request.customer_id,
                    isMobile = Request.isMobile,
                    MailRequest = Request.MailRequest,
                    Passkey = Request.Passkey,
                    PhoneNumber = Request.PhoneNumber,
                    PIN = Request.PIN,
                    RequestChannel = Request.RequestChannel,
                    CustomerID = Request.CustomerNumber,
                }).Result;

                if (accountDetails == null || accountDetails.ResponseCode != "00" || accountDetails.AccountInformation == null)
                {
                    response = new CardResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Could not validate details of the account linked to this card."
                    };
                    return Task.Factory.StartNew(() => response);
                }
                #endregion

                CardActionRequestXML clientRequest = new CardActionRequestXML
                {
                    ReqeuestID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = Request.CardAction == CardAction.ActivateForeignTrnx ? "1126" : "1125",
                    AccountNumber = accountNumber,
                    Pan = Request.CardPAN,
                    AccountType = accountDetails.AccountInformation.AccountType,
                };

                CardBaseResponse serverResponse = new IBSBridgeProcessor<CardActionRequestXML, CardBaseResponse>().Processor(clientRequest, true, true, Encoding.UTF8) as CardBaseResponse;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new CardResponse
                    {
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = serverResponse.ResponseText
                    };
                }
                else
                {
                    response = new CardResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText

                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new CardResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Activation of Foreign Transactions on Card failed.",
                };
            }
            Logger.LogInfo("CardServices.ActivateForeignTransactions, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<AllowedCountriesOnPostilionResponse> AllowedCountriesOnPostilion(AllowedCountriesOnPostilionRequest Request)
        {
            Logger.LogInfo("CardServices.AllowedCountriesOnPostilion, Input", Request);

            AllowedCountriesOnPostilionResponse response = null;
            try
            {
                if (isdemo)
                {
                    response = new AllowedCountriesOnPostilionResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Succesfully retrieved the demo data.",
                        AllowedCountries = new List<AllowedCountryOnPostillion>() {
                            new AllowedCountryOnPostillion() { AlphaCode="NGN", CountryName="Nigeria", CurrencyCode="566", ISO2Code="NG", ISO3Code="NGA" },
                            new AllowedCountryOnPostillion() { AlphaCode="USD", CountryName="United States", CurrencyCode="840", ISO2Code="US", ISO3Code="USA"  } }.ToArray(),
                    };
                    Logger.LogInfo("CardServices.AllowedCountriesOnPostilion, response", response);
                    return Task.Factory.StartNew(() => response);
                }

                AllowedCountriesOnPostilionRequestXML clientRequest = new AllowedCountriesOnPostilionRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "943",
                };

                AllowedCountriesOnPostilionResponseXML serverResponse = new IBSBridgeProcessor<AllowedCountriesOnPostilionRequestXML, AllowedCountriesOnPostilionResponseXML>().Processor(clientRequest, true) as AllowedCountriesOnPostilionResponseXML;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new AllowedCountriesOnPostilionResponse
                    {
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = serverResponse.ResponseText
                    };

                    if (serverResponse.AllowedCountries != null && serverResponse.AllowedCountries.Count() > 0)
                    {
                        List<AllowedCountryOnPostillion> TheAllowedCountries = new List<AllowedCountryOnPostillion>();
                        foreach (var item in serverResponse.AllowedCountries)
                        {
                            TheAllowedCountries.Add(new AllowedCountryOnPostillion()
                            {
                                AlphaCode = item.alpha_code.Trim(),
                                CountryName = item.Country_Name.Trim(),
                                CurrencyCode = item.currency_code.Trim(),
                                ISO2Code = item.ISO_2_code.Trim(),
                                ISO3Code = item.ISO_3_Code.Trim(),
                            });
                        }
                        response.AllowedCountries = TheAllowedCountries.ToArray();
                    }
                    else
                    {
                        response.ResponseCode = "06";
                        response.ResponseDescription = "No matching country data was found.";
                    }
                }
                else
                {
                    response = new AllowedCountriesOnPostilionResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText,
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new AllowedCountriesOnPostilionResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Fetching Allowed Countries On Postilion failed"
                };
            }
            Logger.LogInfo("CardServices.AllowedCountriesOnPostilion, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<CreateAndActivateVirtualCardResponse> CreateAndActivateVirtualCard(CreateAndActivateVirtualCardRequest Request)
        {
            Logger.LogInfo("CardServices.CreateAndActivateVirtualCard, Input", Request);

            CreateAndActivateVirtualCardResponse response = null;
            try
            {
                if (isdemo)
                {
                    response = new CreateAndActivateVirtualCardResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Succesfully created and activated Virtual Card.",
                        PAN = "1234567890123456",
                        CVV = "123",
                        ExpiryDate = "9912",
                        SequenceNumber = "001"
                    };
                    Logger.LogInfo("CardServices.CreateAndActivateVirtualCard, response", response);
                    return Task.Factory.StartNew(() => response);
                }

                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(Request, out br);
                if (!tokenValid)
                {
                    // TODO: Uncomment the commented lines below.
                    //response = new CreateAndActivateVirtualCardResponse
                    //{
                    //    ResponseCode = "01",
                    //    ResponseDescription = br.ResponseDescription,
                    //};
                    //return Task<CreateAndActivateVirtualCardResponse>.Factory.StartNew(() => response);
                }

                DateTime dob = DateTime.Now;
                if(!DateTime.TryParse(Request.DateOfBirth, out dob))
                {
                    response = new CreateAndActivateVirtualCardResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = $"Invalid 'Date of Birth' value was supplied.",
                    };
                    return Task<CreateAndActivateVirtualCardResponse>.Factory.StartNew(() => response);
                }

                CreateAndActivateVirtualCardRequestXML clientRequest = new CreateAndActivateVirtualCardRequestXML
                {
                    ReferenceID = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "945",
                    AppCode = "2",
                    AddressHome = Request.AddressHome,
                    bvn = Request.BVN,
                    cusnum = Request.CustomerNumber,
                    DateOfBirth = dob.ToString("dd-MMM-yy").ToUpperInvariant(),
                    email = Request.Email,
                    FirstName = Request.FirstName,
                    Gender = Request.Gender,
                    LastName = Request.LastName,
                    MiddleName = Request.MiddleName,
                    mobile = Request.PhoneNumber,
                    Title = Request.Title,
                };

                CreateAndActivateVirtualCardResponseXML serverResponse = new IBSBridgeProcessor<CreateAndActivateVirtualCardRequestXML, CreateAndActivateVirtualCardResponseXML>().Processor(clientRequest, true) as CreateAndActivateVirtualCardResponseXML;

                if (serverResponse.ResponseCode == "00")
                {
                    response = new CreateAndActivateVirtualCardResponse
                    {
                        ResponseCode = serverResponse.ResponseCode,
                        ResponseDescription = "Succesfully created and activated Virtual Card.",
                    };

                    if (!string.IsNullOrWhiteSpace(serverResponse.ResponseText))
                    {
                        // Format: sql.pan + "|" + sql.expiry_date + "|" + sql.seq_nr + "|" + retvcc + "|" + postal_address_1 + "|" + postal_address_2 + "|" + postal_city + "|" + postal_region + "|" + postal_code + "|" + postal_country +"|"+ ResponseMessage
                        string[] cardDetails = serverResponse.ResponseText.Trim().Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (cardDetails.Length == 11)
                        {
                            response.PAN = cardDetails[0];
                            response.ExpiryDate = cardDetails[1];
                            response.SequenceNumber = cardDetails[2];
                            response.CVV = cardDetails[3];
                            response.PostalAddress1 = cardDetails[4];
                            response.PostalAddress2 = cardDetails[5];
                            response.PostalCity = cardDetails[6];
                            response.PostalRegion = cardDetails[7];
                            response.PostalCode = cardDetails[8];
                            response.PostalCountry = cardDetails[9];
                            response.LinkedAccountNumber = cardDetails[10];
                        }
                        else
                        {
                            response.ResponseCode = "06";
                            response.ResponseDescription = "Incomplete Card details was returned.";
                        }
                    }
                    else
                    {
                        response.ResponseCode = "06";
                        response.ResponseDescription = "Virtual Card creation/activation failed.";
                    }
                }
                else
                {
                    response = new CreateAndActivateVirtualCardResponse
                    {
                        ResponseCode = "EP96",
                        ResponseDescription = serverResponse.ResponseText,
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new CreateAndActivateVirtualCardResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Creation/Activation of Virtual Card failed"
                };
            }
            Logger.LogInfo("CardServices.CreateAndActivateVirtualCard, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<ActivateChannelResponse> GetActiveChannels(ActivateChannelRequest Request)
        {
            Logger.LogInfo("CardServices.GetActiveChannels, Input", Request);
            ActivateChannelResponse response = null;

            if (isdemo)
            {
                Dictionary<CardChannel, string> dummyResponse = new Dictionary<CardChannel, string>();
                dummyResponse.Add(CardChannel.Atm, "00|successful");

                response = new ActivateChannelResponse
                {
                    ActivationResponse = dummyResponse,
                    ResponseCode = "00",
                    ResponseDescription = "SUCCESSFUL"

                };
                return Task<ActivateChannelResponse>.Factory.StartNew(() => response);
            }

            if(string.IsNullOrWhiteSpace(Request.CardPAN))
            {
                response = new ActivateChannelResponse
                {
                    ActivationResponse = null,
                    ResponseCode = "MW06",
                    ResponseDescription = "'CardPAN' was not supplied."
                };
                return Task<ActivateChannelResponse>.Factory.StartNew(() => response);
            }

            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(Request, out br);
                if (!tokenValid)
                {
                    // TODO: Uncomment the commented lines below.
                    //response = new ActivateChannelResponse
                    //{
                    //    ResponseCode = "06",
                    //    ResponseDescription = br.ResponseDescription,
                    //};
                    //return Task<ActivateChannelResponse>.Factory.StartNew(() => response);
                }

                string resp = string.Empty;
                using (IBCardService.CardServClient client = new IBCardService.CardServClient())
                {
                    resp = client.GetActiveChannels(Request.CardPAN);
                }

                if (string.IsNullOrWhiteSpace(resp))
                {
                    response = new ActivateChannelResponse
                    {
                        ResponseCode = "EP06",
                        ResponseDescription = "No response was received.",
                    };
                }
                else
                {
                    response = new ActivateChannelResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Successful!",
                    };

                    // Exmple: "PAN:1234567890123456|POS:True|ATM:True|Web :True"
                    Dictionary<CardChannel, string> activeChannels = new Dictionary<CardChannel, string>();
                    string[] respDetails = resp.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (respDetails != null && respDetails.Count() > 0)
                    {
                        foreach (var splitPipe in respDetails)
                        {
                            string[] channelsSplit = splitPipe.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                            if (channelsSplit != null && channelsSplit.Count() == 2)
                            {
                                switch (channelsSplit[0].Trim().ToUpperInvariant())
                                {
                                    case "POS":
                                        activeChannels.Add(CardChannel.Pos, Convert.ToBoolean(channelsSplit[1]).ToString());
                                        break;
                                    case "ATM":
                                        activeChannels.Add(CardChannel.Atm, Convert.ToBoolean(channelsSplit[1]).ToString());
                                        break;
                                    case "WEB":
                                        activeChannels.Add(CardChannel.Web, Convert.ToBoolean(channelsSplit[1]).ToString());
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    response.ActivationResponse = activeChannels;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new ActivateChannelResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "GetActiveChannels failed"
                };
            }

            Logger.LogInfo("CardServices.GetActiveChannels, response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<ActivateFxTrxResponse> GetActiveCountries(ActivateFxTrxRequest Request)
        {
            Logger.LogInfo("CardServices.GetActiveCountries, Input", Request);
            ActivateFxTrxResponse response = null;

            if (isdemo)
            {
                response = new ActivateFxTrxResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "GetActiveCountries) Succeeded"
                };
                return Task.Factory.StartNew(() => response);
            }

            if (string.IsNullOrWhiteSpace(Request.PAN))
            {
                response = new ActivateFxTrxResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "'PAN' was not supplied."
                };
                return Task<ActivateFxTrxResponse>.Factory.StartNew(() => response);
            }

            try
            {
                BaseResponse br = null;
                bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(Request, out br);
                if (!tokenValid)
                {
                    // TODO: Uncomment the commented lines below.
                    //response = new ActivateFxTrxResponse
                    //{
                    //    ResponseCode = "01",
                    //    ResponseDescription = br.ResponseDescription,
                    //};
                    //return Task<ActivateFxTrxResponse>.Factory.StartNew(() => response);
                }

                string resp = string.Empty;
                using (IBCardService.CardServClient client = new IBCardService.CardServClient())
                {
                    resp = client.GetActiveCountries(Request.PAN);
                }

                if (string.IsNullOrWhiteSpace(resp))
                {
                    response = new ActivateFxTrxResponse
                    {
                        ResponseCode = "EP06",
                        ResponseDescription = "No response was received.",
                    };
                }
                else
                {
                    response = new ActivateFxTrxResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Successful!",
                    };

                    // TODO: Populate the Active Countries here
                    response.ResponseDescription = resp;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new ActivateFxTrxResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Card Activation (for Countries) Failed.",
                };
            }

            Logger.LogInfo("CardServices.GetActiveCountries, response", response);
            return Task.Factory.StartNew(() => response);
        }
    }
}
