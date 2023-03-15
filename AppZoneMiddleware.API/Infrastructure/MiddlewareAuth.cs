using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace AppZoneMiddleware.API.Infrastructure
{
    public class MiddlewareAuth : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;

            var solution = actionContext.ControllerContext.RouteData.Values["Solution"].ToString();

            if (!AllowedServices.HasServiceAccess(solution, controllerName))
            {
                actionContext.Response = new HttpResponseMessage
                {
                    Content = new StringContent("Service Not Allowed"),
                    StatusCode = HttpStatusCode.Unauthorized
                };

            }
        }
    }
}