using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IBillPayment
    {
        QuicktellerBillerCategories GetQuicktellerCategories();

        QuicktellerBillerList GetQuciktellerBillersByCategory (QuicktellerBillerRequest Request);

        QucktellerPaymentItems GetQuciktellerPaymentItems(QuicktellerPaymentItemRequest Request);

        BillPaymnetAdviceResponse BillsPaymentAdvice(BillPaymentAdviceRequest paymentRequest);
        CustomerValidationResponse CustomerValidation(BillerCustomerValidation ValidationRequest);
    }
}
