using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Blend.Controllers
{
    [Route("CardService")]
    public class CardServiceController : ApiController
    {
        ICardServices _cardService;

        public CardServiceController(ICardServices CardService)
        {
            _cardService = CardService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> CardRequest([FromBody]CardRequest cardRequest)
        {
            CardResponse response = await _cardService.CardRequest(cardRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> HotlistCard([FromBody]HotlistCardRequest hotlistCardRequest)
        {
            HotlistCardResponse response = await _cardService.HotlistCard(hotlistCardRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ActivateCard([FromBody]ActivateCardRequest ActivateCardRequest)
        {
            ActivateCardResponse response = await _cardService.ActivateCard(ActivateCardRequest);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetActiveCard([FromBody]RetrieveCardRequest CardRequests)
        {
            RetrieveCardResponse response = await _cardService.GetActiveCards(CardRequests);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetInActiveCard([FromBody]RetrieveCardRequest CardRequests)
        {
            RetrieveCardResponse response = await _cardService.GetInActiveCards(CardRequests);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ActivateCountriesForTrnx([FromBody]ActivateFxTrxRequest Request)
        {
            ActivateFxTrxResponse response = await _cardService.ActivateCountriesForTrnx(Request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreditCardRequest([FromBody]CreditCardRequest Request)
        {
            CardResponse response = await _cardService.CreditCardRequest(Request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> DebitCardRequest([FromBody]CreditCardRequest Request)
        {
            CardResponse response = await _cardService.DebitCardRequest(Request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ActivateCardChannels([FromBody]ActivateChannelRequest Request)
        {
            ActivateChannelResponse response = await _cardService.ActivateChannels(Request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> ActivateForeignTrx([FromBody]CardActionRequest Request)
        {
            CardResponse response = await _cardService.ActivateForeignTransactions(Request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> AllowedCountriesOnPostilion([FromBody]AllowedCountriesOnPostilionRequest Request)
        {
            AllowedCountriesOnPostilionResponse response = await _cardService.AllowedCountriesOnPostilion(Request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreateAndActivateVirtualCard([FromBody]CreateAndActivateVirtualCardRequest Request)
        {
            CreateAndActivateVirtualCardResponse response = await _cardService.CreateAndActivateVirtualCard(Request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IHttpActionResult> GetActiveChannels([FromBody]ActivateChannelRequest Request)
        {
            ActivateChannelResponse response = await _cardService.GetActiveChannels(Request);
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IHttpActionResult> GetActiveCountries([FromBody]ActivateFxTrxRequest Request)
        {
            ActivateFxTrxResponse response = await _cardService.GetActiveCountries(Request);
            return Ok(response);
        }
    }
}
