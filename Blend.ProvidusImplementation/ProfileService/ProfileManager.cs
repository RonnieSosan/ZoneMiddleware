using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using Blend.ProvidusImplementation.Persistence;
using AppZoneMiddleware.Shared.Utility;
using System.Text.RegularExpressions;
using AppZoneMiddleware.Shared.Extension;
using Blend.ProvidusImplementation.NotificationService;
using Blend.ProvidusImplementation.CoreBankingService;
using Newtonsoft.Json.Linq;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;

namespace Blend.ProvidusImplementation.ProfileService
{
    public class ProfileManager : IProfileManager
    {
        public string ZoneSecretKey = System.Configuration.ConfigurationManager.AppSettings.Get("ZoneSecretKey");
        public double otpTimeSpan = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings.Get("OTPLifeSpan"));

        public Task<LoggedInUsersResponse> GetCurrentLoggedInUsers()
        {
            {
                Logger.LogInfo("UserProfileService.GetCurrentLoggedInUsers ", "Initiated");
                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                LoggedInUsersResponse response = null;

                try
                {
                    //Get all currently logged in users by checking for token expiry date from the current syhstem DateTime
                    List<UserProfile> loggedInUsers = repository.Get().Where(x => x.TokenExpDate >= DateTime.Now).ToList();


                    if (loggedInUsers == null || loggedInUsers.Count > 0)
                    {
                        response = new LoggedInUsersResponse()
                        {
                            LoggedInUsers = loggedInUsers,
                            ResponseCode = "00",
                            ResponseDescription = "SUCCESSFUL"
                        };
                    }
                    else
                    {
                        response = new LoggedInUsersResponse()
                        {
                            ResponseCode = "01",
                            ResponseDescription = "No User Currently Logged in"
                        };
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    response = new LoggedInUsersResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "FAILED: " + ex.Message
                    };
                }

                Logger.LogInfo("UserProfileService.GetCurrentLoggedInUsers, response", response);
                return Task<LoggedInUsersResponse>.Factory.StartNew(() => response);
            }
        }

