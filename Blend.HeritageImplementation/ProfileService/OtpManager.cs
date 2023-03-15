using AppZoneMiddleware.Shared.Contracts;
using Blend.DefaultImplementation.ProfileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.HeritageImplementation.ProfileService
{
    public class OtpManager : DefaultOtpManager, IOtpManager
    {
        public OtpManager(IMailService mailServcie, ISMSSender smsService, IAccountInquiry accountInquiryService) : base(mailServcie, smsService, accountInquiryService)
        {
        }
    }
}
