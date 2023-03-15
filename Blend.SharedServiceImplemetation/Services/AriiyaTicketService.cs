using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SharedServiceImplementation.Services
{
    public class AriiyaTicketService : IAriiyaTicketService
    {
        public string AriiyaUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings.Get("AriiyaUrl"));
        public string getEventsURL = string.Empty;
        IBankTransfer _BankTransfer;

        public AriiyaTicketService(IBankTransfer bankTransfer)
        {
            getEventsURL = String.Format("{0}/tribe/events/v1/events/", AriiyaUrl);
            _BankTransfer = bankTransfer;
        }

        public AriiyaGetEventsResponse GetEvents()
        {
            Logger.LogInfo("AriiyaTicketService.GetEvents", "");
            string rawResponse = string.Empty;
            AriiyaGetEventsResponse response = new AriiyaGetEventsResponse();
            try
            {

                response = new AriiyaGetEventsResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "Success",
                    events = retrieveAriiyaTickets(getEventsURL)
                };

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                response = new AriiyaGetEventsResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "unable to get ariiya ticket"
                };
            }
            Logger.LogInfo("AriiyaTicketService.GetEvents.Response", response);
            return response;
        }

        public AriiyaOrderResponse PlaceTicketOrder(AriiyaTicketOrderRequest orderRequest)
        {
            Logger.LogInfo("AriiyaTicketService.PlaceTicketOrder: ", orderRequest);
            string rawResponse = string.Empty;
            AriiyaOrderResponse response = new AriiyaOrderResponse();

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage();

                request = new HttpRequestMessage() { RequestUri = new Uri("https://www.ariiyatickets.com/dev/wp-json/wc/v3/orders"), Method = HttpMethod.Post };
                string username = "ck_1657317ee5a49374a6ae214f4d5902013c992bc9";
                string password = "cs_7685009f05fd6f19beb17c04ecb9dbaf50996a9a";

                string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

                request.Headers.Add("Authorization", "Basic " + svcCredentials);

                HttpResponseMessage httpResponse = client.SendAsync(request).Result;
                rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                if (httpResponse.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<AriiyaOrderResponse>(rawResponse);

                }
            }
            return response;
        }

        private List<Event> retrieveAriiyaTickets(string eventsURL)
        {
            string rawResponse = string.Empty;
            List<Event> response = new List<Event>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request = new HttpRequestMessage() { RequestUri = new Uri(eventsURL), Method = HttpMethod.Get };

                    HttpResponseMessage httpResponse = client.SendAsync(request).Result;
                    rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        AriiyaGetEventsResponse Events = JsonConvert.DeserializeObject<AriiyaGetEventsResponse>(rawResponse);

                        if (Events.NextRestUrl != null)
                        {
                            Events.events.AddRange(retrieveAriiyaTickets(Convert.ToString(Events.NextRestUrl)));
                        }
                        response = Events.events;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to reach ariiya servers");
            }
            return response;
        }


    }
}
