using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using AppZoneMiddleware.Shared.Extension;
using System.Text.RegularExpressions;
using Blend.DefaultImplementation.Persistence;
using Blend.DefaultImplementation.ProfileService;

namespace Blend.HeritageImplementation.ProfileService
{
    public class PasswordManager : DefaultPasswordManager, IPasswordManager
    {
        public PasswordManager(IMailService mailService, ISMSSender smsSender) : base(mailService, smsSender)
        {
        }
    }
}
