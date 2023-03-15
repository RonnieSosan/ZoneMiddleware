using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using Blend.SterlingImplementation.ServiceUtilities;
using AppZoneMiddleware.Shared.Utility;
using Blend.SterlingImplementation.Entites;
using System.Xml.Serialization;
using System.IO;

namespace Blend.SterlingImplementation.Payments
{
    public class PaymentServices : IPaymentServices
    {
        private static bool runMPassRegisterUserInDemoMode = false;
        private static string _MPassRegisterUserAPI = string.Empty;
        int webServiceAppId = -1;
        public string spayUrl = string.Empty;

        public PaymentServices()
        {
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("MPassRegisterUserInDemoMode"), out runMPassRegisterUserInDemoMode);

            _MPassRegisterUserAPI = System.Configuration.ConfigurationManager.AppSettings.Get("MPassRegisterUserAPI");
            if (string.IsNullOrWhiteSpace(_MPassRegisterUserAPI))
                throw new ApplicationException("Value for 'MPassRegisterUserAPI' hasn't been configured in the AppSettings Section of your app/web config file.");
            
            if (string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings["WebServiceAppId"]))
                throw new ApplicationException("Value for 'WebServiceAppId' hasn't been configured in the AppSettings Section of your app/web config file.");

            Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["WebServiceAppId"], out webServiceAppId);
            if(webServiceAppId < 0)
                throw new ApplicationException("Value for conf 'WebServiceAppId' is invalid.");

            spayUrl = System.Configuration.ConfigurationManager.AppSettings.Get("SterlingSpayURL");
            if (string.IsNullOrWhiteSpace(spayUrl))
                throw new ApplicationException("Value for 'SterlingSpayURL' hasn't been configured in the AppSettings Section of your app/web config file.");
        }

        public Task<MPassRegisterUserResponse> MpassRegisterUser(MPassRegisterUserRequest req)
        {
            MPassRegisterUserResponse retVal = new MPassRegisterUserResponse();

            if (runMPassRegisterUserInDemoMode)
            {
                retVal.ResponseCode = "00";
                retVal.ResponseDescription = "Successfully registered!";
            }
            else
            {
                MPassRegisterUserRequestJSON apiRequest = new MPassRegisterUserRequestJSON()
                {
                    Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "108",    // TODO: Confirm this from the Sterling Team
                    Translocation = req.Translocation,
                    BusinessName = req.BusinessName,
                    BVN = req.BVN,
                    MccCode = req.MccCode,
                    NUBAN = req.NUBAN,
                    Password = req.Password,
                };
                MPassRegisterUserResponseJSON apiResponse = new RESTProcessor<MPassRegisterUserRequestJSON, MPassRegisterUserResponseJSON>(_MPassRegisterUserAPI).DoPOST(apiRequest, true, false) as MPassRegisterUserResponseJSON;

                retVal.ResponseCode = "00";
                retVal.ResponseDescription = "Successful";
            }

            return Task.Factory.StartNew(() => retVal);
        }

        public Task<GetQuickTellerBillersResponse> GetQuickTellerBillers(GetQuickTellerBillersRequest req)
        {
            Logger.LogInfo("PaymentServices.GetQuickTellerBillers, Input", req);
            GetQuickTellerBillersResponse response = new GetQuickTellerBillersResponse() { ResponseCode = "MW06", ResponseDescription = "System Hang!" };

            BaseResponse br = null;
            bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(req, out br);
            if (!tokenValid)
            {
                // TODO: Uncomment the commented lines below.
                //response = new GetQuickTellerBillersResponse
                //{
                //    ResponseCode = "01",
                //    ResponseDescription = br.ResponseDescription
                //};
                //return Task<GetQuickTellerBillersResponse>.Factory.StartNew(() => response);
            }

            try
            {
                spayUrl += "GetBillersISWRequest";

                GetQuickTellerBillersRequestJSON apiRequest = new GetQuickTellerBillersRequestJSON()
                {
                    Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "108",    // TODO: Confirm this from the Sterling Team
                    Translocation = req.Translocation,
                };

                GetQuickTellerBillersResponseJSON apiResponse = new RESTProcessor<GetQuickTellerBillersRequestJSON, GetQuickTellerBillersResponseJSON>(spayUrl).DoPOST(apiRequest, true, false) as GetQuickTellerBillersResponseJSON;
                Logger.LogInfo("PaymentServices.GetQuickTellerBillers, DoPOST Response => ", apiResponse);

                int respCode = -1;
                if (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.response) && Int32.TryParse(apiResponse.response.Trim(), out respCode) && respCode == 0)
                {
                    response = new GetQuickTellerBillersResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Success!",
                        BillersData = apiResponse.data,
                    };
                }
                else
                {
                    response = new GetQuickTellerBillersResponse
                    {
                        ResponseCode = "EP06",
                        ResponseDescription = (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.message) ? apiResponse.message : "Unknown Failure Reason!"),
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("In PaymentServicesGetQuickTellerBillers ", ex));
                response = new GetQuickTellerBillersResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Fetching QuickTeller Billers failed.",
                };
            }

            Logger.LogInfo("PaymentServices.GetQuickTellerBillers, Response", response);
            return Task.Factory.StartNew(() => response);
        }
        
        public Task<GetQuickTellerBillerItemsResponse> GetQuickTellerBillerItems(GetQuickTellerBillerItemsRequest req)
        {
            Logger.LogInfo("PaymentServices.GetQuickTellerBillerItems, Input", req);
            GetQuickTellerBillerItemsResponse response = new GetQuickTellerBillerItemsResponse() { ResponseCode = "MW06", ResponseDescription = "System Hang!" };

            BaseResponse br = null;
            bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(req, out br);
            if (!tokenValid)
            {
                // TODO: Uncomment the commented lines below.
                //response = new GetQuickTellerBillerItemsResponse
                //{
                //    ResponseCode = "01",
                //    ResponseDescription = br.ResponseDescription
                //};
                //return Task<GetQuickTellerBillerItemsResponse>.Factory.StartNew(() => response);
            }

            try
            {
                spayUrl += "GetBillerPmtItemsRequest";

                GetQuickTellerBillerItemsRequestJSON apiRequest = new GetQuickTellerBillerItemsRequestJSON()
                {
                    Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "108",    // TODO: Confirm this from the Sterling Team
                    Translocation = req.Translocation,
                    billerid = req.BillerID,
                };

                GetQuickTellerBillerItemsResponseJSON apiResponse = new RESTProcessor<GetQuickTellerBillerItemsRequestJSON, GetQuickTellerBillerItemsResponseJSON>(spayUrl).DoPOST(apiRequest, true, false) as GetQuickTellerBillerItemsResponseJSON;
                Logger.LogInfo("PaymentServices.GetQuickTellerBillerItems, DoPOST Response => ", apiResponse);

                int respCode = -1;
                if (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.response) && Int32.TryParse(apiResponse.response.Trim(), out respCode) && respCode == 0)
                {
                    response = new GetQuickTellerBillerItemsResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Success!",
                        BillerItemsData = null,
                    };

                    if(apiResponse.data != null)
                    {
                        response.BillerItemsData = new GetQuickTellerBillerItemsResponseData() { Billers = apiResponse.data.billers, Status = apiResponse.data.status };
                    }
                }
                else
                {
                    response = new GetQuickTellerBillerItemsResponse
                    {
                        ResponseCode = "EP06",
                        ResponseDescription = (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.message) ? apiResponse.message : "Unknown Failure Reason!"),
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("In PaymentServices.GetQuickTellerBillerItems", ex));
                response = new GetQuickTellerBillerItemsResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Fetching QuickTeller Biller Items failed.",
                };
            }

            Logger.LogInfo("PaymentServices.GetQuickTellerBillerItems, Response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<QuickTellerBillsPaymentAdviceResponse> InitQuickTellerBillsPaymentAdvice(QuickTellerBillsPaymentAdviceRequest req)
        {
            Logger.LogInfo("PaymentServices.InitQuickTellerBillsPaymentAdvice, Input", req);
            QuickTellerBillsPaymentAdviceResponse response = new QuickTellerBillsPaymentAdviceResponse() { ResponseCode = "MW06", ResponseDescription = "System Hang!" };

            BaseResponse br = null;
            bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(req, out br);
            if (!tokenValid)
            {
                // TODO: Uncomment the commented lines below.
                //response = new QuickTellerBillsPaymentAdviceResponse
                //{
                //    ResponseCode = "01",
                //    ResponseDescription = br.ResponseDescription
                //};
                //return Task<QuickTellerBillsPaymentAdviceResponse>.Factory.StartNew(() => response);
            }

            try
            {
                spayUrl += "BillPaymtAdviceRequestISW";

                QuickTellerBillsPaymentAdviceRequestJSON apiRequest = new QuickTellerBillsPaymentAdviceRequestJSON()
                {
                    Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "108",    // TODO: Confirm this from the Sterling Team
                    Translocation = req.Translocation,
                    ActionType = req.ActionType,
                    amt = req.Amount,
                    email = req.Email,
                    mobile = req.PhoneNumber,
                    nuban = req.NUBAN,
                    paymentcode = req.PaymentCode,
                    SubscriberInfo1 = req.SubscriberInfo1,
                };

                QuickTellerBillsPaymentAdviceResponseJSON apiResponse = new RESTProcessor<QuickTellerBillsPaymentAdviceRequestJSON, QuickTellerBillsPaymentAdviceResponseJSON>(spayUrl).DoPOST(apiRequest, true, false) as QuickTellerBillsPaymentAdviceResponseJSON;
                Logger.LogInfo("PaymentServices.InitQuickTellerBillsPaymentAdvice, DoPOST Response => ", apiResponse);

                int respCode = -1;
                if (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.response) && Int32.TryParse(apiResponse.response.Trim(), out respCode) && respCode == 0)
                {
                    response = new QuickTellerBillsPaymentAdviceResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Success!",
                        ResponseDetails = null,
                    };

                    if (apiResponse.data != null)
                    {
                        response.ResponseDetails = new QuickTellerBillsPaymentAdviceResponseData() { BillerResponse = apiResponse.data.billerResponse, Status = apiResponse.data.status };
                    }
                }
                else
                {
                    response = new QuickTellerBillsPaymentAdviceResponse
                    {
                        ResponseCode = "EP06",
                        ResponseDescription = (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.message) ? apiResponse.message : "Unknown Failure Reason!"),
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("In PaymentServices.InitQuickTellerBillsPaymentAdvice", ex));
                response = new QuickTellerBillsPaymentAdviceResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Fetching QuickTeller Biller Items failed.",
                };
            }

            Logger.LogInfo("PaymentServices.InitQuickTellerBillsPaymentAdvice, Response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<QuickTellerCustomerValidationResponse> QuickTellerCustomerValidation(QuickTellerCustomerValidationRequest req)
        {
            Logger.LogInfo("PaymentServices.QuickTellerCustomerValidation, Input", req);
            QuickTellerCustomerValidationResponse response = new QuickTellerCustomerValidationResponse() { ResponseCode = "MW06", ResponseDescription = "System Hang!" };

            BaseResponse br = null;
            bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(req, out br);
            if (!tokenValid)
            {
                // TODO: Uncomment the commented lines below.
                //response = new QuickTellerCustomerValidationResponse
                //{
                //    ResponseCode = "01",
                //    ResponseDescription = br.ResponseDescription
                //};
                //return Task<QuickTellerCustomerValidationResponse>.Factory.StartNew(() => response);
            }

            try
            {
                spayUrl += "ISWCustomerValidationRequest";

                QuickTellerCustomerValidationRequestJSON apiRequest = new QuickTellerCustomerValidationRequestJSON()
                {
                    Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "108",   // TODO: Changed from 1019 to 108, due to the error: 
                    Translocation = req.Translocation,
                    CustomerId = req.BillerCustomerId,
                    PaymentCode = req.BillerPaymentCode,
                };

                QuickTellerCustomerValidationResponseJSON apiResponse = new RESTProcessor<QuickTellerCustomerValidationRequestJSON, QuickTellerCustomerValidationResponseJSON>(spayUrl).DoPOST(apiRequest, true, false) as QuickTellerCustomerValidationResponseJSON;
                Logger.LogInfo("PaymentServices.QuickTellerCustomerValidation, DoPOST Response => ", apiResponse);

                int respCode = -1;
                if (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.response) && Int32.TryParse(apiResponse.response.Trim(), out respCode) && respCode == 0)
                {
                    response = new QuickTellerCustomerValidationResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Success!",
                        CustomerDetails = null,
                    };

                    if (apiResponse.data != null && !string.IsNullOrWhiteSpace(apiResponse.data.customerDetails))
                    {
                        try
                        {
                            var apiResponseDeserializer = new XmlSerializer(typeof(QuickTellerCustomerValidationResponseDetails));
                            QuickTellerCustomerValidationResponseDetails deserialized = default(QuickTellerCustomerValidationResponseDetails);
                            using (var reader = new StringReader(apiResponse.data.customerDetails))
                            {
                                deserialized = apiResponseDeserializer.Deserialize(reader) as QuickTellerCustomerValidationResponseDetails;
                            }

                            if(deserialized != null && deserialized.Customer != null)
                            {
                                apiResponse.data.Details = deserialized;
                                response.QuickTellerResponseStatus = apiResponse.data.status;
                                response.QuickTellerResponseCode = deserialized.ResponseCode;
                                response.CustomerDetails = new QuickTellerCustomerValidationDetails()
                                {
                                    Amount = deserialized.Customer.Amount,
                                    CustomerId = deserialized.Customer.CustomerId,
                                    CustomerValidationField = deserialized.Customer.CustomerValidationField,
                                    FullName = deserialized.Customer.FullName,
                                    PaymentCode = deserialized.Customer.PaymentCode,
                                    ResponseCode = deserialized.Customer.ResponseCode,
                                    WithDetails = deserialized.Customer.WithDetails,
                                };
                            }
                            else
                            {
                                response = new QuickTellerCustomerValidationResponse
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "Customer details could not be deciphered.",
                                    CustomerDetails = null,
                                    QuickTellerResponseStatus = apiResponse.data.status,
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(new Exception($"In PaymentServices.QuickTellerCustomerValidation. Could not deserialize contents of customerDetails field. customerDetails = [{apiResponse.data.customerDetails}]", ex));
                            response = new QuickTellerCustomerValidationResponse
                            {
                                ResponseCode = "06",
                                ResponseDescription = "Error occurred while deciphering customer details.",
                                CustomerDetails = null,
                                QuickTellerResponseStatus = apiResponse.data.status,
                            };
                        }
                    }
                    else
                    {
                        response = new QuickTellerCustomerValidationResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "Customer details was not found!",
                            CustomerDetails = null,
                        };
                    }
                }
                else
                {
                    response = new QuickTellerCustomerValidationResponse
                    {
                        ResponseCode = "EP06",
                        ResponseDescription = (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.message) ? apiResponse.message : "Unknown Failure Reason!"),
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("In PaymentServices.QuickTellerCustomerValidation", ex));
                response = new QuickTellerCustomerValidationResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Fetching QuickTeller Biller Items failed.",
                };
            }

            Logger.LogInfo("PaymentServices.QuickTellerCustomerValidation, Response", response);
            return Task.Factory.StartNew(() => response);
        }

        public Task<MPassPaymentResponse> MakeMPassPayment(MPassPaymentRequest req)
        {
            Logger.LogInfo("PaymentServices.MakeMPassPayment, Input", req);
            MPassPaymentResponse response = new MPassPaymentResponse() { ResponseCode = "MW06", ResponseDescription = "System Hang!" };

            BaseResponse br = null;
            bool tokenValid = new IBSBridgeProcessor<BaseRequest, UserProfileResponse>().ValidateTokenFromExternalSource(req, out br);
            if (!tokenValid)
            {
                // TODO: Uncomment the commented lines below.
                //response = new MPassPaymentResponse
                //{
                //    ResponseCode = "01",
                //    ResponseDescription = br.ResponseDescription
                //};
                //return Task<MPassPaymentResponse>.Factory.StartNew(() => response);
            }

            try
            {
                spayUrl += "MoneySendRqt";

                MPassPaymentRequestJSON apiRequest = new MPassPaymentRequestJSON()
                {
                    Referenceid = string.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssffffff")),
                    RequestType = "1024",
                    Translocation = req.Translocation,
                };

                MPassPaymentResponseJSON apiResponse = new RESTProcessor<MPassPaymentRequestJSON, MPassPaymentResponseJSON>(spayUrl).DoPOST(apiRequest, true, false) as MPassPaymentResponseJSON;
                Logger.LogInfo("PaymentServices.MakeMPassPayment, DoPOST Response => ", apiResponse);

                int respCode = -1;
                if (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.response) && Int32.TryParse(apiResponse.response.Trim(), out respCode) && respCode == 0)
                {
                    response = new MPassPaymentResponse
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Success!",
                        PaymentResponseDetails = null,
                    };

                    if (apiResponse.data != null)
                    {
                        response.PaymentResponseDetails = new MPassPaymentResponseDetails()
                        {
                            NetworkReferenceNumber = apiResponse.data.NetworkReferenceNumber,
                            RequestID = apiResponse.data.RequestId,
                            ResponseCode = apiResponse.data.ResponseCode,
                            ResponseDescription = apiResponse.data.Response_Description,
                            SettlementDate = apiResponse.data.SettlementDate,
                            Status = apiResponse.data.status,
                            SubmitDateTime = apiResponse.data.SubmitDateTime,
                            SystemTraceAuditNumber = apiResponse.data.SystemTraceAuditNumber,
                            TransactionReference = apiResponse.data.TransactionReference,
                            TransferType = apiResponse.data.TransferType,
                        };
                    }
                }
                else
                {
                    response = new MPassPaymentResponse
                    {
                        ResponseCode = "EP06",
                        ResponseDescription = (apiResponse != null && !string.IsNullOrWhiteSpace(apiResponse.message) ? apiResponse.message : "Unknown Failure Reason!"),
                        PaymentResponseDetails = null,
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("In PaymentServices.MakeMPassPayment", ex));
                response = new MPassPaymentResponse
                {
                    ResponseCode = "MW06",
                    ResponseDescription = "Fetching QuickTeller Biller Items failed.",
                    PaymentResponseDetails = null,
                };
            }

            Logger.LogInfo("PaymentServices.MakeMPassPayment, Response", response);
            return Task.Factory.StartNew(() => response);
        }
    }
}
