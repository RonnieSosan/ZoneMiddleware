using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.DefaultImplementation.AdminPortal
{
    public class ActiveDirectoryService : IUserService
    {
        string ADUsername = string.Empty;
        string ADPassword = string.Empty;
        string ADContainer = string.Empty;
        string ADServer = string.Empty;

        bool demo = false;
        public ActiveDirectoryService()
        {
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings.Get("IsDemo"), out demo);
            try
            {

                ADUsername = System.Configuration.ConfigurationManager.AppSettings.Get("ADUsername");
                ADPassword = System.Configuration.ConfigurationManager.AppSettings.Get("ADPassword");
                ADContainer = System.Configuration.ConfigurationManager.AppSettings.Get("ADContainer");
                ADServer = System.Configuration.ConfigurationManager.AppSettings.Get("ADServer");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("AD credentials not configured properly");
            }

        }

        public Task<ValidateADUserResponse> ValidateUser(ValidateADUserRequest user)
        {
            Logger.LogInfo("ADProcessor.ValidateUser, input: ", user.Username);

            Task<ValidateADUserResponse> response = null;
            string jsonResponse = string.Empty;
            if (demo)
            {
                if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
                {
                    response = Task<ValidateADUserResponse>.Factory.StartNew(() =>
                    {
                        return new ValidateADUserResponse
                        {
                            ResponseCode = "00",
                            ResponseDescription = "VALIDATION SUCCESSFUL"
                        };
                    });
                }
                else
                {
                    response = Task<ValidateADUserResponse>.Factory.StartNew(() =>
                    {
                        return new ValidateADUserResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "VALIDATION FAILED"
                        };
                    });
                }
                jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                Logger.LogInfo("ADProcessor.ValidateUser: response", jsonResponse);
                return response;
            }

            bool auth = false;
            try
            {
                PrincipalContext insPrincipalContext =
                    new PrincipalContext(ContextType.Domain, ADServer, ADContainer, ADUsername, ADPassword);

                UserPrincipal insUserPrincipal = new UserPrincipal(insPrincipalContext);
                auth = insPrincipalContext.ValidateCredentials(user.Username, user.Password);

                if (auth)
                {
                    response = Task<ValidateADUserResponse>.Factory.StartNew(() =>
                    {
                        return new ValidateADUserResponse
                        {
                            ResponseCode = "00",
                            ResponseDescription = "VALIDATION SUCCESSFUL"
                        };
                    });
                }
                else
                {
                    response = Task<ValidateADUserResponse>.Factory.StartNew(() =>
                    {
                        return new ValidateADUserResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "VALIDATION FAILED"
                        };
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                response = Task<ValidateADUserResponse>.Factory.StartNew(() =>
                {
                    return new ValidateADUserResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "VALIDATION FAILED" + ex.Message
                    };
                });
            }

            return response;
        }

        public Task<GetADUserResponse> GetUserDetails(GetADUserRequest input)
        {
            Logger.LogInfo("ADProcessor.GetUserDetails, input: ", input);


            Task<GetADUserResponse> response = null;
            string jsonResponse = string.Empty;

            if (demo)
            {
                response = Task<GetADUserResponse>.Factory.StartNew(() =>
                {
                    return new GetADUserResponse
                    {
                        UserName = input.UserName,
                        ResponseCode = "00",
                        ResponseDescription = "SUCCESSFUL"
                    };
                });

                jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                Logger.LogInfo("ADProcessor.GetUserDetails, response: ", jsonResponse);
                return response;
            }


            PrincipalContext insPrincipalContext = new PrincipalContext(ContextType.Domain, ADServer, ADContainer, ADUsername, ADPassword);

            UserPrincipal insUserPrincipal = new UserPrincipal(insPrincipalContext);
            try
            {
                insUserPrincipal.SamAccountName = input.UserName;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = Task<GetADUserResponse>.Factory.StartNew(() =>
                {
                    return new GetADUserResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "UNABLE TO GET USERNAME: " + ex.Message
                    };
                });
            }

            try
            {
                PrincipalSearcher searcher = new PrincipalSearcher(insUserPrincipal);
                UserPrincipal result = searcher.FindOne() as UserPrincipal;

                if (result != null)
                {
                    response = Task<GetADUserResponse>.Factory.StartNew(() =>
                    {
                        return new GetADUserResponse
                        {
                            UserName = input.UserName,
                            PhoneNumber = result.VoiceTelephoneNumber,
                            LastName = result.Surname,
                            OtherName = string.Format("{0} {1}", result.GivenName, result.MiddleName),
                            Email = result.EmailAddress,
                            EmployeeNumber = result.EmployeeId,
                            ResponseCode = "00",
                            ResponseDescription = "SUCCESSFUL"
                        };
                    });
                }
                else
                {
                    response = Task<GetADUserResponse>.Factory.StartNew(() =>
                    {
                        return new GetADUserResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "INVALID USERNAME"
                        };
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = Task<GetADUserResponse>.Factory.StartNew(() =>
                {
                    return new GetADUserResponse
                    {
                        ResponseCode = "06",
                        ResponseDescription = "UNABLE TO GET USERNAME: " + ex.Message
                    };
                });
            }

            jsonResponse = Newtonsoft.Json.JsonConvert.SerializeObject(response);
            Logger.LogInfo("ADProcessor.GetUserDetails, response: ", jsonResponse);
            return response;

        }
    }
}
