using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.ProvidusImplementation.Persistence;
using AppZoneMiddleware.Shared.Extension;
using Blend.ProvidusImplementation.NotificationService;
using System.Text.RegularExpressions;

namespace Blend.ProvidusImplementation.ProfileService
{
    public class PasswordManager : IPasswordManager
    {
        public double otpTimeSpan = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings.Get("OTPLifeSpan"));

        public Task<UserProfileResponse> ChangePassword(ChangePasswordRequest passwordRequest)
        {
            Logger.LogInfo("UserProfileService.ChangePassword, input", passwordRequest);

            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            UserProfileResponse response = null;
            try
            {
                UserProfile UserProfile = repository.Get(passwordRequest.CustomerID);

                //tokin valid proceed to validate user password

                if (string.IsNullOrEmpty(passwordRequest.AuthToken))
                {
                    //Meaning user is doing a reset Password no need to verify the old password
                    UserProfile.Password = PasswordStorage.CreateHash(passwordRequest.NewPassword);
                    repository.Update(UserProfile);

                    if (passwordRequest.MailRequest != null)
                    {
                        passwordRequest.MailRequest.customer_id = passwordRequest.CustomerID;
                        new MailService().SendMailToCustomer(passwordRequest.MailRequest);
                    }

                    response = new UserProfileResponse()
                        {
                            ResponseCode = "00",
                            ResponseDescription = "password changed",
                        };
                }
                else
                {
                    if (PasswordStorage.VerifyPassword(passwordRequest.Password, UserProfile.Password))
                    {
                        UserProfile.Password = PasswordStorage.CreateHash(passwordRequest.NewPassword);
                        repository.Update(UserProfile);

                        if (passwordRequest.MailRequest != null)
                        {
                            passwordRequest.MailRequest.customer_id = passwordRequest.CustomerID;
                            new MailService().SendMailToCustomer(passwordRequest.MailRequest);
                        }

                        response = new UserProfileResponse()
                            {
                                ResponseCode = "00",
                                ResponseDescription = "password changed",
                            };
                    }
                    else
                    {
                        response = new UserProfileResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "password verification failed",
                            };
                    }
                }

            }
            catch (Exception ex)
            {

                response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "FAILED: " + ex.Message,
                    };
            }

            Logger.LogInfo("UserProfileService.ChangePassword, response", response);
            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> ForgotPassword(UserAccount forgotPasswordRequest)
        {
            Logger.LogInfo("UserProfileService.ForgotPassword, input", forgotPasswordRequest);
            UserProfileResponse response = null;

            try
            {
                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                ContextRepository<UserOTP> otpRep = new ContextRepository<UserOTP>();
                UserOTP userOtp = otpRep.Get(forgotPasswordRequest.CustomerID);
                UserProfile verifingAccount = repository.Get(forgotPasswordRequest.CustomerID);
                string userOTP = new PasswordStorage().generateOTP();
                Logger.LogInfo("UserProfileService.ForgotPassword, User OTP", userOTP);
                if (verifingAccount != null)
                {
                    //tokin valid proceed to validate user password
                    if (userOtp != null)
                    {

                        userOtp.OTP = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(userOTP));
                        userOtp.OTPType = "Forgot Password OTP";
                        userOtp.OTPExpDate = DateTime.Now.AddMinutes(otpTimeSpan);

                        otpRep.Update(userOtp);


                        if (forgotPasswordRequest.MailRequest != null)
                        {
                            forgotPasswordRequest.MailRequest.MailBody = Regex.Replace(forgotPasswordRequest.MailRequest.MailBody, @"#newOtp", userOTP);
                            forgotPasswordRequest.MailRequest.MailBody = Regex.Replace(forgotPasswordRequest.MailRequest.MailBody, @"#expiredOtpTime", DateTime.Now.AddMinutes(otpTimeSpan).ToString());
                            forgotPasswordRequest.MailRequest.customer_id = forgotPasswordRequest.CustomerID;
                            new MailService().SendMailToCustomer(forgotPasswordRequest.MailRequest);

                            var sms = string.Format("Verification Code: {0}", userOTP);

                            var smsService = new SMSSender();

                            smsService.Send(new SMSRequest { customer_id = forgotPasswordRequest.CustomerID, Message = sms });
                        }

                        response = new UserProfileResponse()
                            {
                                ResponseCode = "00",
                                ResponseDescription = "SUCCESSFUL",
                            };
                    }
                    else
                    {
                        userOtp.CustomerID = forgotPasswordRequest.CustomerID;
                        userOtp.OTP = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(userOTP));
                        userOtp.OTPType = "Forgot Password OTP";
                        userOtp.OTPExpDate = DateTime.Now.AddMinutes(otpTimeSpan);

                        otpRep.Save(userOtp);

                        response = new UserProfileResponse()
                        {
                            ResponseCode = "00",
                            ResponseDescription = "SUCCESSFUL",
                        };
                    }
                }
                else
                {
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "invalid customer ID",
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
            }

            Logger.LogInfo("UserProfileService.ForgotPassword, response", response);

            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            Logger.LogInfo("UserProfileService.ResetPassword, input", resetPasswordRequest);
            UserProfileResponse response = null;

            try
            {
                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                ContextRepository<UserOTP> otpRep = new ContextRepository<UserOTP>();
                UserOTP userOtp = otpRep.Get(resetPasswordRequest.CustomerID);
                UserProfile verifingAccount = repository.Get(resetPasswordRequest.CustomerID);


                //validate token
                if (verifingAccount != null)
                {
                    //token valid proceed to OTP validation
                    if (userOtp != null)
                    {
                        if (DateTime.Now <= userOtp.OTPExpDate)
                        {

                            if (PasswordStorage.VerifyPassword(resetPasswordRequest.userOTP.OTP, userOtp.OTP))
                            {
                                verifingAccount.Password = PasswordStorage.CreateHash(resetPasswordRequest.NewPassword);

                                repository.Update(verifingAccount);

                                response = new UserProfileResponse()
                                {
                                    ResponseCode = "00",
                                    ResponseDescription = "SUCCESSFUL",
                                };
                            }
                            else
                            {
                                response = new UserProfileResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "Invalid user OTP",
                                };
                            }
                        }
                        else
                        {
                            response = new UserProfileResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "User OTP Expired"
                            };
                        }
                    }
                    else
                    {
                        response = new UserProfileResponse()
                        {
                            ResponseCode = "06",
                            ResponseDescription = "OTP not available for this user",
                        };
                    }
                }
                else
                {
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "invalid customer ID",
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
                Logger.LogError(ex);
            }

            Logger.LogInfo("UserProfileService.resetPassword, response", response);

            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }
    }
}
