using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IMailService
    {
        Task<MailResponse> SendMail(MailRequest MailRequest);

        bool SendMail(string mailBody, string emailRecepients, string mailSubject, Dictionary<string, byte[]> mailAttachment);

        UserProfileResponse ValidateEmailAddress(string CustomerID);

        Task<MailResponse> SendMailToCustomer(MailRequest MailRequest);
    }
}
