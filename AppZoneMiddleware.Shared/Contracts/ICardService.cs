using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Contracts
{
    public interface ICardServices
    {
        Task<CardResponse> CardRequest(CardRequest cardRequest);
        Task<HotlistCardResponse> HotlistCard(HotlistCardRequest hotlistCardRequest);
        Task<ActivateCardResponse> ActivateCard(ActivateCardRequest ActivateCardRequest);
        Task<RetrieveCardResponse> GetActiveCards(RetrieveCardRequest cardRequest);
        Task<RetrieveCardResponse> GetInActiveCards(RetrieveCardRequest cardRequest);
        Task<ActivateFxTrxResponse> ActivateCountriesForTrnx(ActivateFxTrxRequest Request);
        Task<CardResponse> CreditCardRequest(CreditCardRequest Request);
        Task<CardResponse> DebitCardRequest(CreditCardRequest Request);
        Task<ActivateChannelResponse> ActivateChannels(ActivateChannelRequest Request);
        Task<CardResponse> ActivateForeignTransactions(CardActionRequest Request);
        Task<AllowedCountriesOnPostilionResponse> AllowedCountriesOnPostilion(AllowedCountriesOnPostilionRequest Request);
        Task<CreateAndActivateVirtualCardResponse> CreateAndActivateVirtualCard(CreateAndActivateVirtualCardRequest Request);
        Task<ActivateChannelResponse> GetActiveChannels(ActivateChannelRequest Request);
        Task<ActivateFxTrxResponse> GetActiveCountries(ActivateFxTrxRequest Request);
    }
}
