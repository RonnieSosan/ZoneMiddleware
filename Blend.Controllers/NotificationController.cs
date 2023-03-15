using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entites;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Blend.Controllers
{
    [Route("Notification")]
    public class NotificationController: ApiController
    {
        IMailService _mailService;
        //ISecondaryAuthentication _secondaryAuthentication;
        ISMSSender _smsSender;

        public NotificationController(IMailService MailService, ISMSSender SMSSender)
        {
            _mailService = MailService;
            _smsSender = SMSSender;
        }

        [HttpPost]
        public async Task<IHttpActionResult> SendMail([FromBody]MailRequest MailRequest)
        {
            MailResponse response = await _mailService.SendMail(MailRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> SendMailToCustomer([FromBody]MailRequest MailRequest)
        {
            MailResponse response = await _mailService.SendMailToCustomer(MailRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> SendSMS([FromBody]SMSRequest smsRequest)
        {
            SMSResponse response = await _smsSender.Send(smsRequest);
            return Ok(response);
        }

        //[HttpPost]
        //public async Task<IHttpActionResult> CreateProfile([FromBody]ProfieCreationrequest ProfielCreationRequest)
        //{
        //    SecondaryAuthenticationResponse response = await _secondaryAuthentication.CreateProfile(ProfielCreationRequest);
        //    return Ok(response);
        //}
    }
}
