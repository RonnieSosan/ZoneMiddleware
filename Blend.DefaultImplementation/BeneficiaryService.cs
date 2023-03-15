using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.DefaultImplementation.Persistence;
using AppZoneMiddleware.Shared.Entities.BeneficiaryService;
using Newtonsoft.Json;

namespace Blend.DefaultImplementation
{
    public class BeneficiaryService : IBeneficiaryService
    {
        public GetBeneficiariesResponse AddBeneficiary(AddBenefciariesRequest Request)
        {
            Logger.LogInfo("BeneficiaryService.AddBeneficiary, input", Request);

            GetBeneficiariesResponse response = null;
            try
            {
                if (Request.CustomerID == string.Empty || Request.Name == string.Empty || Request.CustomerID == null || Request.Name == null)
                {
                    response = new GetBeneficiariesResponse
                    {
                        ResponseCode = "01",
                        ResponseDescription = "invalid request data: " + JsonConvert.SerializeObject(Request),
                        Beneficiaries = null,
                    };
                    return response;
                }
                ContextRepository<UserBeneficiaries> beneficiaryRepository = new ContextRepository<UserBeneficiaries>();
                UserBeneficiaries userBeneficiaries = null;
                userBeneficiaries = beneficiaryRepository.Get(Request.CustomerID);
                if (userBeneficiaries != null)
                {
                    if (userBeneficiaries.Beneficiaries == null)
                    {
                        userBeneficiaries.Beneficiaries = JsonConvert.SerializeObject(
                            new List<Beneficiary>() {
                                new Beneficiary{
                                    AccountNumber = Request.AccountNumber,
                                    Name = Request.Name,
                                    BankCode = Request.BankCode,
                                    BankName = Request.BankName
                                } });
                        beneficiaryRepository.Update(userBeneficiaries);
                    }
                    else
                    {
                        List<Beneficiary> beneficiaries = JsonConvert.DeserializeObject<List<Beneficiary>>(userBeneficiaries.Beneficiaries);
                        if (beneficiaries.Where(x => x.AccountNumber == Request.AccountNumber).Count() > 0)
                        {
                            response = new GetBeneficiariesResponse
                            {
                                ResponseCode = "01",
                                ResponseDescription = "Beneficiary already exists",
                                Beneficiaries = JsonConvert.DeserializeObject<List<Beneficiary>>(beneficiaryRepository.Get(Request.CustomerID).Beneficiaries),
                            };
                            Logger.LogInfo("BeneficiaryService.GetBeneficiaries, output", response);
                            return response;
                        }
                        else
                        {
                            beneficiaries.Add(new Beneficiary
                            {
                                AccountNumber = Request.AccountNumber,
                                Name = Request.Name,
                                BankCode = Request.BankCode,
                                BankName = Request.BankName
                            });
                        }
                        userBeneficiaries.Beneficiaries = JsonConvert.SerializeObject(beneficiaries);
                        beneficiaryRepository.Update(userBeneficiaries);
                    }
                }
                else
                {
                    beneficiaryRepository.Save(new UserBeneficiaries
                    {
                        Beneficiaries = JsonConvert.SerializeObject(
                            new List<Beneficiary>() {
                                new Beneficiary{
                                    AccountNumber = Request.AccountNumber,
                                    Name = Request.Name,
                                    BankCode = Request.BankCode,
                                    BankName = Request.BankName
                                } }),
                        CustomerID = Request.CustomerID
                    });
                }
                response = new GetBeneficiariesResponse
                {
                    ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.Successful),
                    ResponseDescription = "SUCCESSFUL",
                    Beneficiaries = JsonConvert.DeserializeObject<List<Beneficiary>>(beneficiaryRepository.Get(Request.CustomerID).Beneficiaries),
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new GetBeneficiariesResponse
                {
                    ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.SystemError),
                    ResponseDescription = "Failed: " + ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
            }

            Logger.LogInfo("BeneficiaryService.GetBeneficiaries, output", response);
            return response;
        }

        public GetBeneficiariesResponse GetBeneficiaries(UserBeneficiaries Request)
        {
            Logger.LogInfo("BeneficiaryService.GetBeneficiaries, input", Request);

            GetBeneficiariesResponse response = null;
            try
            {
                ContextRepository<UserBeneficiaries> beneficiaryRepository = new ContextRepository<UserBeneficiaries>();
                Request = beneficiaryRepository.Get(Request.CustomerID);
                if (Request != null)
                {
                    response = new GetBeneficiariesResponse
                    {
                        ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.Successful),
                        ResponseDescription = "SUCCESSFUL",
                        Beneficiaries = JsonConvert.DeserializeObject<List<Beneficiary>>(Request.Beneficiaries)
                    };
                }
                else
                {
                    response = new GetBeneficiariesResponse
                    {
                        ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.InvalidCustomerID),
                        ResponseDescription = "user not found"
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new GetBeneficiariesResponse
                {
                    ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.SystemError),
                    ResponseDescription = "Failed: " + ex.InnerException != null ? ex.InnerException.Message : ex.Message
                };
            }

            Logger.LogInfo("BeneficiaryService.GetBeneficiaries, output", response);
            return response;
        }

        public GetBeneficiariesResponse RemoveBeneficiary(RemoveBeneficiaryRequest Request)
        {
            Logger.LogInfo("BeneficiaryService.RemoveBeneficiary, input", Request);

            GetBeneficiariesResponse response = null;
            try
            {
                ContextRepository<UserBeneficiaries> beneficiaryRepository = new ContextRepository<UserBeneficiaries>();
                UserBeneficiaries userBeneficiaries = null;
                userBeneficiaries = beneficiaryRepository.Get(Request.CustomerID);
                if (userBeneficiaries != null)
                {
                    if (userBeneficiaries.Beneficiaries != null)
                    {
                        List<Beneficiary> beneficiaries = JsonConvert.DeserializeObject<List<Beneficiary>>(userBeneficiaries.Beneficiaries);
                        beneficiaries.Remove(beneficiaries.Where(x => x.Name == Request.BeneficiaryName && x.AccountNumber == Request.AccountNumber).FirstOrDefault());
                        userBeneficiaries.Beneficiaries = JsonConvert.SerializeObject(beneficiaries);
                        beneficiaryRepository.Update(userBeneficiaries);
                    }
                    response = new GetBeneficiariesResponse
                    {
                        ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.Successful),
                        ResponseDescription = "SUCCESSFUL",
                        Beneficiaries = JsonConvert.DeserializeObject<List<Beneficiary>>(beneficiaryRepository.Get(Request.CustomerID).Beneficiaries),
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new GetBeneficiariesResponse
                {
                    ResponseCode = ResponseCodeTranslator.ToIntVal(MiddleWareResponseCodes.SystemError),
                    ResponseDescription = "Failed: " + ex.InnerException == null ? ex.Message : ex.InnerException.Message
                };
            }

            Logger.LogInfo("BeneficiaryService.RemoveBeneficiary, output", response);
            return response;
        }
    }
}
