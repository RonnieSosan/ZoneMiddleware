using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.SterlingImplementation.Persistence;
using AppZoneMiddleware.Shared.Extension;
using Blend.SterlingImplementation.NotificationService;
using System.Text.RegularExpressions;
using Blend.SharedServiceImplementation;

namespace Blend.SterlingImplementation.ProfileService
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
                if (UserProfile == null)
                {

                    response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Profile not found",
                    };

                }
                else
                {
                    if (string.IsNullOrEmpty(passwordRequest.AuthToken))
                    {
                        //Meaning user is doing a reset Password no need to verify the old password
                        UserProfile.Password = PasswordStorage.CreateHash(passwordRequest.NewPassword);
                        UserProfile.PIN = PasswordStorage.CreateHash(passwordRequest.NewPassword);    // Based on requirements, Password and PIN should be set to the same value
                        UserProfile.IsLocked = false;
                        UserProfile.InvalidTrials = 0;
                        repository.Update(UserProfile);

                        if (passwordRequest.MailRequest != null)
                        {
                            passwordRequest.MailRequest.customer_id = passwordRequest.CustomerID;
                            // new MailService().SendMailToCustomer(passwordRequest.MailRequest);
                            Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailRequest>(Newtonsoft.Json.JsonConvert.SerializeObject(passwordRequest.MailRequest)));
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
                            UserProfile.PIN = PasswordStorage.CreateHash(passwordRequest.NewPassword);    // Based on requirements, Password and PIN should be set to the same value
                            UserProfile.WasPasswordRecentlyReset = false;
                            repository.Update(UserProfile);

                            if (passwordRequest.MailRequest != null)
                            {
                                passwordRequest.MailRequest.customer_id = passwordRequest.CustomerID;
                                //new MailService().SendMailToCustomer(passwordRequest.MailRequest);
                                Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailRequest>(Newtonsoft.Json.JsonConvert.SerializeObject(passwordRequest.MailRequest)));
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
            }
            catch (Exception ex)
            {

                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "Unable to process request",
                };
                Logger.LogError(ex);
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
                        if (forgotPasswordRequest.MailRequest != null && !string.IsNullOrWhiteSpace(forgotPasswordRequest.MailRequest.MailBody))
                        {
                            userOtp.OTP = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(userOTP));
                            userOtp.OTPType = "Forgot Password OTP";
                            userOtp.OTPExpDate = DateTime.Now.AddMinutes(otpTimeSpan);

                            otpRep.Update(userOtp);

                            forgotPasswordRequest.MailRequest.MailBody = Regex.Replace(forgotPasswordRequest.MailRequest.MailBody, @"#newOtp", userOTP);
                            //forgotPasswordRequest.MailRequest.MailBody = Regex.Replace(forgotPasswordRequest.MailRequest.MailBody, @"#newPassword", "<strong>This will be sent in a separate mail.</strong>");
                            forgotPasswordRequest.MailRequest.MailBody = Regex.Replace(forgotPasswordRequest.MailRequest.MailBody, @"Temporary Password is: #newPassword", string.Format("OTP is: {0}", userOTP));
                            forgotPasswordRequest.MailRequest.MailBody = Regex.Replace(forgotPasswordRequest.MailRequest.MailBody, @"#newPassword", userOTP);
                            forgotPasswordRequest.MailRequest.MailBody = Regex.Replace(forgotPasswordRequest.MailRequest.MailBody, @"#expiredOtpTime", DateTime.Now.AddMinutes(otpTimeSpan).ToString());
                            forgotPasswordRequest.MailRequest.MailBody = Regex.Replace(forgotPasswordRequest.MailRequest.MailBody, @"#DateandTime", DateTime.Now.AddMinutes(otpTimeSpan).ToString());
                            forgotPasswordRequest.MailRequest.customer_id = forgotPasswordRequest.CustomerID;
                            //new MailService().SendMailToCustomer(forgotPasswordRequest.MailRequest);
                            Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(forgotPasswordRequest.MailRequest)));
                            var sms = string.Format("Verification Code: {0}", userOTP);

                            var smsService = new SMSSender();
                            var request = new SMSRequest { PhoneNumber = forgotPasswordRequest.CustomerID, Message = sms };
                            Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendSMS").CallService<SMSResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(request)));

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
                                ResponseCode = "MW06",
                                ResponseDescription = "Mail template for sending the OTP for the Forgot Password/PIN operation is missing.",
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
                    ResponseDescription = "Unable to process request",
                };
                Logger.LogError(ex);
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
                                if (resetPasswordRequest.MailRequest != null && !string.IsNullOrWhiteSpace(resetPasswordRequest.MailRequest.MailBody))
                                {
                                    string newPassOrPIN = new PasswordStorage().generateOTP();
                                    newPassOrPIN = newPassOrPIN.Substring(0, (newPassOrPIN.Length >= 4 ? 4 : newPassOrPIN.Length)).PadRight(4, '0');
                                    resetPasswordRequest.NewPassword = newPassOrPIN;
                                    verifingAccount.Password = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(resetPasswordRequest.NewPassword));    // Based on requirements, Password and PIN should be set to the same value
                                    verifingAccount.PIN = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(resetPasswordRequest.NewPassword));    // Based on requirements, Password and PIN should be set to the same value
                                    verifingAccount.IsLocked = false;
                                    verifingAccount.InvalidTrials = 0;
                                    verifingAccount.WasPasswordRecentlyReset = true;    // Indicating that this user's password was just reset.
                                    repository.Update(verifingAccount);

                                    resetPasswordRequest.MailRequest.MailBody = Regex.Replace(resetPasswordRequest.MailRequest.MailBody, @"#newPassword", newPassOrPIN);
                                    resetPasswordRequest.MailRequest.customer_id = resetPasswordRequest.CustomerID;
                                    Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(resetPasswordRequest.MailRequest)));

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
                                        ResponseCode = "MW06",
                                        ResponseDescription = "Mail template for password/pin reset is missing.",
                                    };
                                }
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
                    ResponseDescription = "Unable to process request",
                };
                Logger.LogError(ex);
            }

            Logger.LogInfo("UserProfileService.resetPassword, response", response);

            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }
    }
}
