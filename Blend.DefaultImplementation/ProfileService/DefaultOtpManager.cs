using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.DefaultImplementation.Persistence;
using AppZoneMiddleware.Shared.Extension;
using System.Text.RegularExpressions;

namespace Blend.DefaultImplementation.ProfileService
{
    public class DefaultOtpManager : IOtpManager
    {
        public double otpTimeSpan = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings.Get("OTPLifeSpan"));
        IAccountInquiry _accountInquiryService;
        ISMSSender _smsService;
        IMailService _mailService;

        public DefaultOtpManager(IMailService mailServcie, ISMSSender smsService, IAccountInquiry accountInquiryService)
        {
            _mailService = mailServcie;
            _smsService = smsService;
            _accountInquiryService = accountInquiryService;
        }

        public Task<SendOtpResponse> SendOtp(SendOTPRequest userOtpRequest)
        {
            Logger.LogInfo("UserProfileService.SendOtp, input", userOtpRequest);
            SendOtpResponse response = null;
            string phoneNumber = string.Empty; //phone number for sending customer OTP
            string email = string.Empty; //Email address to send user OTP
            try
            {
                ContextRepository<UserOTP> OTPRepository = new ContextRepository<UserOTP>();
                string userOTP = new PasswordStorage().generateOTP();
                UserOTP otpUser = null;

                //send otp for open account customer without customer id or BVN
                if (userOtpRequest.BVN != string.Empty)
                {
                    //using BVN Scenario
                    otpUser = OTPRepository.Get(userOtpRequest.BVN);
                    NIPResponse nipBvnResponse = _accountInquiryService.DoNIPBVNInquiry(new NIPRequest { BVN = userOtpRequest.BVN }).Result;
                    if (nipBvnResponse.ResponseCode == "00")
                    {

                        email = nipBvnResponse.EmailAddress;
                        phoneNumber = nipBvnResponse.PhoneNumber;
                    }
                }
                else
                {

                    otpUser = OTPRepository.Get(otpUser.CustomerID);
                    CustomerAccountsResponse cbaResponse = _accountInquiryService.GetAccountsWithCustomerID(new AccountRequest { CustomerID = userOtpRequest.CustomerID }).Result;
                    if (cbaResponse.ResponseCode == "00" && cbaResponse.AccountInformation.Count > 0)
                    {
                        AccountDetails account = cbaResponse.AccountInformation.FirstOrDefault();
                        email = account.emailAddress;
                        phoneNumber = account.PHONE;
                    }
                }

                if (otpUser == null)
                {
                    otpUser = new UserOTP()
                    {
                        CustomerID = userOtpRequest.CustomerID,
                        OTP = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(userOTP)),
                        OTPType = userOtpRequest.OTPType,
                        OTPExpDate = DateTime.Now.AddMinutes(otpTimeSpan),
                    };
                    OTPRepository.Save(otpUser);
                }
                else
                {
                    otpUser.OTPType = userOtpRequest.OTPType;
                    otpUser.OTP = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(userOTP));
                    otpUser.OTPExpDate = DateTime.Now.AddMinutes(otpTimeSpan);

                    OTPRepository.Update(otpUser);
                }

                Logger.LogInfo("UserProfileService.SendOtp, User OTP", userOTP);
                if (userOtpRequest.MailRequest != null)
                {
                    userOtpRequest.MailRequest.MailBody = Regex.Replace(userOtpRequest.MailRequest.MailBody, @"#newOtp", userOTP);
                    userOtpRequest.MailRequest.MailBody = Regex.Replace(userOtpRequest.MailRequest.MailBody, @"#expiredOtpTime", otpUser.OTPExpDate.ToString());

                    //validate emailaddress
                    UserProfileResponse mailValidation = _mailService.ValidateEmailAddress(userOtpRequest.CustomerID);
                    if (mailValidation.ResponseCode == "00")
                    {
                        userOtpRequest.MailRequest.MailRecepients = email;
                        _mailService.SendMail(userOtpRequest.MailRequest);
                    }

                    var sms = string.Format("Verification Code: {0}", userOTP);

                    _smsService.Send(new SMSRequest { customer_id = userOtpRequest.CustomerID, Message = sms, PhoneNumber = phoneNumber });
                }
                if (userOtpRequest.isResend)
                {
                    StringBuilder _phoneNumber = new StringBuilder();
                    _phoneNumber.Append(phoneNumber.Substring(0, 4));
                    _phoneNumber.Append("****");
                    _phoneNumber.Append(phoneNumber.Substring(9, 3));
                    phoneNumber = _phoneNumber.ToString();
                }
                response = new SendOtpResponse()
                {
                    ResponseCode = "00",
                    ResponseDescription = "User otp created successfully",
                    MaskedPhoneNumber = userOtpRequest.isResend ? phoneNumber : string.Empty
                };
            }
            catch (Exception ex)
            {
                response = new SendOtpResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
                Logger.LogError(ex);
            };

            Logger.LogInfo("UserProfileService.SendOtp, response", response);

            return Task<SendOtpResponse>.Factory.StartNew(() => response);
        }

        public Task<ValidateOTPResponse> ValidateOTP(UserOTP userOTPrequest)
        {
            Logger.LogInfo("UserProfileService.ValidateOTP, input", userOTPrequest);
            ValidateOTPResponse response = null;
            try
            {
                ContextRepository<UserOTP> OTPRepository = new ContextRepository<UserOTP>();
                UserOTP otpUser = null;
                string userToken = new PasswordStorage().generateOTP();
                //Validate customers OTP
                if (userOTPrequest.BVN != string.Empty)
                {
                    otpUser = OTPRepository.Get(userOTPrequest.BVN);
                }
                else
                {
                    otpUser = OTPRepository.Get(userOTPrequest.CustomerID);
                }

                if (otpUser == null)
                {
                    if (otpUser.OTPExpDate > System.DateTime.Now)
                    {
                        if (PasswordStorage.VerifyPassword(userOTPrequest.OTP, otpUser.OTP))
                        {
                            response = new ValidateOTPResponse()
                            {
                                ResponseCode = "00",
                                ResponseDescription = "valid OTP",
                                AuthenticationToken = new PasswordStorage().CryptoHash(userToken)
                            };
                            Logger.LogInfo("UserProfileService.ValidateOTP, User OTP Valid", userOTPrequest.OTP);
                        }
                        else
                        {
                            response = new ValidateOTPResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "invalid OTP",
                            };
                        }
                    }
                    else
                    {
                        response = new ValidateOTPResponse()
                        {
                            ResponseCode = "06",
                            ResponseDescription = "OTP Expired",
                        };
                    }
                }
                else
                {
                    response = new ValidateOTPResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "No OTP previously sent for this user",
                    };
                }
            }
            catch (Exception ex)
            {
                response = new ValidateOTPResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "Unable to process request",
                };
                Logger.LogError(ex);
            };

            Logger.LogInfo("UserProfileService.ValidateOTP, response", response);

            return Task<ValidateOTPResponse>.Factory.StartNew(() => response);
        }
    }
}
