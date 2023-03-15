using AppZoneMiddleware.Shared.Contracts.ProxyAuthentication;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Utility;
using Blend.DefaultImplementation.Persistence;
using Blend.DefaultImplementation.ProfileService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.DefaultImplementation
{
    public class DefaultProxyAuthentication : IProxyRequestAuthentication
    {
        /// <summary>
        /// Sefault implementation to Authenticate proxy api requests
        /// </summary>
        /// <param name="proxyRequest"></param>
        /// <returns></returns>
        public Task<ApiProxyResponse> RunAuthentication(ApiProxyRequest proxyRequest)
        {
            Logger.LogInfo("ProxyRequestAuthentication.RunAuthentication.Input", proxyRequest);
            ApiProxyResponse response = null;
            UserProfileResponse authResponse = null;
            try
            {
                ContextRepository<ApiSecuritySpec> repository = new ContextRepository<ApiSecuritySpec>();
                ApplicationDbContext context = new ApplicationDbContext();

                ApiSecuritySpec securitySpec = context.APISecuritySpecs.SqlQuery(string.Format("select * from APISecuritySpecs where URL like '%{0}%' ", proxyRequest.EndPointURL.Trim())).ToList().FirstOrDefault();
                if (securitySpec != null)
                {
                    string[] authentications = securitySpec.SecurityMetric.Split(new Char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    Logger.LogInfo("ProxyRequestAuthentication.RunAuthentication.tracker", authentications);
                    if (authentications.Contains("Token"))
                    {
                        authResponse = new DefaultTokenManager().NonTransactionalTokenAuthentication(proxyRequest.AuthData);
                        response = new ApiProxyResponse
                        {
                            ResponseCode = authResponse.ResponseCode,
                            ResponseDescription = authResponse.ResponseDescription
                        };
                    }
                    else if (authentications.Contains("Open"))
                    {
                        Logger.LogInfo("ProxyRequestAuthentication.RunAuthentication", "Open service");
                        response = new ApiProxyResponse
                        {
                            ResponseCode = "00",
                            ResponseDescription = "no authentication required"
                        };
                    }
                    else
                    {
                        authResponse = new DefaultTokenManager().TransactionalTokenAuthentication(proxyRequest.AuthData);
                        response = new ApiProxyResponse
                        {
                            ResponseCode = authResponse.ResponseCode,
                            ResponseDescription = authResponse.ResponseDescription
                        };
                    }

                }
                else
                {
                    response = new ApiProxyResponse
                    {
                        ResponseCode = "",
                        ResponseDescription = "No authentication configured for this service"
                    };
                }

            }
            catch (Exception ex)
            {
                response = new ApiProxyResponse
                {
                    ResponseCode = "06",
                    ResponseDescription = "An error occured while authenticating: " + ex.Message
                };
            }

            Logger.LogInfo("ProxyRequestAuthentication.RunAuthentication.Response", response);
            return Task.Run(() => response);
        }
    }
}
