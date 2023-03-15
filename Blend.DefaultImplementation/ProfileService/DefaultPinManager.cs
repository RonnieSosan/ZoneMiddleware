using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using AppZoneMiddleware.Shared.Extension;
using System.Text.RegularExpressions;
using Blend.DefaultImplementation.Persistence;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;

namespace Blend.DefaultImplementation.ProfileService
{
    /// <summary>
    /// blend Default implementation for pin management
    /// </summary>
    public class DefaultPinManager : IPinManager
    {
        public double otpTimeSpan = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings.Get("OTPLifeSpan"));
        public IMailService _mailService;
        public ISMSSender _smsService;

        public DefaultPinManager(IMailService mailService, ISMSSender smsService)
        {
            _mailService = mailService;
            _smsService = smsService;
        }

        /// <summary>
        /// Default implementation for Changing customer pin
        /// </summary>
        /// <param name="changePinRequest">Change pin request message</param>
        /// <returns></returns>
        public Task<UserProfileResponse> ChangePin(ChangePinRequest changePinRequest)
        {
            Logger.LogInfo("UserProfileService.ChangePIN, input", changePinRequest);
            Task<UserProfileResponse> response = null;
            try
            {
                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                ContextRepository<UserOTP> otpRep = new ContextRepository<UserOTP>();
                UserOTP userOtp = otpRep.Get(changePinRequest.CustomerID);
                UserProfile verifingAccount = repository.Get(changePinRequest.CustomerID);

                #region validate customer session token
                UserProfileResponse AuthenticateTokenResponse = new DefaultTokenManager().NonTransactionalTokenAuthentication(new AuthenticationData() { customer_id = changePinRequest.CustomerID, AuthToken = changePinRequest.AuthToken });
                Logger.LogInfo("UserProfileService.ChangePIN/ResetPin, response", AuthenticateTokenResponse.ResponseCode);

                if (AuthenticateTokenResponse.ResponseCode == "06")
                {

                    response = Task<UserProfileResponse>.Factory.StartNew(() =>
                    {
                        return new UserProfileResponse()
                        {
                            ResponseCode = "06",
                            ResponseDescription = AuthenticateTokenResponse.ResponseDescription
                        };
                    });
                    Logger.LogInfo("UserProfileService.ChangePIN, response", response.Result);

                    return response;
                }
                //tokin valid proceed to validate user password
                Logger.LogInfo("UserProfileService.ChangePIN/ResetPin, SUCCESSFUL TOKEN AUTHENTICATION", AuthenticateTokenResponse.ResponseCode);
                #endregion

                if (changePinRequest.userOTP != null)
                {
                    Logger.LogInfo("UserProfileService.ResetPin, PASSWORD VERIFIED", changePinRequest.NewPin);
                    if (DateTime.Now <= userOtp.OTPExpDate)
                    {
                        //UserProfileResponse resetPinResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileResponse>(ResetPIN(input));
                        if (PasswordStorage.VerifyPassword(changePinRequest.userOTP.OTP, userOtp.OTP))
                        {
                            verifingAccount.PIN = PasswordStorage.CreateHash(changePinRequest.NewPin);
                            Logger.LogInfo("UserProfileService.ResetPin, NEW PIN", verifingAccount.PIN);
                            repository.Update(verifingAccount);

                            response = Task<UserProfileResponse>.Factory.StartNew(() =>
                            {
                                return new UserProfileResponse()
                                {
                                    ResponseCode = "00",
                                    ResponseDescription = "SUCCESSFUL",
                                };
                            });
                        }
                        else
                        {
                            response = Task<UserProfileResponse>.Factory.StartNew(() =>
                            {
                                return new UserProfileResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "Invalid user OTP",
                                };
                            });
                        }
                    }
                    else
                    {
                        response = Task<UserProfileResponse>.Factory.StartNew(() =>
                        {
                            return new UserProfileResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "user OTP Expired",
                            };
                        });
                    }
                }
                else
                {
                    Logger.LogInfo("UserProfileService.ChangePIN, PASSWORD VERIFIED", "");
                    if (PasswordStorage.VerifyPassword(changePinRequest.PIN, verifingAccount.PIN))
                    {
                        verifingAccount.PIN = PasswordStorage.CreateHash(changePinRequest.NewPin);
                        Logger.LogInfo("UserProfileService.ChangePIN, NEW PIN", verifingAccount.PIN);
                        repository.Update(verifingAccount);

                        if (changePinRequest.MailRequest != null)
                        {
                            changePinRequest.MailRequest.customer_id = changePinRequest.CustomerID;
                            _mailService.SendMailToCustomer(changePinRequest.MailRequest);
                        }

                        response = Task<UserProfileResponse>.Factory.StartNew(() =>
                        {
                            return new UserProfileResponse()
                            {
                                ResponseCode = "00",
                                ResponseDescription = "PIN changed",
                            };
                        });
                    }
                    else
                    {
                        response = Task<UserProfileResponse>.Factory.StartNew(() =>
                        {
                            return new UserProfileResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "PIN verification failed",
                            };
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                response = Task<UserProfileResponse>.Factory.StartNew(() =>
                {
                    return new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "FAILED: " + ex.Message,
                    };
                });
                Logger.LogError(ex);
            }
            Logger.LogInfo("UserProfileService.ChangePin/ResetPin, response", response.Result);

            return response;
        }

