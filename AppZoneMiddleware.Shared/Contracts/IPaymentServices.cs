using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface IPaymentServices
    {
        Task<MPassRegisterUserResponse> MpassRegisterUser(MPassRegisterUserRequest mrur);
        Task<GetQuickTellerBillersResponse> GetQuickTellerBillers(GetQuickTellerBillersRequest req);
        Task<GetQuickTellerBillerItemsResponse> GetQuickTellerBillerItems(GetQuickTellerBillerItemsRequest req);
        Task<QuickTellerBillsPaymentAdviceResponse> InitQuickTellerBillsPaymentAdvice(QuickTellerBillsPaymentAdviceRequest req);
        Task<QuickTellerCustomerValidationResponse> QuickTellerCustomerValidation(QuickTellerCustomerValidationRequest req);
        Task<MPassPaymentResponse> MakeMPassPayment(MPassPaymentRequest req);
    }
}
