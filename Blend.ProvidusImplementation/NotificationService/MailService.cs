using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using System.IO;
using System.Net.Mail;
using System.Configuration;
using System.Net.Mime;
using System.Net;
using System.Text.RegularExpressions;
using Blend.ProvidusImplementation.CoreBankingService;

namespace Blend.ProvidusImplementation.NotificationService
{
    public class MailService : IMailService
    {
        public Task<MailResponse> SendMail(MailRequest MailRequest)
        {

            Logger.LogInfo("Sendmail", (object)MailRequest);

            Task<MailResponse> mailResponse = null;
            try
            {
                string theInput = string.Empty;
                theInput = WebUtility.HtmlDecode(MailRequest.MailBody);
                theInput = theInput.Replace("&lt;", "<").Replace("&gt;", ">");

                string err = string.Empty;
                bool authenticated = true;

                bool response = false;

                Logger.LogInfo("authenticated for mail", authenticated);

                if (authenticated)
                {

                    #region For adding customer name to email body
                    if (theInput.Contains("#CustomerName"))
                    {
                        Logger.LogInfo("Sendmail.Modification replace", " #CustomerName");
                        AccountRequest customer = new AccountRequest()
                        {
                            CustomerID = MailRequest.customer_id,
                        };

                        string jsonCustomer = Newtonsoft.Json.JsonConvert.SerializeObject(customer);

                        //Retrieve the customer details using the customer id
                        List<AccountDetails> accounts = new CoreBankingService.AccountInquiryService().GetAccountsWithCustomerID(customer).Result.AccountInformation.ToList();
                        Logger.LogInfo("Sendmail.Modification for CustomerName", accounts.FirstOrDefault().AccountName);

                        theInput = Regex.Replace(theInput, @"#CustomerName", accounts.FirstOrDefault().AccountName);
                        MailRequest.MailRecepients = string.IsNullOrEmpty(MailRequest.MailRecepients) ? accounts.FirstOrDefault().emailAddress : MailRequest.MailRecepients;
                    }

                    #endregion
                    if (string.IsNullOrEmpty(MailRequest.MailRecepients))
                    {
                        mailResponse = Task<MailResponse>.Factory.StartNew(() =>
                        {
                            return new MailResponse
                            {
                                ResponseCode = "06",
                                ResponseDescription = "Unable to send mail, mail recipient not specified"
                            };
                        });
                    }
                    response = SendMail(theInput, MailRequest.MailRecepients.ToLower().Trim(), MailRequest.MailSubject, MailRequest.Attachement);
                }

                if (Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["ignoreMailFailure"]))
                {
                    response = true;
                }

                mailResponse = Task<MailResponse>.Factory.StartNew(() =>
                {
                    return new MailResponse
                    {
                        ResponseCode = response ? "00" : "06",
                        ResponseDescription = response ? "Successful" : "A system error occurred. Unable to send mail"
                    };
                });

                return mailResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                bool ignoreMailFailure = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["ignoreMailFailure"]);

                mailResponse = Task<MailResponse>.Factory.StartNew(() =>
               {
                   return new MailResponse
                   {
                       ResponseCode = ignoreMailFailure ? "00" : "06",
                       ResponseDescription = ignoreMailFailure ? "Successful" : "A system error occurred. Unable to send mail"
                   };
               });