        public Task<UserProfileResponse> InitiateProfileRegistration(UserAccount profileToRegister)
        {
            Logger.LogInfo("UserProfileService.InitiateProfileRegistration, input", profileToRegister);
            UserProfileResponse response = null;
            try
            {

                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                ContextRepository<UserOTP> OTPRepository = new ContextRepository<UserOTP>();
                string userOTP = new PasswordStorage().generateOTP();
                UserOTP otpUser = null;

                //send otp for open account customer without account id
                UserProfile users = repository.Get(profileToRegister.CustomerID);
                otpUser = OTPRepository.Get(profileToRegister.CustomerID);

                if (users == null)
                {
                    if (otpUser == null)
                    {
                        otpUser = new UserOTP()
                        {
                            CustomerID = profileToRegister.CustomerID,
                            OTP = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(userOTP)),
                            OTPType = "Account Verification Code",
                            OTPExpDate = DateTime.Now.AddMinutes(otpTimeSpan),
                        };
                        OTPRepository.Save(otpUser);
                    }
                    else
                    {
                        otpUser.OTP = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(userOTP));
                        otpUser.OTPExpDate = DateTime.Now.AddMinutes(otpTimeSpan);

                        OTPRepository.Update(otpUser);
                    }

                    Logger.LogInfo("UserProfileService.InitiateProfileRegistration, User OTP", userOTP);
                    if (profileToRegister.MailRequest != null)
                    {
                        profileToRegister.MailRequest.MailBody = Regex.Replace(profileToRegister.MailRequest.MailBody, @"#newOtp", userOTP);
                        profileToRegister.MailRequest.MailBody = Regex.Replace(profileToRegister.MailRequest.MailBody, @"#expiredOtpTime", otpUser.OTPExpDate.ToString());

                        //validate emailaddress
                        UserProfileResponse mailValidation = new MailService().ValidateEmailAddress(profileToRegister.CustomerID);
                        if (mailValidation.ResponseCode == "00")
                        {
                            //return Newtonsoft.Json.JsonConvert.SerializeObject(mailValidation);
                            profileToRegister.MailRequest.customer_id = otpUser.CustomerID;
                            new MailService().SendMailToCustomer(profileToRegister.MailRequest);
                        }

                        var sms = string.Format("Verification Code: {0}", userOTP);

                        var smsService = new SMSSender();

                        smsService.Send(new SMSRequest { customer_id = profileToRegister.CustomerID, Message = sms });
                    }
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "00",
                        ResponseDescription = "User otp created successfully"
                    };
                }
                else
                {
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Customer ID already exists"
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
            };

            Logger.LogInfo("UserProfileService.InitiateProfileRegistration, response", response);

            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> LockProfile(string CustomerID)
        {
            Logger.LogInfo("UserProfileService.lockProfile, input", CustomerID);
            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            UserProfileResponse response = null;
            try
            {
                UserProfile userProfile = repository.Get(CustomerID);
                if (userProfile != null)
                {
                    userProfile.IsLocked = true;

                    repository.Update(userProfile);

                    response = new UserProfileResponse()
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Profile successfully locked",
                    };
                }
                else
                {
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Invalid Customer ID",
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
            }

            Logger.LogInfo("UserProfileService.lockProfile, response", response);

            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> RegisterProfile(string profileToRegister)
        {
            Logger.LogInfo("UserProfileService.RegisterProfile, input", profileToRegister);
            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            ContextRepository<UserOTP> otpRep = new ContextRepository<UserOTP>();
            UserProfileResponse response = null;

            try
            {
                UserProfile ProfiletoRegister = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(profileToRegister);
                UserAccount ProfileFromService = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(profileToRegister);

                UserProfile users = repository.Get(ProfiletoRegister.CustomerID);
                UserOTP userOtp = otpRep.Get(ProfiletoRegister.CustomerID);

                if (userOtp != null)
                {
                    if (DateTime.Now <= userOtp.OTPExpDate)
                    {

                        Logger.LogInfo("UserProfileService.RegisterProfile, Saved userOTP", PasswordStorage.CreateHash(ProfileFromService.userOTP.OTP));
                        if (PasswordStorage.VerifyPassword(ProfileFromService.userOTP.OTP, userOtp.OTP))
                        {
                            if (users == null)
                            {
                                ProfiletoRegister.AuthToken = ProfiletoRegister.AuthToken;
                                ProfiletoRegister.TokenExpDate = DateTime.Now.AddMinutes(5);
                                ProfiletoRegister.Password = PasswordStorage.CreateHash(ProfiletoRegister.Password);
                                ProfiletoRegister.PIN = PasswordStorage.CreateHash(ProfiletoRegister.PIN);
                                ProfiletoRegister.IsLocked = false;
                                ProfiletoRegister.InvalidTrials = 0;

                                repository.Save(ProfiletoRegister);

                                if (ProfileFromService.MailRequest != null)
                                {
                                    ProfileFromService.MailRequest.customer_id = ProfileFromService.CustomerID;
                                    MailResponse mailResponse = new MailService().SendMailToCustomer(ProfileFromService.MailRequest).Result;

                                    Logger.LogInfo("UserProfileService.RegisterProfile, serialized user accounts from mail", mailResponse);

                                    response = new UserProfileResponse()
                                    {
                                        AccountInformation = new CoreBankingService.AccountInquiryService().GetAccountsWithCustomerID(new AccountRequest { CustomerID = ProfiletoRegister.CustomerID }).Result.AccountInformation,
                                    };
                                }

                                response.ResponseCode = "00";
                                response.ResponseDescription = "User profiling successful";
                            }
                            else
                            {
                                response = new UserProfileResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "Customer ID already exists"
                                };
                            }
                        }
                        else
                        {
                            response = new UserProfileResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "User OTP Invalid"
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
                        ResponseDescription = "User has not requested for verification code"
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
            };

            Logger.LogInfo("UserProfileService.RegisterProfile, response", response);
            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> UnlockProfile(string CustorerID)
        {
            Logger.LogInfo("UserProfileService.lockProfile, input", CustorerID);
            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            UserProfileResponse response = null;
            try
            {
                UserProfile userProfile = repository.Get(CustorerID);
                if (userProfile != null)
                {
                    userProfile.IsLocked = true;

                    repository.Update(userProfile);

                    response = new UserProfileResponse()
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Profile successfully locked",
                    };
                }
                else
                {
                    response = new UserProfileResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "Invalid Customer ID",
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
            }

            Logger.LogInfo("UserProfileService.lockProfile, response", response);

            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public Task<UserLoginResponse> UserLogin(UserLoginRequest loginRequest)
        {
            Logger.LogInfo("UserProfileService.UserLogin, input: ", loginRequest);
            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            UserLoginResponse response = null;
            try
            {
                UserProfile verifyingAccount = repository.Get(loginRequest.customer_id);

                if (verifyingAccount != null)
                {
                    if (loginRequest.RequestChannel == "Zone")
                    {
                        Logger.LogInfo("UserProFileService.UserLogin, both passwords: ", verifyingAccount.Password + " " + ZoneSecretKey);
                        if (PasswordStorage.VerifyPassword(loginRequest.Passkey, ZoneSecretKey))
                        {
                            new UserLoginResponse()
                            {
                                ResponseCode = "00",
                                ResponseDescription = "valid token",
                            };
                        }
                        else
                        {
                            response = new UserLoginResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "invalid token",
                            };
                        }
                    }
                    else
                    {
                        if (!verifyingAccount.IsLocked)
                        {
                            Logger.LogInfo("UserProFileService.UserLogin, both passwords: ", verifyingAccount.Password + " " + loginRequest.Passkey);
                            if (PasswordStorage.VerifyPassword(loginRequest.Passkey, verifyingAccount.Password))
                            {
                                string authToken = new PasswordStorage().generateOTP();
                                verifyingAccount.AuthToken = PasswordStorage.CreateHash(authToken);
                                verifyingAccount.TokenExpDate = DateTime.Now.AddMinutes(otpTimeSpan);
                                verifyingAccount.InvalidTrials = 0;
                                verifyingAccount.IsLocked = false;

                                repository.Update(verifyingAccount);

                                AccountRequest acctReq = new AccountRequest()
                                {
                                    CustomerID = loginRequest.customer_id,
                                };

                                List<AccountDetails> userAccounts = new CoreBankingService.AccountInquiryService().GetAccountsWithCustomerID(acctReq).Result.AccountInformation.ToList();

                                response = new UserLoginResponse()
                                {
                                    ResponseCode = "00",
                                    ResponseDescription = "SUCCESSFUL",
                                    AuthToken = authToken,
                                    AccountInformation = userAccounts
                                };
                            }
                            else
                            {
                                verifyingAccount.InvalidTrials += 1;
                                verifyingAccount.IsLocked = verifyingAccount.InvalidTrials == 3 ? true : false;
                                repository.Update(verifyingAccount);

                                response = new UserLoginResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "invalid password",
                                };
                            }
                        }
                        else
                        {
                            response = new UserLoginResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "Profile has been Locked",
                            };
                        }
                    }

                }
                else
                {
                    response = new UserLoginResponse()
                    {
                        ResponseCode = "06",
                        ResponseDescription = "User not found",
                    };
                }
            }
            catch (Exception ex)
            {

                response = new UserLoginResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILURE: " + ex.Message,
                };
            }

            Logger.LogInfo("UserProfileService.UserLogin, response", response);
            return Task<UserLoginResponse>.Factory.StartNew(() => response);

        }

        public Task<UserProfileResponse> ValidateProfile(UserProfile validateProfileRequest)
        {
            throw new NotImplementedException();
        }
        public Task<JObject> ProxyLogin(ApiProxyRequest loginRequest)
        {
            throw new NotImplementedException();
        }
    }
}
