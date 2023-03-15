using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using System.Collections.Concurrent;
using AppZoneMiddleware.Shared.Utility;
using System.Net;
using System.Net.Http;
using AppZoneMiddleware.Shared.Extension;

namespace Blend.SharedServiceImplementation.Services
{
    public class BillPayment : IBillPayment
    {
        private ConcurrentDictionary<string, List<Paymentitem>> _billersPaymentItemConcurrentDict = new ConcurrentDictionary<string, List<Paymentitem>>();

        private static readonly string TerminalID = System.Configuration.ConfigurationManager.AppSettings.Get("InterswithTerminalID");
        private static readonly string BankPrefix = System.Configuration.ConfigurationManager.AppSettings.Get("BankPrefix");
        public bool isDemo = false;
        IBankTransfer _BankTransfer;


        public BillPayment(IBankTransfer bankTransfer)
        {
            _BankTransfer = bankTransfer;
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["isDemo"], out isDemo);
        }

        /// <summary>
        /// Retrieve quickteller biller categories through quickteller API
        /// </summary>
        /// <returns>QuicktellerBillerCategories</returns>
        public QuicktellerBillerCategories GetQuicktellerCategories()
        {
            Logger.LogInfo("BillsPaymentService.GetQuciktelleCategories", "");

            QuicktellerBillerCategories interswitchResponse = new QuicktellerBillerCategories();
            //List<BillerCategory> billerCategory = new List<BillerCategory>();

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                            | SecurityProtocolType.Tls11
                            | SecurityProtocolType.Tls12
                            | SecurityProtocolType.Ssl3;

                var baseAddress = string.Format("{0}/api/v2/quickteller/categorys", System.Configuration.ConfigurationManager.AppSettings["InterswitchURL"]);


                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("GET"), new Uri(baseAddress)))
                    {
                        client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BillPaymentTimeout"]));

                        //set bank terminal ID as a header
                        client.DefaultRequestHeaders.Add("TerminalId", TerminalID);
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        InterswitchAuthenticationService.SetHeaders(client, baseAddress, "GET", "clientID", "secretKey");

                        client.Timeout = TimeSpan.FromSeconds(30);
                        var responseTask = client.SendAsync(request);
                        responseTask.Wait();
                        response = responseTask.Result;
                    }
                }
                //httpPostResponse.Wait();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    interswitchResponse = response.Content.ReadAsAsync<QuicktellerBillerCategories>().Result;
                    interswitchResponse.ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.Successful);
                    interswitchResponse.ResponseDescription = "SUCCESSFUL";
                }
                else
                {
                    InterswitchBaseResponse interswitchFailedResponse = response.Content.ReadAsAsync<InterswitchBaseResponse>().Result;
                    interswitchResponse.error = interswitchFailedResponse.error;
                    interswitchResponse.ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.SystemError);
                    interswitchResponse.ResponseDescription = "FAILED";
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                interswitchResponse.ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.SystemError);
                interswitchResponse.ResponseDescription = "Unable to process request";
            }

            Logger.LogInfo("BillsPaymentService.GetQuicktellerCategories, output", Newtonsoft.Json.JsonConvert.SerializeObject(interswitchResponse));
            return interswitchResponse;
        }

        /// <summary>
        /// Get all quickteller billers in a category
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns>List of quickteller billers</returns>
        public QuicktellerBillerList GetQuciktellerBillersByCategory(QuicktellerBillerRequest Request)
        {
            Logger.LogInfo("BillsPaymentService.GetQuciktelleBillrsByCategories", Request.CategoryId);
            QuicktellerBillerList interswitchResponse = new QuicktellerBillerList();

            try
            {
                //Make server certification call back
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                            | SecurityProtocolType.Tls11
                            | SecurityProtocolType.Tls12
                            | SecurityProtocolType.Ssl3;

                var baseAddress = string.Format("{0}/api/v2/quickteller/categorys/{1}/billers", System.Configuration.ConfigurationManager.AppSettings["InterswitchURL"], Request.CategoryId);


                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseAddress)))
                    {
                        client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BillPaymentTimeout"]));

                        client.DefaultRequestHeaders.Add("TerminalId", TerminalID);
                        client.DefaultRequestHeaders.Add("id", Request.CategoryId);
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        InterswitchAuthenticationService.SetHeaders(client, baseAddress, "GET", "clientID", "secretKey");

                        var responseTask = client.SendAsync(request);
                        responseTask.Wait();
                        response = responseTask.Result;
                    }
                }
                //httpPostResponse.Wait();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    interswitchResponse = response.Content.ReadAsAsync<QuicktellerBillerList>().Result;
                    if (interswitchResponse.billers != null && interswitchResponse.billers.Count > 0)
                    {
                        List<Task> billerPaymentItemsTasks = new List<Task>();
                        foreach (var biller in interswitchResponse.billers)
                        {
                            if (!_billersPaymentItemConcurrentDict.Keys.Contains(biller.billerid))
                            {
                                var tsk = new TaskFactory().StartNew(() => RetrievePaymentItemsForBiller(biller.billerid));
                                billerPaymentItemsTasks.Add(tsk);
                            }
                        }
                        Task.WaitAll(billerPaymentItemsTasks.ToArray());

                        foreach (var biller in interswitchResponse.billers)
                        {
                            List<Paymentitem> theData = new List<Paymentitem>();
                            if (_billersPaymentItemConcurrentDict.TryGetValue(biller.billerid, out theData))
                            {
                                biller.paymentitems = theData;
                            }
                        }
                    }
                    interswitchResponse.ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.Successful);
                    interswitchResponse.ResponseDescription = "SUCCESSFUL";
                }
                else
                {
                    InterswitchBaseResponse interswitchFailedResponse = response.Content.ReadAsAsync<InterswitchBaseResponse>().Result;
                    interswitchResponse.error = interswitchFailedResponse.error;
                    interswitchResponse.ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.SystemError);
                    interswitchResponse.ResponseDescription = "FAILED";
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                interswitchResponse.ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.SystemError);
                interswitchResponse.ResponseDescription = "Unable to process reqeust";
            }

            return interswitchResponse;
        }

        /// <summary>
        /// Get biller payment items per quickteller biller
        /// </summary>
        /// <param name="BillerId"></param>
        /// <returns>List of payment items</returns>
        public QucktellerPaymentItems GetQuciktellerPaymentItems(QuicktellerPaymentItemRequest Request)
        {
            QucktellerPaymentItems interswitchResponse = new QucktellerPaymentItems();

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                            | SecurityProtocolType.Tls11
                            | SecurityProtocolType.Tls12
                            | SecurityProtocolType.Ssl3;

                var baseAddress = string.Format("{0}/api/v2/quickteller/billers/{1}/paymentitems", System.Configuration.ConfigurationManager.AppSettings["InterswitchURL"], Request.BillerId);


                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseAddress)))
                    {
                        client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BillPaymentTimeout"]));

                        client.DefaultRequestHeaders.Add("TerminalId", TerminalID);
                        client.DefaultRequestHeaders.Add("BillerId", Request.BillerId);
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        InterswitchAuthenticationService.SetHeaders(client, baseAddress, "GET", "clientID", "secretKey");

                        var responseTask = client.SendAsync(request);
                        responseTask.Wait();
                        response = responseTask.Result;
                    }
                }
                //httpPostResponse.Wait();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    interswitchResponse = response.Content.ReadAsAsync<QucktellerPaymentItems>().Result;
                    interswitchResponse.ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.Successful);
                    interswitchResponse.ResponseDescription = "SUCCESSFUL";
                }
                else
                {
                    InterswitchBaseResponse interswitchFailedResponse = response.Content.ReadAsAsync<InterswitchBaseResponse>().Result;
                    interswitchResponse.error = interswitchFailedResponse.error;
                    interswitchResponse.ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.SystemError);
                    interswitchResponse.ResponseDescription = "FAILED";
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                interswitchResponse.ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.SystemError);
                interswitchResponse.ResponseDescription = "Unable to process request";
            }
            return interswitchResponse;
        }

        /// <summary>
        /// Retrieves the list of <see cref="Paymentitem"/> for the specified billerID. 
        /// Data retrieved is added to the <seealso cref="_billersPaymentItemConcurrentDict"/> dictionary. 
        /// </summary>
        /// <param name="billerID">ID of the Biller whose <see cref="Paymentitem"/> is to be fetched.</param>
        private void RetrievePaymentItemsForBiller(string billerID)
        {
            int loopCount = 0;
            bool addedSuccessfully = false;
            int numberOfItemsAdded = 0;
            try
            {
                QucktellerPaymentItems pItems = GetQuciktellerPaymentItems(new QuicktellerPaymentItemRequest { BillerId = billerID });
                if (pItems != null && pItems.ResponseCode == "00" && pItems.paymentitems.Count > 0)
                {
                    numberOfItemsAdded = pItems.paymentitems.Count;
                    do
                    {
                        if (!_billersPaymentItemConcurrentDict.Keys.Contains(billerID))
                        {
                            addedSuccessfully = _billersPaymentItemConcurrentDict.TryAdd(billerID, pItems.paymentitems);
                        }
                        else
                        {
                            addedSuccessfully = true;
                        }
                    } while (!addedSuccessfully && loopCount <= 50);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("Exception In BillPaymentService.RetrievePaymentItemsForBiller ", ex));
            }
            Logger.LogInfo("BillPaymentService.RetrievePaymentItemsForBiller => ", $"addedSuccessfully = {addedSuccessfully} | loopCount = {loopCount} | billerID = {billerID} | numberOfItemsAdded = {numberOfItemsAdded} ");
        }

        /// <summary>
        /// Bill payment advice to interswitch
        /// </summary>
        /// <param name="input"></param>
        /// <returns>ProvidusTransactionResponse</returns>
        public BillPaymnetAdviceResponse BillsPaymentAdvice(BillPaymentAdviceRequest paymentRequest)
        {
            Logger.LogInfo("BillsPaymentService.BillsPaymentAdvice", paymentRequest);
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox"], out bool isCustomerValidationSandBox);

            //call providusAPI to do posting for providus bill payment request


            paymentRequest.PhoneNumber = paymentRequest.customerMobile;
            string builder = paymentRequest.customerMobile;
            if (builder.StartsWith("0"))
            {
                builder = builder.Substring(1, builder.Length - 1);
                paymentRequest.customerMobile = "234" + builder;
            }
            BillPaymnetAdviceResponse interswitchResponse = new BillPaymnetAdviceResponse();
            HttpResponseMessage response = null;
            string trxChannel = paymentRequest.isMobile ? "Mobile" : "Online";
            decimal postingAmount = Convert.ToDecimal(paymentRequest.amount) + Convert.ToDecimal(paymentRequest.itemFee);
            string creditSuspenseAccount = System.Configuration.ConfigurationManager.AppSettings.Get("BillPaymentSuspenseAccount");
            paymentRequest.SuspenseAccount = creditSuspenseAccount;

            try
            {

                var baseAddress = string.Format("{0}/api/v2/quickteller/payments/advices", System.Configuration.ConfigurationManager.AppSettings["InterswitchURL"]);

                var trx = new LifestyleDebitRequest
                {
                    AccountNumber = paymentRequest.AccountNo,
                    Amount = Convert.ToString(paymentRequest.amount),
                    Narration = string.Format("{0}: Bill Payment", trxChannel),
                    RequestChannel = trxChannel,
                    MerchantId = "999030190522232343",
                    PIN = paymentRequest.PIN,
                    CustomerNumber = paymentRequest.CustomerNumber,
                    Currency = paymentRequest.Currency,
                    RefId = DateTime.Now.Ticks.ToString()
                };
                Logger.LogInfo("BillsPaymentService.BillsPaymentAdvice, trxRequestMessage", trx);
                LifestyleDebitResponse trxResponse = _BankTransfer.LifeStyleDebit(trx);

                Logger.LogInfo("BillsPaymentService.BillsPaymentAdvice, trx response", trxResponse);

                if (trxResponse.ResponseCode != "00")
                {
                    interswitchResponse = new BillPaymnetAdviceResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Debit failed: " + trxResponse.ResponseDescription,
                    };
                    return interswitchResponse;
                }


                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                            | SecurityProtocolType.Tls11
                            | SecurityProtocolType.Tls12
                            | SecurityProtocolType.Ssl3;

                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;


                paymentRequest.TerminalId = TerminalID;
                paymentRequest.requestReference = BankPrefix + (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString("F0").Substring(7, 5);
                paymentRequest.amount = (Convert.ToDecimal(paymentRequest.amount) * 100).ToString().Replace(".00", "");
                #region "BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox" is true, so populate dummy data for Interswitch Staging Environment
                if (isCustomerValidationSandBox)
                {
                    paymentRequest.customerEmail = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox.CustmerEmail"]);
                    paymentRequest.CustomerUniqueCode = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox.CustmerID"]);
                    paymentRequest.customerMobile = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox.CustmerMobile"]);
                    paymentRequest.paymentCode = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox.PaymentCode"]);
                    Logger.LogInfo("BillsPaymentService.BillsPaymentAdvice: Using SandBox data from the config file! ", paymentRequest);
                }
                #endregion
                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(paymentRequest);

                #region Bill payment service
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BillPaymentTimeout"]));
                    using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAddress)))
                    {
                        Logger.LogInfo("BillsPaymentService.BillsPaymentAdvice", "Interswitchjson: " + jsonContent);
                        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        request.Headers.Add("TerminalId", TerminalID);
                        InterswitchAuthenticationService.SetHeaders(request, baseAddress, "POST", "clientID", "secretKey");

                        var responseTask = client.SendAsync(request);
                        responseTask.Wait();
                        response = responseTask.Result;
                    }
                }

                string serializedFailedResponse = string.Empty;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    interswitchResponse = response.Content.ReadAsAsync<BillPaymnetAdviceResponse>().Result;
                    interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.Successful);
                    interswitchResponse.ResponseDescription = "SUCCESSFUL";
                    interswitchResponse.requestReference = paymentRequest.requestReference;
                }
                else
                {
                    var type = response.Content.Headers.ContentType;
                    InterswitchBaseResponse interswitchFailedResponse = response.Content.ReadAsAsync<InterswitchBaseResponse>().Result;
                    interswitchResponse.error = interswitchFailedResponse.error;
                    interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.SystemError);
                    interswitchResponse.ResponseDescription = interswitchFailedResponse.error.message;
                    Logger.LogInfo("BillsPaymentService.BillsPaymentAdvice", Newtonsoft.Json.JsonConvert.SerializeObject(interswitchResponse));
                }
                #endregion

                if (interswitchResponse.ResponseCode == "00")
                {
                    interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.Successful);
                    interswitchResponse.ResponseDescription = "Bill payment successful ";
                }
                else if (interswitchResponse.error != null && (interswitchResponse.error.code == "900A0" || interswitchResponse.error.code == "90009"))
                {
                    BillPaymentStatusRequest queryTransaction = new BillPaymentStatusRequest()
                    {
                        TransactiontReference = paymentRequest.requestReference
                    };

                    BillPaymentStatusResponse queryResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<BillPaymentStatusResponse>(CheckBillsPaymentStatus(Newtonsoft.Json.JsonConvert.SerializeObject(queryTransaction)));
                    if (queryResponse.ResponseCode == "00" || queryResponse.status == "Complete")
                    {
                        if (queryResponse.transactionResponseCode == "90000")
                        {
                            interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.Successful);
                            interswitchResponse.ResponseDescription = "SUCCESSFUL";
                            interswitchResponse.requestReference = paymentRequest.requestReference;
                        }
                        else
                        {
                            interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.Successful);
                            interswitchResponse.ResponseDescription = "Request is Processing";
                            interswitchResponse.requestReference = paymentRequest.requestReference;
                        }
                    }
                    else
                    {
                        interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.SystemError);
                        interswitchResponse.ResponseDescription = "FAILED: Unable to pay bill, please try again.";
                    }
                    Logger.LogInfo("BillsPaymentAdvice => CheckBillsPaymentStatus interswitchResponse", Newtonsoft.Json.JsonConvert.SerializeObject(interswitchResponse));
                }
                else
                {

                    interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.FailedBillsPaymentReversal);
                    //previous code was displaying transaction response from initial customer debit not reversal ** interswitchResponse.ResponseDescription = "Unable to Complete bill payment, Reversal failed " + postingResponse.ResponseDescription; **
                    interswitchResponse.ResponseDescription = "Unable to Complete bill payment, Reversal failed ";
                }


            }
            catch (Exception ex)
            {
                BillPaymentStatusRequest queryTransaction = new BillPaymentStatusRequest()
                {
                    TransactiontReference = paymentRequest.requestReference
                };

                BillPaymentStatusResponse queryResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<BillPaymentStatusResponse>(CheckBillsPaymentStatus(Newtonsoft.Json.JsonConvert.SerializeObject(queryTransaction)));
                if (queryResponse.ResponseCode == "00" || queryResponse.status == "Complete")
                {
                    if (queryResponse.transactionResponseCode == "90000")
                    {
                        interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.Successful);
                        interswitchResponse.ResponseDescription = "SUCCESSFUL";
                        interswitchResponse.requestReference = paymentRequest.requestReference;
                    }
                    else
                    {
                        interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.Successful);
                        interswitchResponse.ResponseDescription = "Request is Processing";
                        interswitchResponse.requestReference = paymentRequest.requestReference;
                    }
                }
                else
                {
                    interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.SystemError);
                    interswitchResponse.ResponseDescription = "FAILED: Unable to pay bill, please try again.";
                }
                Logger.LogError(ex);
                Logger.LogInfo("BillsPaymentAdvice Exception => ", ex.InnerException.Message + " " + ex.StackTrace);
                interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.SystemError);
                interswitchResponse.ResponseDescription = "FAILED: Unable to pay bill, please try again.";
            }

            Logger.LogInfo("BillsPaymentService.BillsPaymentAdvice, output", Newtonsoft.Json.JsonConvert.SerializeObject(interswitchResponse));
            return interswitchResponse;
        }

        /// <summary>
        /// Check current bill payment advice status
        /// </summary>
        /// <param name="BillPaymentStatusRequest"></param>
        /// <returns>BillPaymentStatusResponse</returns>
        public string CheckBillsPaymentStatus(string input)
        {
            Logger.LogInfo("BillsPaymentService.CheckBillsPaymentStatus", input);

            BillPaymentStatusRequest checkPaymentRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<BillPaymentStatusRequest>(input);
            BillPaymentStatusResponse interswitchResponse = new BillPaymentStatusResponse();

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                            | SecurityProtocolType.Tls11
                            | SecurityProtocolType.Tls12
                            | SecurityProtocolType.Ssl3;

                var baseAddress = string.Format("{0}/api/v2/quickteller/transactions?requestreference={1}", System.Configuration.ConfigurationManager.AppSettings["InterswitchPaycodeURL"], checkPaymentRequest.TransactiontReference);


                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(baseAddress)))
                    {
                        client.Timeout = TimeSpan.FromSeconds(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BillPaymentTimeout"]));

                        client.DefaultRequestHeaders.Add("TerminalId", TerminalID);
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        InterswitchAuthenticationService.SetHeaders(client, baseAddress, "GET", "clientID", "secretKey");

                        client.Timeout = TimeSpan.FromSeconds(30);
                        var responseTask = client.SendAsync(request);
                        responseTask.Wait();
                        response = responseTask.Result;
                    }
                }
                //httpPostResponse.Wait();

                Logger.LogInfo("InterswitchPayCodeService.CheckStatus,stringResponse", response.Content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    interswitchResponse = response.Content.ReadAsAsync<BillPaymentStatusResponse>().Result;
                    interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.Successful);
                    interswitchResponse.ResponseDescription = "SUCCESSFUL";
                }
                else
                {
                    InterswitchBaseResponse interswitchFailedResponse = response.Content.ReadAsAsync<InterswitchBaseResponse>().Result;
                    interswitchResponse.error = interswitchFailedResponse.error;
                    interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.SystemError);
                    interswitchResponse.ResponseDescription = "FAILED";
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.SystemError);
                interswitchResponse.ResponseDescription = "Unable to process request";
            }

            Logger.LogInfo("BillsPaymentService.CheckBillsPaymentStatus, output", Newtonsoft.Json.JsonConvert.SerializeObject(interswitchResponse));
            return Newtonsoft.Json.JsonConvert.SerializeObject(interswitchResponse);
        }

        /// <summary>
        /// Quickteller biller/merchant customer validation
        /// </summary>
        /// <param name="BillerCustomerValidation">Customer validation request payload</param>
        /// <returns>CustomerValidationResponse</returns>
        public CustomerValidationResponse CustomerValidation(BillerCustomerValidation vaildationRequest)
        {
            Logger.LogInfo("BillsPaymentService.CustomerValidation", vaildationRequest);
            CustomerValidationResponse interswitchResponse = new CustomerValidationResponse();
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox"], out bool isCustomerValidationSandBox);

            try
            {
                #region "BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox" is true, so populate dummy data for Interswitch Staging Environment
                if (isCustomerValidationSandBox && vaildationRequest.customers != null && vaildationRequest.customers.Count > 0)
                {
                    vaildationRequest.customers.ForEach(x =>
                    {
                        x.customerId = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox.CustmerID"]);
                        x.paymentCode = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BillsPaymentService.CustomerValidation.IsCustomerValidationSandBox.PaymentCode"]);
                    });
                    Logger.LogInfo("BillsPaymentService.CustomerValidation: Using SandBox data from the config file! ", vaildationRequest);
                }
                #endregion

                var baseAddress = string.Format("{0}/api/v2/quickteller/customers/validations", System.Configuration.ConfigurationManager.AppSettings["InterswitchURL"]);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                            | SecurityProtocolType.Tls11
                            | SecurityProtocolType.Tls12
                            | SecurityProtocolType.Ssl3;

                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;

                string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(vaildationRequest);

                #region Bill payment service
                HttpResponseMessage response = null;
                using (var client = new HttpClient())
                {
                    int serviceTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BillPaymentTimeout"]);
                    if (TerminalID == string.Empty || serviceTimeout == 0)
                    {
                        interswitchResponse = new CustomerValidationResponse()
                        {
                            ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.SystemError),
                            ResponseDescription = "Invalid coniguration settings for interswitch bill payment, ",
                        };

                        Logger.LogInfo("BillsPaymentService.CustomerValidation, failedResponse ", interswitchResponse);
                        return interswitchResponse;
                    }
                    client.Timeout = TimeSpan.FromSeconds(serviceTimeout);
                    using (var request = new HttpRequestMessage(HttpMethod.Post, new Uri(baseAddress)))
                    {
                        Logger.LogInfo("BillsPaymentService.CustomerValidation", "Interswitchjson: " + jsonContent);
                        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        request.Headers.Add("TerminalId", TerminalID);
                        InterswitchAuthenticationService.SetHeaders(request, baseAddress, "POST", "clientID", "secretKey");

                        var responseTask = client.SendAsync(request);
                        responseTask.Wait();
                        response = responseTask.Result;
                    }
                }
                #endregion

                string serializedFailedResponse = string.Empty;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    interswitchResponse = response.Content.ReadAsAsync<CustomerValidationResponse>().Result;

                    #region TODO: Interswitch responceCode value of 90000 is successful. Any other value should be treated as not successful.
                    if (interswitchResponse.Customers != null && interswitchResponse.Customers.Count > 0 && !interswitchResponse.Customers.Any(x => x.responseCode == "90000"))
                    {
                        // Handle scenarios where there is no successful response. 
                    }
                    #endregion

                    interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.Successful);
                    interswitchResponse.ResponseDescription = "SUCCESSFUL";
                }
                else
                {
                    var type = response.Content.Headers.ContentType;
                    InterswitchBaseResponse interswitchFailedResponse = response.Content.ReadAsAsync<InterswitchBaseResponse>().Result;
                    interswitchResponse.error = interswitchFailedResponse.error;
                    interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.SystemError);
                    interswitchResponse.ResponseDescription = interswitchFailedResponse.error.message;
                    Logger.LogInfo("BillsPaymentService.CustomerValidation", Newtonsoft.Json.JsonConvert.SerializeObject(interswitchResponse));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Logger.LogInfo("BillsPaymentService.CustomerValidation.DoBillsPaymentException", ex.InnerException.Message + " " + ex.StackTrace);
                interswitchResponse.ResponseCode = Utils.ToIntVal(MiddleWareResponseCodes.SystemError);
                interswitchResponse.ResponseDescription = "FAILED: Unable to pay bill, please try again.";
            }

            Logger.LogInfo("BillsPaymentService.CustomerValidation, output", interswitchResponse);
            return interswitchResponse;
        }

    }
}
