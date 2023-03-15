using AppZoneMiddleware.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using AppZoneMiddleware.Shared.Contracts;

namespace Blend.Controllers
{
    public class BlendServiceCollectionExtension : IServiceCollectionExtension
    {
        public void ServiceInitializer(Container container)
        {
            switch (AppZoneMiddleware.Shared.Utility.Utils.ImplementationClientBank)
            {
                case AppZoneMiddleware.Shared.Utility.ImplementationBank.ProvidusBank:
                    container.Register<IUserService, ProvidusImplementation.UserService.ActiveDirectoryUserService>(Lifestyle.Scoped);
                    container.Register<IAccountInquiry, ProvidusImplementation.CoreBankingService.AccountInquiryService>(Lifestyle.Scoped);
                    container.Register<IProfileManager, ProvidusImplementation.ProfileService.ProfileManager>(Lifestyle.Scoped);
                    container.Register<ITokenManager, ProvidusImplementation.ProfileService.TokenManager>(Lifestyle.Scoped);
                    container.Register<IPasswordManager, ProvidusImplementation.ProfileService.PasswordManager>(Lifestyle.Scoped);
                    container.Register<IPinManager, ProvidusImplementation.ProfileService.PinManager>(Lifestyle.Scoped);
                    container.Register<IMailService, ProvidusImplementation.NotificationService.MailService>(Lifestyle.Scoped);
                    container.Register<ISMSSender, ProvidusImplementation.NotificationService.SMSSender>(Lifestyle.Scoped);
                    container.Register<ISecondaryAuthentication, ProvidusImplementation.NotificationService.ISECNotificationService>(Lifestyle.Scoped);
                    container.Register<IBankTransfer, ProvidusImplementation.CoreBankingService.FundTransferService>(Lifestyle.Scoped);
                    break;
                case AppZoneMiddleware.Shared.Utility.ImplementationBank.SterlingBank:
                    container.Register<IUserService, SterlingImplementation.UserService.ActiveDirectoryUserService>(Lifestyle.Scoped);
                    container.Register<IAccountInquiry, SterlingImplementation.CoreBankingService.AccountInquiryService>(Lifestyle.Scoped);
                    container.Register<IProfileManager, SterlingImplementation.ProfileService.ProfileManager>(Lifestyle.Scoped);
                    container.Register<IAccountServices, SterlingImplementation.CoreBankingService.AccountServices>(Lifestyle.Scoped);
                    container.Register<IAirtimeTopup, SterlingImplementation.CustomerService.MobileTopUp>(Lifestyle.Scoped);
                    container.Register<ITokenManager, SterlingImplementation.ProfileService.TokenManager>(Lifestyle.Scoped);
                    container.Register<IPasswordManager, SterlingImplementation.ProfileService.PasswordManager>(Lifestyle.Scoped);
                    container.Register<IPinManager, SterlingImplementation.ProfileService.PinManager>(Lifestyle.Scoped);
                    container.Register<IMailService, SterlingImplementation.NotificationService.MailService>(Lifestyle.Scoped);
                    container.Register<ISMSSender, SterlingImplementation.NotificationService.SMSSender>(Lifestyle.Scoped);
                    container.Register<IBankTransfer, SterlingImplementation.CoreBankingService.FundTransferService>(Lifestyle.Scoped);
                    container.Register<ICardServices, SterlingImplementation.CardManagementServices.CardServices>(Lifestyle.Scoped);
                    container.Register<IInvestmentService, SterlingImplementation.CoreBankingService.InvestmentService>(Lifestyle.Scoped);
                    container.Register<IPaymentServices, SterlingImplementation.Payments.PaymentServices>(Lifestyle.Scoped);
                    break;
                default:
                    throw new ApplicationException(string.Format("Implementation Bank [{0}] is not currently supported.", AppZoneMiddleware.Shared.Utility.Utils.ImplementationClientBank));
            }
        }
    }
}
