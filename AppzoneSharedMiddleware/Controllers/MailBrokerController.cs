using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AppzoneSharedMiddleware.Controllers
{
    [RoutePrefix("api/MailBroker")]
    public class MailBrokerController : ApiController
    {
        IMailService _mailService;

        public MailBrokerController(IMailService MailService)
        {
            _mailService = MailService;
        }

        [HttpPost]
        [Route("SendMail")]
        public async Task<IHttpActionResult> SendMailAsync([FromBody]MailRequest MailRequest)
        {
            MailResponse response = await _mailService.SendMail(MailRequest);
            return Ok(response);
        }
    }
}
