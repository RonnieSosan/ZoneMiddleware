using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Reflection;
using Autofac;
using Autofac.Integration.WebApi;
using Autofac.Integration.Mvc;
using System.Web.Http;
using MiddlewareApiProxy.Providers;
using AppZoneMiddleware.Shared.Contracts.ProxyAuthentication;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using MiddlewareApiProxy.Controllers;
using AppZoneMiddleware.Shared.Contracts;
using System.Net.Http;

[assembly: OwinStartup(typeof(MiddlewareApiProxy.Startup))]

namespace MiddlewareApiProxy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            //Register controller assembly
            builder.RegisterControllers(typeof(MessageBrokerController).Assembly);
            builder.RegisterModule<AutofacWebTypesModule>();
            builder.RegisterWebApiFilterProvider(config);

            //Register services
            builder = RegisterServices(builder);
            builder.Register(c => new AuthorizationHelper(c.Resolve<IProxyRequestAuthentication>())).AsWebApiAuthenticationFilterFor<MessageBrokerController>(c => c.Run(c.Request.Content.ReadAsAsync<ApiProxyRequest>().Result)).InstancePerRequest();
            //builder.Register(c => new AuthorizationHelper(c.Resolve<IProxyRequestAuthentication>())).AsWebApiActionFilterOverrideFor<MessageBrokerController>(c => c.Post(default(ApiProxyRequest))).InstancePerRequest();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            // ConfigureAuth(app);
        }

        ContainerBuilder RegisterServices(ContainerBuilder builder)
        {

            return RegisterServicesForBlendContracts(builder);
        }

        private ContainerBuilder RegisterServicesForBlendContracts(ContainerBuilder builder)
        {
            switch (AppZoneMiddleware.Shared.Utility.Utils.ImplementationClientBank)
            {
                case AppZoneMiddleware.Shared.Utility.ImplementationBank.HeritageBank:
                    builder.RegisterType<Blend.HeritageImplementation.ProfileService.PasswordManager>().As<IPasswordManager>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.ProfileService.BeneficiaryManager>().As<IBeneficiaryService>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.ProfileService.PinManager>().As<IPinManager>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.ProfileService.ProfileManager>().As<IProfileManager>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.ProfileService.TokenManager>().As<ITokenManager>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.ProxyAuthentication>().As<IProxyRequestAuthentication>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.AccountInquiryService>().As<IAccountInquiry>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.NotificationService.MailService>().As<IMailService>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.NotificationService.SMSService>().As<ISMSSender>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.ApiSecurityService>().As<IAPISecurityService>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.AdminPortal.HeritageActiveDirectoryService>().As<IUserService>().SingleInstance();
                    break;
                case AppZoneMiddleware.Shared.Utility.ImplementationBank.SterlingBank:
                    break;
                default:
                    throw new ApplicationException(string.Format("Implementation Bank [{0}] is not currently supported.", AppZoneMiddleware.Shared.Utility.Utils.ImplementationClientBank));
            }
            return builder;
        }
    }
}