        public Task<UserProfileResponse> ForgotPin(UserAccount forgotPinRequest)
        {
            Logger.LogInfo("UserProfileService.ForgotPIN, input", forgotPinRequest);
            UserProfileResponse response = null;
            try
            {
                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                ContextRepository<UserOTP> otpRep = new ContextRepository<UserOTP>();
                UserOTP userOtp = otpRep.Get(forgotPinRequest.CustomerID);
                UserProfile verifingAccount = repository.Get(forgotPinRequest.CustomerID);

                //validate token
                UserProfileResponse AuthenticateTokenResponse = new DefaultTokenManager().NonTransactionalTokenAuthentication(new AuthenticationData() { customer_id = forgotPinRequest.CustomerID, AuthToken = forgotPinRequest.AuthToken });

                if (AuthenticateTokenResponse.ResponseCode == "06")
                {
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = AuthenticateTokenResponse.ResponseDescription,
                    };
                    Logger.LogInfo("UserProfileService.ChangePIN, ForgotPIN", response);

                    return Task<UserProfileResponse>.Factory.StartNew(() => response);
                }

                string userOTP = new PasswordStorage().generateOTP();
                Logger.LogInfo("UserProfileService.ForgotPIN, User OTP", userOTP);
                //tokin valid proceed to validate user password
                if (PasswordStorage.VerifyPassword(forgotPinRequest.Password, verifingAccount.Password))
                {
                    if (userOtp != null)
                    {

                        userOtp.OTP = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(userOTP));
                        userOtp.OTPType = "Forgot PIN OTP";
                        userOtp.OTPExpDate = DateTime.Now.AddMinutes(otpTimeSpan);

                        otpRep.Update(userOtp);

                        if (forgotPinRequest.MailRequest != null)
                        {
                            forgotPinRequest.MailRequest.MailBody = Regex.Replace(forgotPinRequest.MailRequest.MailBody, @"#newPin", userOTP);
                            forgotPinRequest.MailRequest.customer_id = forgotPinRequest.CustomerID;
                            _mailService.SendMailToCustomer(forgotPinRequest.MailRequest);

                            var sms = string.Format("Verification Code: {0}", userOTP);


                            _smsService.Send(new SMSRequest { customer_id = forgotPinRequest.CustomerID, Message = sms });
                        }

                        response = new UserProfileResponse()
                        {
                            ResponseCode = "00",
                            ResponseDescription = "SUCCESSFUL",
                        };

                    }
                    else
                    {
                        userOtp.CustomerID = forgotPinRequest.CustomerID;
                        userOtp.OTP = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(userOTP));
                        userOtp.OTPType = "Forgot PIN OTP";
                        userOtp.OTPExpDate = DateTime.Now.AddMinutes(otpTimeSpan);

                        otpRep.Save(userOtp);

                        if (forgotPinRequest.MailRequest != null)
                        {
                            forgotPinRequest.MailRequest.MailBody = Regex.Replace(forgotPinRequest.MailRequest.MailBody, @"#newOtp", userOTP);
                            forgotPinRequest.MailRequest.customer_id = forgotPinRequest.CustomerID;
                            new Task(() => { _mailService.SendMailToCustomer(forgotPinRequest.MailRequest); }).Start();
                        }

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
                        ResponseDescription = "invalid password",
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
            Logger.LogInfo("UserProfileService.ForgotPIN, response", response);

            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> ResetPin(ResetPinRequest resetPinRequest)
        {
            Logger.LogInfo("UserProfileService.ResetPin, input", resetPinRequest);
            Task<UserProfileResponse> response = null;

            try
            {
                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                ContextRepository<UserOTP> otpRep = new ContextRepository<UserOTP>();
                UserOTP userOtp = otpRep.Get(resetPinRequest.CustomerID);
                UserProfile verifingAccount = repository.Get(resetPinRequest.CustomerID);

                //validate token
                UserProfileResponse AuthenticateTokenResponse = new DefaultTokenManager().NonTransactionalTokenAuthentication(new AuthenticationData() { customer_id = resetPinRequest.CustomerID, AuthToken = resetPinRequest.AuthToken });

                if (AuthenticateTokenResponse.ResponseCode == "06")
                {
                    response = Task<UserProfileResponse>.Factory.StartNew(() =>
                    {
                        return new UserProfileResponse()
                        {
                            ResponseCode = "06",
                            ResponseDescription = AuthenticateTokenResponse.ResponseDescription,
                        };
                    });

                    Logger.LogInfo("UserProfileService.ResetPin, response", response.Result);

                    return response;
                }

                //token valid proceed to OTP validation
                if (userOtp != null)
                {
                    if (DateTime.Now <= userOtp.OTPExpDate)
                    {
                        if (PasswordStorage.VerifyPassword(resetPinRequest.userOTP.OTP, userOtp.OTP))
                        {
                            verifingAccount.PIN = PasswordStorage.CreateHash(resetPinRequest.NewPin);

                            repository.Update(verifingAccount);

                            response = Task<UserProfileResponse>.Factory.StartNew(() =>
                            {
                                return new UserProfileResponse()
                                {
                                    ResponseCode = "00",
                                    ResponseDescription = "SUCCESSFUL",
                                };
                            });
                        }
                        else
                        {
                            response = Task<UserProfileResponse>.Factory.StartNew(() =>
                            {
                                return new UserProfileResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "Invalid user OTP",
                                };
                            });
                        }
                    }
                    else
                    {
                        response = Task<UserProfileResponse>.Factory.StartNew(() =>
                        {
                            return new UserProfileResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "user OTP Expired",
                            };
                        });
                    }
                }
                else
                {
                    response = Task<UserProfileResponse>.Factory.StartNew(() =>
                    {
                        return new UserProfileResponse()
                        {
                            ResponseCode = "06",
                            ResponseDescription = "OTP not available for this user, re-initiate PIN reset",
                        };
                    });
                }



            }
            catch (Exception ex)
            {
                response = Task<UserProfileResponse>.Factory.StartNew(() =>
                {
                    return new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "FAILED: " + ex.Message,
                    };
                });
                Logger.LogError(ex);
            }

            Logger.LogInfo("UserProfileService.ResetPin, response", response.Result);
            return response;
        }
    }
}
