using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Extension;
using AppZoneMiddleware.Shared.Utility;
using Blend.SharedServiceImplementation;
using Blend.SterlingImplementation.Entites;
using Blend.SterlingImplementation.NotificationService;
using Blend.SterlingImplementation.Persistence;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.ProfileService
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
                        //UserProfileResponse mailValidation = new MailService().ValidateEmailAddress(profileToRegister.CustomerID);
                        UserProfileResponse mailValidation = new UserProfileResponse();
                        mailValidation.ResponseCode = "00";
                        if (mailValidation.ResponseCode == "00")
                        {
                            //return Newtonsoft.Json.JsonConvert.SerializeObject(mailValidation);
                            profileToRegister.MailRequest.customer_id = otpUser.CustomerID;
                            // new MailService().SendMailToCustomer(profileToRegister.MailRequest);
                            Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(profileToRegister.MailRequest)));
                        }

                        var sms = string.Format("Verification Code: {0}", userOTP);
                        var smsService = new SMSSender();
                        var request = new SMSRequest { PhoneNumber = profileToRegister.CustomerID, Message = sms };
                        Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendSMS").CallService<SMSResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(request)));
                        //smsService.Send(new SMSRequest { PhoneNumber = profileToRegister.CustomerID, Message = sms });
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
                    ResponseDescription = "Unable to process request",
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

        public Task<UserProfileResponse> RegisterProfile(string input)
        {
            Logger.LogInfo("UserProfileService.RegisterProfile, input", input);
            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            ContextRepository<UserOTP> otpRep = new ContextRepository<UserOTP>();
            UserProfileResponse response = null;
            CreateWalletRequest walletAccountRequest = null;
            try
            {
                UserProfile ProfiletoRegister = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfile>(input);
                UserAccount ProfileFromService = Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(input);

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

                                if (repository.Get(ProfiletoRegister.CustomerID) != null)
                                {
                                    if (ProfileFromService.CreateWalletRequest != null)
                                    {

                                        //#region Old Account Opening request
                                        //var address = ProfileFromService.c.address_line1;
                                        //if (string.IsNullOrWhiteSpace(address))
                                        //{
                                        //    address = "Sterling Bank, Marina, Lagos.";
                                        //    ProfileFromService.OpenAccountRequest.address_line1 = address;
                                        //}
                                        //var theTitle = ProfileFromService.OpenAccountRequest.Title;
                                        //if (string.IsNullOrWhiteSpace(theTitle))
                                        //{
                                        //    theTitle = "Mr";
                                        //}
                                        //theTitle = theTitle.Trim().Replace(".", string.Empty);
                                        //ProfileFromService.OpenAccountRequest.gender = string.Equals(theTitle, "Mr", StringComparison.InvariantCultureIgnoreCase) ? "M" : "F";
                                        //#endregion
                                    }

                                    if (!ProfileFromService.IsExistingAccountHolder)
                                    {
                                        Logger.LogInfo("RegisterProfile [About to Open Wallet Account for]. ", ProfiletoRegister);
                                        string Response = new AppzoneApiProcessor("Blend", "WalletService", "OpenAccount").CallService<WalletResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(ProfileFromService.CreateWalletRequest));
                                        //OpenAccountResponse openAccountResponse = new AccountServices().OpenAccount(ProfileFromService.AccountRequest).Result;
                                        WalletResponse openAccountResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WalletResponse>(Response);
                                        if (openAccountResponse.ResponseCode == "00")
                                        {
                                            string responseForAccountValidation = new AppzoneApiProcessor("Blend", "WalletService", "GetWalletDetails").CallService<WalletDetails>(Newtonsoft.Json.JsonConvert.SerializeObject(new GetWalletDetails { nuban = openAccountResponse.data.AccountNumber, Translocation = ProfileFromService.CreateWalletRequest.Translocation }));
                                            WalletDetails account = Newtonsoft.Json.JsonConvert.DeserializeObject<WalletDetails>(responseForAccountValidation);
                                            if (account.ResponseCode == "00")
                                            {
                                                response = new UserProfileResponse()
                                                {
                                                    WalletDetails = account.data,
                                                    ResponseCode = "00",
                                                    ResponseDescription = "User profiling successful"
                                                };

                                                if (ProfileFromService.MailRequest != null)
                                                {
                                                    ProfileFromService.MailRequest.customer_id = ProfileFromService.CustomerID;
                                                    if (account.data != null)
                                                        ProfileFromService.MailRequest.AccountNumber = account.data.AccountNumber;
                                                    ProfileFromService.MailRequest.MailBody = ProfileFromService.MailRequest.MailBody.Replace("#accountNumber", ProfileFromService.MailRequest.AccountNumber);
                                                    string mailResponse = new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(ProfileFromService.MailRequest));
                                                    //MailResponse mailResponse = new MailService().SendMailToCustomer(ProfileFromService.MailRequest).Result;
                                                    Logger.LogInfo("UserProfileService.RegisterProfile, serialized user accounts from mail", mailResponse);
                                                }
                                            }
                                            else
                                            {
                                                repository.Delete(ProfiletoRegister);
                                                response = new UserProfileResponse()
                                                {
                                                    ResponseCode = "06",
                                                    ResponseDescription = "Account Validation Failed"
                                                };
                                            }

                                        }
                                        else
                                        {
                                            repository.Delete(ProfiletoRegister);
                                            response = new UserProfileResponse()
                                            {
                                                ResponseCode = "06",
                                                ResponseDescription = "Account Creation Failed"
                                            };
                                        }
                                    }
                                    else
                                    {
                                        response = new UserProfileResponse()
                                        {
                                            ResponseCode = "00",
                                            ResponseDescription = "User profiling successful"
                                        };

                                        if (ProfileFromService.MailRequest != null)
                                        {
                                            ProfileFromService.MailRequest.customer_id = ProfileFromService.CustomerID;
                                            ProfileFromService.MailRequest.MailBody = ProfileFromService.MailRequest.MailBody.Replace("#accountNumber", "Still Your Existing Account Number(s)");
                                            string mailResponse = new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(ProfileFromService.MailRequest));
                                            //MailResponse mailResponse = new MailService().SendMailToCustomer(ProfileFromService.MailRequest).Result;
                                            Logger.LogInfo("UserProfileService.RegisterProfile [Existing Account Holder], serialized user accounts from mail", mailResponse);
                                        }
                                        Logger.LogInfo("UserProfileService.RegisterProfile [Existing Account Holder] completed for. ", ProfiletoRegister);
                                    }
                                }
                                else
                                {
                                    repository.Delete(ProfiletoRegister);
                                    response = new UserProfileResponse()
                                    {
                                        ResponseCode = "06",
                                        ResponseDescription = "Could not create profile at the moment."
                                    };
                                }
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
                    ResponseDescription = "Unable to register profile",
                };
                Logger.LogError(ex);
            };

            Logger.LogInfo("UserProfileService.RegisterProfile, response", response);
            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> UnlockProfile(string CustorerID)
        {
            Logger.LogInfo("UserProfileService.unlock, input", CustorerID);
            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            UserProfileResponse response = null;
            try
            {
                UserProfile userProfile = repository.Get(CustorerID);
                if (userProfile != null)
                {
                    userProfile.IsLocked = false;
                    userProfile.InvalidTrials = 0;
                    repository.Update(userProfile);

                    response = new UserProfileResponse()
                    {
                        ResponseCode = "00",
                        ResponseDescription = "Profile successfully Unlock",
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

            Logger.LogInfo("UserProfileService.unlockProfile, response", response);

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
                            response = new UserLoginResponse()
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
                                verifyingAccount.AuthToken = PasswordStorage.CreateHash(new PasswordStorage().CryptoHash(authToken));
                                verifyingAccount.TokenExpDate = DateTime.Now.AddMinutes(otpTimeSpan);
                                verifyingAccount.InvalidTrials = 0;
                                verifyingAccount.IsLocked = false;

                                repository.Update(verifyingAccount);

                                if (loginRequest.MailRequest != null)
                                {
                                    Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Notification", "SendMail").CallService<MailResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(loginRequest.MailRequest)));
                                }

                                response = new UserLoginResponse()
                                {
                                    ResponseCode = "00",
                                    ResponseDescription = "SUCCESSFUL",
                                    AuthToken = new PasswordStorage().CryptoHash(authToken),
                                    AccountInformation = null,
                                    WasPasswordRecentlyReset = verifyingAccount.WasPasswordRecentlyReset,
                                };
                            }
                            else
                            {
                                verifyingAccount.InvalidTrials += 1;
                                verifyingAccount.IsLocked = verifyingAccount.InvalidTrials == 3 ? true : false;
                                repository.Update(verifyingAccount);

                                string trialsLeft = (3 - verifyingAccount.InvalidTrials).ToString();
                                response = new UserLoginResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = string.Format("Invalid Password, {0} trials left ", trialsLeft),
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
                Logger.LogError(ex);
                response = new UserLoginResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "Login Failed",
                };
            }

            Logger.LogInfo("UserProfileService.UserLogin, response", response);
            return Task<UserLoginResponse>.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> ValidateOTP(ValidateOTP request)
        {
            Logger.LogInfo("UserProfileService.ValidateOTP, input", request);

            ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
            ContextRepository<UserOTP> otpRep = new ContextRepository<UserOTP>();
            UserProfileResponse response = null;

            try
            {
                UserOTP userOtp = otpRep.Get(request.CustomerID);
                UserProfile user = repository.Get(request.CustomerID);
                if (userOtp != null)
                {
                    if (DateTime.Now <= userOtp.OTPExpDate)
                    {

                        if (PasswordStorage.VerifyPassword(userOtp.OTP, request.OTP))
                        {
                            if (user == null)
                            {
                                response = new UserProfileResponse()
                                {
                                    ResponseCode = "00",
                                    ResponseDescription = "User OTP Valid"
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
                Logger.LogError(ex);
                response = new UserProfileResponse()
                {
                    ResponseCode = "MW96",
                    ResponseDescription = "unable to process request"
                };
            }

            Logger.LogInfo("UserProfileService.ValidateOTP, response", response);
            return Task<UserProfileResponse>.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> ValidateOTP(UserOTP userOTPrequest)
        {
            throw new NotImplementedException();
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
