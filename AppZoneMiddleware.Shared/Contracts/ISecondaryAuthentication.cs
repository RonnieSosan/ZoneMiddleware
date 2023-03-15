using AppZoneMiddleware.Shared.Entites;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface ISecondaryAuthentication
    {
       Task<SecondaryAuthenticationResponse>  CreateProfile(ProfieCreationrequest request);

       bool PushNotification(PushNotification request, out string ErrorMessage);
    }
}
