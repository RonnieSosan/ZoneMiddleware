using AppZoneMiddleware.Shared.Contracts;
using Blend.DefaultImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blend.HeritageImplementation
{
    public class ApiSecurityService : APISecurityService, IAPISecurityService
    {
    }
}