                return mailResponse;
            }
        }

        public bool SendMail(string mailBody, string emailRecepients, string mailSubject, Dictionary<string, byte[]> mailAttachment)
        {
            bool result = false;
            string server = ConfigurationManager.AppSettings["SmtpServer"];
            string port = ConfigurationManager.AppSettings["SmtpPort"];
            string mailfrom = ConfigurationManager.AppSettings["MailFrom"];
            string usesAuth = ConfigurationManager.AppSettings["UsesAuthentication"];
            string domain = ConfigurationManager.AppSettings["SmtpDomain"];
            string username = ConfigurationManager.AppSettings["SmtpUserName"];
            string password = ConfigurationManager.AppSettings["SmtpPassword"];
            bool enableSSLInMail = Convert.ToBoolean(ConfigurationManager.AppSettings["enableSSLInMail"]);
            bool isTest = Convert.ToBoolean(ConfigurationManager.AppSettings["isTestEnvironment"]);

            SmtpClient client = new SmtpClient(server, Convert.ToInt32(port));

            if (Convert.ToBoolean(usesAuth))
            {
                //client.UseDefaultCredentials = false;
                //if (string.IsNullOrEmpty(domain))
                client.Credentials = new NetworkCredential(username, password);
                //else
                //    client.Credentials = new NetworkCredential(username, password, domain);
            }
            try
            {
                string mailPath = ConfigurationManager.AppSettings["MailPackage"];
                string mailTemplatePath = string.Format("{0}\\EmailTemplate.html", mailPath);
                string TheMailBody = File.ReadAllText(mailTemplatePath);
                mailBody = mailBody.Replace("<body>", "").Replace("</body>", "");
                TheMailBody = Regex.Replace(TheMailBody, @"#mailBody", mailBody);


                LinkedResource logo = new LinkedResource(string.Format("{0}\\logo.png", mailPath));
                logo.ContentId = "logo";
                LinkedResource googleplay = new LinkedResource(string.Format("{0}\\google-play-btn.png", mailPath));
                googleplay.ContentId = "googleplay";
                LinkedResource applestore = new LinkedResource(string.Format("{0}\\app-store-btn.png", mailPath));
                applestore.ContentId = "applestore";
                LinkedResource mobileImg = new LinkedResource(string.Format("{0}\\mobile.png", mailPath));
                mobileImg.ContentId = "mobileImg";

                AlternateView av1 = AlternateView.CreateAlternateViewFromString(TheMailBody, null, MediaTypeNames.Text.Html);
                av1.LinkedResources.Add(logo);
                av1.LinkedResources.Add(googleplay);
                av1.LinkedResources.Add(applestore);
                av1.LinkedResources.Add(mobileImg);


                System.Net.ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => { return true; };

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(mailfrom);

                mail.Subject = string.Format("{0}{1}", isTest ? "[Test]" : "", mailSubject);

                //mail.Body = mailBody;
                mail.Body = TheMailBody;
                mail.AlternateViews.Add(av1);


                foreach (var item in emailRecepients.Split(new string[] { "|", "rnrn" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.CC.Add(item);
                }

                if (mailAttachment != null)
                {
                    foreach (var item in mailAttachment.Keys)
                    {
                        Stream stream = new MemoryStream(mailAttachment[item]);
                        Attachment attachment = new Attachment(stream, item);
                        mail.Attachments.Add(attachment);
                    }
                }

                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.IsBodyHtml = true;
                //  client.UseDefaultCredentials = false;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = enableSSLInMail; //if not demo set to true, else set to false

                client.Send(mail);
                mail.Dispose();

                result = true;
                Logger.LogInfo("MailService.SendMail, mail sent to ", emailRecepients);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Logger.LogInfo("Sendmail", string.Format("user:{0}, pwd:{1}, IP:{2}, emailFrom:{3}, emailTo:{4}, port:{5}, ssl:{6}", username, password, server, mailfrom, emailRecepients, port, enableSSLInMail));

                Logger.LogInfo("MailService.SendMail, sending mail failed ", emailRecepients);
                if (isTest)
                    Logger.LogInfo("mail content", mailBody);

                return Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["ignoreMailFailure"]);

            }
            return result;
        }

        public Task<MailResponse> SendMailToCustomer(MailRequest MailRequest)
        {
            Logger.LogInfo(string.Format("SendMailToCustomer customer id, {0}: ", MailRequest.customer_id), MailRequest);
            MailRequest request = MailRequest;
            Logger.LogInfo("confirm mail request, ", request.MailSubject);

            AccountRequest customer = new AccountRequest();
            if (!string.IsNullOrEmpty(request.customer_id))
            {
                customer.CustomerID = request.customer_id;
            }
            else
            {
                return Task.Factory.StartNew<MailResponse>(() => new MailResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "No Customer Id found"
                });
            }
            string jsonCustomer = Newtonsoft.Json.JsonConvert.SerializeObject(customer);

            //Retrieve the customer details using the customer id
            List<AccountDetails> accounts = new CoreBankingService.AccountInquiryService().GetAccountsWithCustomerID(customer).Result.AccountInformation.ToList();
            if (string.IsNullOrEmpty(accounts.FirstOrDefault().emailAddress))
            {
                Task<MailResponse> noMailAddressResponse = Task<MailResponse>.Factory.StartNew(() =>
                {
                    return new MailResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No Mail address found"
                    };
                });

                Logger.LogInfo("SendMailToCustomer email, mailResponse : ", noMailAddressResponse);
                return noMailAddressResponse;
            }
            request.MailRecepients = accounts.FirstOrDefault().emailAddress;
            string nameToUse = accounts.FirstOrDefault().AccountName;
            request.MailBody = Regex.Replace(request.MailBody, @"#customerName", nameToUse);
            Task<MailResponse> mailResponse = SendMail(MailRequest);

            Logger.LogInfo("SendMailToCustomer email, mailResponse : ", mailResponse);
            return mailResponse;
        }

        public UserProfileResponse ValidateEmailAddress(string CustomerID)
        {
            Logger.LogInfo("MailService.validateEmailAddress, input ", CustomerID);
            UserProfileResponse response = null;
            string failedEmailResponse = string.Empty;
            //first validate the email address
            //Retrieve the customer details using the customer id
            try
            {
                AccountRequest acctReq = new AccountRequest() { CustomerID = CustomerID };
                List<AccountDetails> accounts = new CoreBankingService.AccountInquiryService().GetAccountsWithCustomerID(acctReq).Result.AccountInformation.ToList();
                var addr = new MailAddress(accounts.FirstOrDefault().emailAddress);
                if (addr.Address != accounts.FirstOrDefault().emailAddress)
                {
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No Email Address found",
                    };

                    Logger.LogInfo("MailService.validateEmailAddress, FaileEmailResponse", response);
                    return response;
                }
                else
                {
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Email Valid",
                    };

                    Logger.LogInfo("MailService.validateEmailAddress, successfulResponse", response);
                    return response;
                }
            }
            catch (FormatException)
            {
                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "Invalid Email Address",
                };

                failedEmailResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                Logger.LogInfo("MailService.validateEmailAddress, FaileEmailResponse", failedEmailResponse);

                return response;
            }
        }
    }
}
