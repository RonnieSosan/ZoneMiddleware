﻿using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;

namespace Blend.HeritageImplementation.NotificationService
{
    public class SMSService : ISMSSender
    {
        public Task<SMSResponse> Send(SMSRequest SMS)
        {
            throw new NotImplementedException();
        }
    }
}
