using System; 
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http;
using System.Net;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using Blend.DefaultImplementation;
using Autofac.Integration.WebApi;
using AppZoneMiddleware.Shared.Contracts.ProxyAuthentication;

namespace MiddlewareApiProxy.Providers
{
    public class AuthorizationHelper : Attribute, IAutofacAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        private readonly IProxyRequestAuthentication _requestAuthenticationManager;

        public AuthorizationHelper(IProxyRequestAuthentication requestAuthenticationManager)
        {
            _requestAuthenticationManager = requestAuthenticationManager;
        }


        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "VlidatedUser")
                    // Add more claims if needed: Roles, ...
                };

            await Task.Run(() => "");

            ApiProxyResponse authResponse = new ApiProxyResponse() { ResponseCode = "00" };

            ApiProxyRequest authRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiProxyRequest>(context.Request.Content.ReadAsStringAsync().Result);

            var urlComponents = authRequest.EndPointURL.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            authRequest.EndPointURL = urlComponents[0] + "//" + urlComponents[1] + "/" + urlComponents[2];


            //call procy authenitcation service to validate authentication data
            authResponse = await _requestAuthenticationManager.RunAuthentication(authRequest);
            if (authResponse != null)
            {
                if (authResponse.ResponseCode != "00")
                {
                    context.ErrorResult = new AuthenticationFailureResult(authResponse.ResponseDescription, context.Request);
                }
                else
                {
                    var identity = new ClaimsIdentity(claims, "Jwt");
                    IPrincipal user = new ClaimsPrincipal(identity);
                    context.Principal = user;
                    return;
                }
                return;
            }
            else
            {
                //no response from server
                context.ErrorResult = new AuthenticationFailureResult("No response from server", context.Request);
                return;
            }
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }


    }

    public class AuthenticationFailureResult : IHttpActionResult
    {
        public string ReasonPhrase { get; }

        public HttpRequestMessage Request { get; }

        public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
        {
            ReasonPhrase = reasonPhrase;
            Request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = Request,
                ReasonPhrase = ReasonPhrase
            };
            return response;
        }
    }
}