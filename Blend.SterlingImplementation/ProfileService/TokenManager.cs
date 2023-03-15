using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Extension;
using AppZoneMiddleware.Shared.Utility;
using Blend.SterlingImplementation.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SterlingImplementation.ProfileService
{
    public class TokenManager : ITokenManager
    {
        public string ZoneSecretKey = System.Configuration.ConfigurationManager.AppSettings.Get("ZoneSecretKey");
        public double tokenTimeSpan = Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings.Get("TokenLifeSpan"));

        public string ExtendToken(string CustomerId)
        {
            Logger.LogInfo("UserProfileService.ExtendToken, input CustID", CustomerId);
            try
            {
                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                UserProfile verifingAccount = repository.Get(CustomerId);

                if (verifingAccount == null)
                {
                    Logger.LogInfo("UserProfileService.ExtendToken, Response: ", "PROFILE NOT FOUND FOR CUSTOMER ID");
                    return "PROFILE NOT FOUND FOR CUSTOMER ID";
                }

                if (verifingAccount.TokenExpDate >= System.DateTime.Now)
                {
                    verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                    repository.Update(verifingAccount);
                }
                else
                {
                    Logger.LogInfo("UserProfileService.ExtendToken, Response: ", "TOKEN EXPIRED");
                    return "TOKEN EXPIRED";
                }
                return verifingAccount.TokenExpDate.ToString();
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("In UserProfileService.ExtendToken", ex));
                Logger.LogInfo("UserProfileService.ExtendToken, Response: ", "TOKEN NOT EXTENDED: " + ex.GetBaseException().Message);
                return "TOKEN NOT EXTENDED";
            }
        }

        public Task<UserProfileResponse> NonTransactionalTokenAuthentication(BaseRequest TokenValidationRequest)
        {
            Logger.LogInfo("UserProfileService.NonTransactionalTokenAuthentication, input", TokenValidationRequest);
            UserProfileResponse response = new UserProfileResponse() { ResponseCode = "00", ResponseDescription = "Valid Token", };

            try
            {
                //ABaseRequest authRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseRequest>(TokenValidationRequest);
                BaseRequest authRequest = TokenValidationRequest;
                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                UserProfile verifingAccount = repository.Get(authRequest.customer_id);

                if (authRequest.RequestChannel == "Zone")
                {
                    if (PasswordStorage.VerifyPassword(authRequest.Passkey, ZoneSecretKey))
                    {
                        response = new UserProfileResponse()
                        {
                            ResponseCode = "00",
                            ResponseDescription = "valid token",
                        };
                    }
                    else
                    {
                        response = new UserProfileResponse()
                        {
                            ResponseCode = "06",
                            ResponseDescription = "invalid token",
                        };
                    }
                }
                else
                {
                    if (verifingAccount != null)
                    {

                        if (verifingAccount.TokenExpDate >= System.DateTime.Now)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(authRequest.AuthToken) && PasswordStorage.VerifyPassword(authRequest.AuthToken, verifingAccount.AuthToken))
                                {
                                    verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                                    repository.Update(verifingAccount);
                                    response = new UserProfileResponse()
                                    {
                                        ResponseCode = "00",
                                        ResponseDescription = "valid token",
                                    };
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(authRequest.PIN))    // HINT: Verify ONLY PIN since AuthToken verification failed
                                    {
                                        #region Verify ONLY PIN since AuthToken verification failed
                                        Logger.LogInfo("UserProfileService.TransactionalTokenAuthentication, Verify ONLY PIN since AuthToken verification failed. both pins: ", authRequest.PIN + "  " + verifingAccount.PIN);
                                        try
                                        {
                                            if (PasswordStorage.VerifyPassword(authRequest.PIN, verifingAccount.PIN))
                                            {
                                                verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                                                repository.Update(verifingAccount);
                                                response = new UserProfileResponse()
                                                {
                                                    ResponseCode = "00",
                                                    ResponseDescription = "account authenticated",
                                                };
                                            }
                                            else
                                            {
                                                // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                                response = new UserProfileResponse()
                                                {
                                                    ResponseCode = "06",
                                                    ResponseDescription = "invalid PIN",
                                                };
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.LogError(new Exception("In TransactionalTokenAuthentication. Verify ONLY PIN since AuthToken verification failed", ex));
                                            // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                            response = new UserProfileResponse()
                                            {
                                                ResponseCode = "06",
                                                ResponseDescription = "Invalid Pin: " + ex.Message,
                                            };
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        response = new UserProfileResponse()
                                        {
                                            ResponseCode = "06",
                                            ResponseDescription = "invalid token",
                                        };
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(new Exception("In NonTransactionalTokenAuthentication", ex));
                                response = new UserProfileResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "Could not verify token.",
                                };
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(authRequest.PIN))    // HINT: Verify ONLY PIN since AuthToken has expired
                            {
                                #region Verify ONLY PIN since AuthToken has expired
                                Logger.LogInfo("UserProfileService.TransactionalTokenAuthentication, Verify ONLY PIN since AuthToken has expired. both pins: ", authRequest.PIN + "  " + verifingAccount.PIN);
                                try
                                {
                                    if (PasswordStorage.VerifyPassword(authRequest.PIN, verifingAccount.PIN))
                                    {
                                        verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                                        repository.Update(verifingAccount);
                                        response = new UserProfileResponse()
                                        {
                                            ResponseCode = "00",
                                            ResponseDescription = "account authenticated",
                                        };
                                    }
                                    else
                                    {
                                        // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                        //response = new UserProfileResponse()
                                        //{
                                        //    ResponseCode = "06",
                                        //    ResponseDescription = "invalid PIN",
                                        //};
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(new Exception("In TransactionalTokenAuthentication. Verify ONLY PIN since AuthToken has expired", ex));
                                    // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                    //response = new UserProfileResponse()
                                    //{
                                    //    ResponseCode = "06",
                                    //    ResponseDescription = "Invalid Pin: " + ex.Message,
                                    //};
                                }
                                #endregion
                            }
                            else
                            {
                                response = new UserProfileResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "token expired",
                                };
                            }
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
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("In NonTransactionalTokenAuthentication", ex));
                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
            }
            Logger.LogInfo("UserProfileService.NonTransactionalTokenAuthentication, response", response);

            return Task.Factory.StartNew(() => response);
        }

        public Task<UserProfileResponse> TransactionalTokenAuthentication(BaseRequest TokenValidationRequest)
        {
            Logger.LogInfo("UserProfileService.TransactionalTokenAuthentication, input", TokenValidationRequest);
            UserProfileResponse response = new UserProfileResponse() { ResponseCode = "00", ResponseDescription = "account authenticated", };

            try
            {
                //BaseRequest authRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseRequest>(TokenValidationRequest);
                BaseRequest authRequest = TokenValidationRequest;

                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                UserProfile verifingAccount = repository.Get(authRequest.customer_id);

                if (verifingAccount != null)
                {
                    if (authRequest.RequestChannel == "Zone")
                    {
                        if (PasswordStorage.VerifyPassword(authRequest.Passkey, ZoneSecretKey))
                        {
                            response = new UserProfileResponse()
                            {
                                ResponseCode = "00",
                                ResponseDescription = "valid token",
                            };
                        }
                        else
                        {
                            response = new UserProfileResponse()
                            {
                                ResponseCode = "06",
                                ResponseDescription = "invalid token",
                            };
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(authRequest.AuthToken) && PasswordStorage.VerifyPassword(authRequest.AuthToken, verifingAccount.AuthToken))
                        {
                            if (verifingAccount.TokenExpDate >= System.DateTime.Now)
                            {
                                Logger.LogInfo("UserProfileService.TransactionalTokenAuthentication, both pins", authRequest.PIN + "  " + verifingAccount.PIN);
                                try
                                {
                                    if (PasswordStorage.VerifyPassword(authRequest.PIN, verifingAccount.PIN))
                                    {
                                        verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                                        repository.Update(verifingAccount);
                                        response = new UserProfileResponse()
                                        {
                                            ResponseCode = "00",
                                            ResponseDescription = "account authenticated",
                                        };
                                    }
                                    else
                                    {
                                        // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                        //response = new UserProfileResponse()
                                        //{
                                        //    ResponseCode = "06",
                                        //    ResponseDescription = "invalid PIN",
                                        //};
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(new Exception("In TransactionalTokenAuthentication", ex));
                                    // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                    //response = new UserProfileResponse()
                                    //{
                                    //    ResponseCode = "06",
                                    //    ResponseDescription = "Invalid Pin: " + ex.Message,
                                    //};
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(authRequest.PIN))    // HINT: Verify ONLY PIN since AuthToken has expired
                                {
                                    #region Verify ONLY PIN since AuthToken has expired
                                    Logger.LogInfo("UserProfileService.TransactionalTokenAuthentication, Verify ONLY PIN since AuthToken has expired. both pins: ", authRequest.PIN + "  " + verifingAccount.PIN);
                                    try
                                    {
                                        if (PasswordStorage.VerifyPassword(authRequest.PIN, verifingAccount.PIN))
                                        {
                                            verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                                            repository.Update(verifingAccount);
                                            response = new UserProfileResponse()
                                            {
                                                ResponseCode = "00",
                                                ResponseDescription = "account authenticated",
                                            };
                                        }
                                        else
                                        {
                                            // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                            //response = new UserProfileResponse()
                                            //{
                                            //    ResponseCode = "06",
                                            //    ResponseDescription = "invalid PIN",
                                            //};
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogError(new Exception("In TransactionalTokenAuthentication. Verify ONLY PIN since AuthToken has expired", ex));
                                        // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                        //response = new UserProfileResponse()
                                        //{
                                        //    ResponseCode = "06",
                                        //    ResponseDescription = "Invalid Pin: " + ex.Message,
                                        //};
                                    }
                                    #endregion
                                }
                                else
                                {
                                    response = new UserProfileResponse()
                                    {
                                        ResponseCode = "06",
                                        ResponseDescription = "token expired",
                                    };
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(authRequest.PIN))    // HINT: Verify ONLY PIN since AuthToken verification failed
                            {
                                #region Verify ONLY PIN since AuthToken verification failed
                                Logger.LogInfo("UserProfileService.TransactionalTokenAuthentication, Verify ONLY PIN since AuthToken verification failed. both pins: ", authRequest.PIN + "  " + verifingAccount.PIN);
                                try
                                {
                                    if (PasswordStorage.VerifyPassword(authRequest.PIN, verifingAccount.PIN))
                                    {
                                        verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                                        repository.Update(verifingAccount);
                                        response = new UserProfileResponse()
                                        {
                                            ResponseCode = "00",
                                            ResponseDescription = "account authenticated",
                                        };
                                    }
                                    else
                                    {
                                        // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                        response = new UserProfileResponse()
                                        {
                                            ResponseCode = "06",
                                            ResponseDescription = "invalid PIN",
                                        };
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(new Exception("In TransactionalTokenAuthentication. Verify ONLY PIN since AuthToken verification failed", ex));
                                    // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                    response = new UserProfileResponse()
                                    {
                                        ResponseCode = "06",
                                        ResponseDescription = "Invalid Pin: " + ex.Message,
                                    };
                                }
                                #endregion 
                            }
                            else
                            {
                                response = new UserProfileResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "invalid token",
                                };
                            }
                        }
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
                Logger.LogError(new Exception("In TransactionalTokenAuthentication", ex));
                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
            }

            Logger.LogInfo("UserProfileService.TransactionalTokenAuthentication, response", response);

            return Task.Factory.StartNew(() => response);
        }

        public UserProfileResponse NonTransactionalTokenAuthentication(string TokenValidationRequest)
        {
            Logger.LogInfo("UserProfileService.NonTransactionalTokenAuthentication, input", TokenValidationRequest);
            UserProfileResponse response = new UserProfileResponse() { ResponseCode = "00", ResponseDescription = "Valid Token", };

            try
            {
                BaseRequest authRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseRequest>(TokenValidationRequest);

                ContextRepository<UserProfile> repository = new ContextRepository<UserProfile>();
                UserProfile verifingAccount = repository.Get(authRequest.customer_id);

                if (authRequest.RequestChannel == "Zone")
                {
                    if (PasswordStorage.VerifyPassword(authRequest.Passkey, ZoneSecretKey))
                    {
                        response = new UserProfileResponse()
                        {
                            ResponseCode = "00",
                            ResponseDescription = "valid token",
                        };
                    }
                    else
                    {
                        response = new UserProfileResponse()
                        {
                            ResponseCode = "06",
                            ResponseDescription = "invalid token",
                        };
                    }
                }
                else
                {
                    if (verifingAccount != null)
                    {

                        if (verifingAccount.TokenExpDate >= System.DateTime.Now)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(authRequest.AuthToken) && PasswordStorage.VerifyPassword(authRequest.AuthToken, verifingAccount.AuthToken))
                                {
                                    verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                                    repository.Update(verifingAccount);
                                    response = new UserProfileResponse()
                                    {
                                        ResponseCode = "00",
                                        ResponseDescription = "valid token",
                                    };
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(authRequest.PIN))    // HINT: Verify ONLY PIN since AuthToken verification failed
                                    {
                                        #region Verify ONLY PIN since AuthToken verification failed
                                        Logger.LogInfo("UserProfileService.TransactionalTokenAuthentication, Verify ONLY PIN since AuthToken verification failed. both pins: ", authRequest.PIN + "  " + verifingAccount.PIN);
                                        try
                                        {
                                            if (PasswordStorage.VerifyPassword(authRequest.PIN, verifingAccount.PIN))
                                            {
                                                verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                                                repository.Update(verifingAccount);
                                                response = new UserProfileResponse()
                                                {
                                                    ResponseCode = "00",
                                                    ResponseDescription = "account authenticated",
                                                };
                                            }
                                            else
                                            {
                                                // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                                response = new UserProfileResponse()
                                                {
                                                    ResponseCode = "06",
                                                    ResponseDescription = "invalid PIN",
                                                };
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.LogError(new Exception("In TransactionalTokenAuthentication. Verify ONLY PIN since AuthToken verification failed", ex));
                                            // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                            response = new UserProfileResponse()
                                            {
                                                ResponseCode = "06",
                                                ResponseDescription = "Invalid Pin: " + ex.Message,
                                            };
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        response = new UserProfileResponse()
                                        {
                                            ResponseCode = "06",
                                            ResponseDescription = "invalid token",
                                        };
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError(new Exception("In NonTransactionalTokenAuthentication", ex));
                                response = new UserProfileResponse()
                                {
                                    ResponseCode = "00",
                                    ResponseDescription = "Could not verify token.",
                                };
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(authRequest.PIN))    // HINT: Verify ONLY PIN since AuthToken has expired
                            {
                                #region Verify ONLY PIN since AuthToken has expired
                                Logger.LogInfo("UserProfileService.TransactionalTokenAuthentication, Verify ONLY PIN since AuthToken has expired. both pins: ", authRequest.PIN + "  " + verifingAccount.PIN);
                                try
                                {
                                    if (PasswordStorage.VerifyPassword(authRequest.PIN, verifingAccount.PIN))
                                    {
                                        verifingAccount.TokenExpDate = DateTime.Now.AddMinutes(tokenTimeSpan);
                                        repository.Update(verifingAccount);
                                        response = new UserProfileResponse()
                                        {
                                            ResponseCode = "00",
                                            ResponseDescription = "account authenticated",
                                        };
                                    }
                                    else
                                    {
                                        // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                        //response = new UserProfileResponse()
                                        //{
                                        //    ResponseCode = "06",
                                        //    ResponseDescription = "invalid PIN",
                                        //};
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Logger.LogError(new Exception("In TransactionalTokenAuthentication. Verify ONLY PIN since AuthToken has expired", ex));
                                    // HINT: Below was commented out based on Sterling's requirement/request that PIN authentication should be removed.
                                    //response = new UserProfileResponse()
                                    //{
                                    //    ResponseCode = "06",
                                    //    ResponseDescription = "Invalid Pin: " + ex.Message,
                                    //};
                                }
                                #endregion
                            }
                            else
                            {
                                response = new UserProfileResponse()
                                {
                                    ResponseCode = "06",
                                    ResponseDescription = "token expired",
                                };
                            }
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
            }
            catch (Exception ex)
            {
                Logger.LogError(new Exception("In TransactionalTokenAuthentication", ex));
                response = new UserProfileResponse()
                {
                    ResponseCode = "06",
                    ResponseDescription = "FAILED: " + ex.Message,
                };
            }
            Logger.LogInfo("UserProfileService.NonTransactionalTokenAuthentication, response", response);

            return response;
        }
    }
}
