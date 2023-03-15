using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using Blend.GTBImplementation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppzoneSharedMiddleware.Controllers
{
    [RoutePrefix("api/BillPayment")]
    public class BillPaymentController : ApiController
    {
        IBillPayment _BillPaymentService;

        public BillPaymentController(IBillPayment BillPaymentService)
        {
            _BillPaymentService = BillPaymentService;
        }

        [HttpGet]
        [Route("GetBillerCategroies")]
        public IHttpActionResult GetBillerCategroies()
        {
            QuicktellerBillerCategories response = _BillPaymentService.GetQuicktellerCategories();
            return Ok(response);
        }

        [HttpGet]
        [Route("GetQuciktellerBillersByCategory/{categoryId}")]
        public IHttpActionResult GetQuciktellerBillersByCategory(string categoryId)
        {
            QuicktellerBillerRequest Request = new QuicktellerBillerRequest { CategoryId = categoryId };
            QuicktellerBillerList response = _BillPaymentService.GetQuciktellerBillersByCategory(Request);
            return Ok(response);
        }

        [HttpPost]
        [Route("BillsPaymentAdvice")]
        public IHttpActionResult BillsPaymentAdvice(BillPaymentAdviceRequest paymentRequest)
        {
            BillPaymnetAdviceResponse response = _BillPaymentService.BillsPaymentAdvice(paymentRequest);
            return Ok(response);
        }

        [HttpPost]
        [Route("ValidateCustomer")]
        public IHttpActionResult ValidateCustomer(BillerCustomerValidation validationRequest)
        {
            CustomerValidationResponse response = _BillPaymentService.CustomerValidation(validationRequest);
            return Ok(response);
        }

        [HttpPost]
        [Route("GetBillerForms/{CategoryID}")]
        public IHttpActionResult GetBillerForms(string CategoryID)
        {
            List<JObject> response = new BillPaymentService().GetQuicktellerBillersByCategory(CategoryID);
            return Ok(response);
        }
    }
}
