using AppZoneMiddleware.Shared.Contracts;
using AppzoneSharedMiddleware;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

[assembly: OwinStartup(typeof(Startup))]

namespace AppzoneSharedMiddleware
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            var config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            //Register services
            builder = RegisterServices(builder);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        ContainerBuilder RegisterServices(ContainerBuilder builder)
        {

            return RegisterServicesForBlendContracts(builder);
        }

        private ContainerBuilder RegisterServicesForBlendContracts(ContainerBuilder builder)
        {
            switch (AppZoneMiddleware.Shared.Utility.Utils.ImplementationClientBank)
            {
                case AppZoneMiddleware.Shared.Utility.ImplementationBank.Appzone:
                    builder.RegisterType<Blend.SharedServiceImplementation.Services.AirtimeTopUp>().As<IAirtimeTopup>().SingleInstance();
                    builder.RegisterType<Blend.SharedServiceImplementation.Services.BillPayment>().As<IBillPayment>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.NotificationService.MailService>().As<IMailService>().SingleInstance();
                    builder.RegisterType<Blend.SharedServiceImplementation.Services.WakanowService>().As<IWakanowService>().SingleInstance();
                    builder.RegisterType<Blend.SharedServiceImplementation.Services.NairaBoxServices>().As<INairaBoxService>().SingleInstance();
                    break;
                case AppZoneMiddleware.Shared.Utility.ImplementationBank.HeritageBank:
                    builder.RegisterType<Blend.SharedServiceImplementation.Services.AirtimeTopUp>().As<IAirtimeTopup>().SingleInstance();
                    builder.RegisterType<Blend.SharedServiceImplementation.Services.BillPayment>().As<IBillPayment>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.NotificationService.MailService>().As<IMailService>().SingleInstance();
                    builder.RegisterType<Blend.SharedServiceImplementation.Services.WakanowService>().As<IWakanowService>().SingleInstance();
                    builder.RegisterType<Blend.SharedServiceImplementation.Services.NairaBoxServices>().As<INairaBoxService>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.Cards.PinRetrievalService>().As<IPinService>().SingleInstance();
                    builder.RegisterType<Blend.HeritageImplementation.Transfer.TransferService>().As<IBankTransfer>().SingleInstance();
                    builder.RegisterType<Blend.SharedServiceImplementation.Services.AriiyaTicketService>().As<IAriiyaTicketService>().SingleInstance();
                    break;
                default:
                    throw new ApplicationException(string.Format("Implementation Bank [{0}] is not currently supported.", AppZoneMiddleware.Shared.Utility.Utils.ImplementationClientBank));
            }
            return builder;
        }
    }
}