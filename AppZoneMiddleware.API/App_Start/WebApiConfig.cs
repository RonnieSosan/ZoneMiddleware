using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using AppZoneMiddleware.API.Infrastructure;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Web.Http.Tracing;

namespace AppZoneMiddleware.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes

            //config.m
            // BlendRouteInit.Execute(config);
            // ZoneRouteInit.Execute(config);

            config.Services.Replace(typeof(IHttpControllerSelector),new CustomControllerSelector(config));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{Solution}/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            #region Configuration to Enable Tacing. NOTE: Packages 'Microsoft.AspNet.WebApi.Tracing' and 'Microsoft.AspNet.WebApi.WebHost' must be installed/up-to-date
            bool isVerbose = false;
            Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["IsTracingVerbose"], out isVerbose);
            TraceLevel traceLevel = TraceLevel.Off;
            Enum.TryParse<TraceLevel>(System.Configuration.ConfigurationManager.AppSettings["TraceLevel"], out traceLevel);
            SystemDiagnosticsTraceWriter traceWriter = config.EnableSystemDiagnosticsTracing();
            traceWriter.IsVerbose = isVerbose;
            traceWriter.MinimumLevel = traceLevel;
            #endregion
        }
    }
}
