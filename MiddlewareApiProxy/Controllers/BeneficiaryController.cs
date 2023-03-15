using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Entities.BeneficiaryService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MiddlewareApiProxy.Controllers
{
    [RoutePrefix("api/Beneficiary")]
    public class BeneficiaryController : ApiController
    {
        IBeneficiaryService _IBeneficiaryService;

        public BeneficiaryController(IBeneficiaryService BeneficiaryService)
        {
            _IBeneficiaryService = BeneficiaryService;
        }

        [HttpPost]
        [Route("AddBeneficiary")]
        public IHttpActionResult AddBeneficiary(AddBenefciariesRequest Request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            GetBeneficiariesResponse response = _IBeneficiaryService.AddBeneficiary(Request);
            return Ok(response);
        }

        [HttpPost]
        [Route("GetBeneficiaries")]
        public IHttpActionResult GetBeneficiaries(UserBeneficiaries Request)
        {
            GetBeneficiariesResponse response = _IBeneficiaryService.GetBeneficiaries(Request);
            return Ok(response);
        }

        [HttpPost]
        [Route("RemoveBeneficiary")]
        public IHttpActionResult RemoveBeneficiaries(RemoveBeneficiaryRequest Request)
        {
            GetBeneficiariesResponse response = _IBeneficiaryService.RemoveBeneficiary(Request);
            return Ok(response);
        }
    }
}
