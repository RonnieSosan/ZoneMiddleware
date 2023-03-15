using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Entities.Wakanow;
using AppZoneMiddleware.Shared.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Blend.SharedServiceImplementation.Services
{
    public class WakanowService : IWakanowService
    {
        public string WakanowUsername = System.Configuration.ConfigurationManager.AppSettings.Get("WakanowUsername");
        public string WakanowPassword = System.Configuration.ConfigurationManager.AppSettings.Get("WakanowPassword");
        public string WakanowUrl = System.Configuration.ConfigurationManager.AppSettings.Get("WakanowUrl");
        public WakanowAuthenticationData token = null;

        public WakanowService()
        {
            token = GetToken();
        }

        private WakanowAuthenticationData GetToken()
        {
            string rawResponse = string.Empty;
            try
            {
                var keyValue = new Dictionary<string, string> { { "grant_type", "password" }, { "Username", "f08f6877e1e3487fa57a63e1ce35667a" }, { "Password", "0q>;{sw^_e" } };
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    using (var content = new FormUrlEncodedContent(keyValue))
                    {
                        var requestUri = new Uri(string.Format("{0}/token", WakanowUrl));
                        content.Headers.Clear();
                        content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                        HttpResponseMessage response = client.PostAsync(requestUri, content).Result;
                        rawResponse = response.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to fetch token");
            }

            var authenticationData = JsonConvert.DeserializeObject<WakanowAuthenticationData>(rawResponse);
            return authenticationData;
        }

        public Task<WakanowAirport[]> GetAirports()
        {
            string rawResponse = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage();

                request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/api/flight/airports", WakanowUrl)), Method = HttpMethod.Get };

                request.Headers.Add("Authorization", "Bearer " + token.AccessToken);
                HttpResponseMessage response = client.SendAsync(request).Result;
                rawResponse = response.Content.ReadAsStringAsync().Result;

            }

            byte[] compressed = Convert.FromBase64String(rawResponse.Trim('"'));
            byte[] decompressed = Decompress(compressed);
            string stringListOfAirports = Encoding.UTF8.GetString(decompressed);

            var listofAirports = JsonConvert.DeserializeObject<WakanowAirport[]>(stringListOfAirports);
            return Task.Factory.StartNew(() => listofAirports);
        }

        public static byte[] Decompress(byte[] input)
        {
            using (var source = new MemoryStream(input))
            {
                byte[] lengthBytes = new byte[4];
                source.Read(lengthBytes, 0, 4);

                var length = BitConverter.ToInt32(lengthBytes, 0);
                using (var decompressionStream = new GZipStream(source,
                    CompressionMode.Decompress))
                {
                    var result = new byte[length];
                    decompressionStream.Read(result, 0, length);
                    return result;
                }
            }
        }

        public Task<SearchResult> FlightSerach(FlightSearchRequest searchRequest)
        {
            string rawResponse = string.Empty;
            SearchResult searchResponse = new SearchResult();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/api/flight/search", WakanowUrl)), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(searchRequest), Encoding.UTF8, "application/json") };
                    if (DateTime.Parse(token.Expires) < DateTime.Now)
                    {
                        token = GetToken();
                    }

                    request.Headers.Add("Authorization", "Bearer " + token.AccessToken);

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    rawResponse = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        searchResponse.ResponseCode = "06";
                        searchResponse.ResponseDescription = Utils.ExtractJSON(rawResponse);
                    }
                    else
                    {
                        var flightSearchResult = JsonConvert.DeserializeObject<FlightSearchResponse[]>(rawResponse);
                        searchResponse.flightSearchResult = flightSearchResult;
                        searchResponse.ResponseCode = "00";
                        searchResponse.ResponseDescription = "SUCCESSFUL";
                    }
                }
            }
            catch (Exception ex)
            {
                searchResponse.ResponseCode = "06";
                searchResponse.ResponseDescription = ex.Message;
            }
           
            return Task.Factory.StartNew(() =>  searchResponse);
        }

        public Task<SelectFlightResponse> SelectFlight(SelectFlightRequest selectRequest)
        {
            string rawResponse = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage();

                request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/api/flight/select", WakanowUrl)), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(selectRequest), Encoding.UTF8, "application/json") };
                if (DateTime.Parse(token.Expires) < DateTime.Now)
                {
                    token = GetToken();
                }

                request.Headers.Add("Authorization", "Bearer " + token.AccessToken);

                HttpResponseMessage response = client.SendAsync(request).Result;
                rawResponse = response.Content.ReadAsStringAsync().Result;

            }
            var flightSelectResult = JsonConvert.DeserializeObject<SelectFlightResponse>(rawResponse);
            return Task.Factory.StartNew(() => flightSelectResult);
        }

        public Task<FlightBookResponse> FlightBook(FlightBookRequest BookingRequest)
        {
            string rawResponse = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage();

                request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/api/flight/book", WakanowUrl)), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(BookingRequest), Encoding.UTF8, "application/json") };
                if (DateTime.Parse(token.Expires) < DateTime.Now)
                {
                    token = GetToken();
                }

                request.Headers.Add("Authorization", "Bearer " + token.AccessToken);

                HttpResponseMessage response = client.SendAsync(request).Result;
                rawResponse = response.Content.ReadAsStringAsync().Result;

            }
            var flightBookResult = JsonConvert.DeserializeObject<FlightBookResponse>(rawResponse);
            return Task.Factory.StartNew(() => flightBookResult);
        }

        public Task<TicketConfirmResponse> TicketConfirm(TicketConfirmRequest confirmRequest)
        {
            string rawResponse = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage();

                request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/api/flight/ticketpnr", WakanowUrl)), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(confirmRequest), Encoding.UTF8, "application/json") };
                if (DateTime.Parse(token.Expires) < DateTime.Now)
                {
                    token = GetToken();
                }

                request.Headers.Add("Authorization", "Bearer " + token.AccessToken);

                HttpResponseMessage response = client.SendAsync(request).Result;
                rawResponse = response.Content.ReadAsStringAsync().Result;

            }
            var ticketConfirmResult = JsonConvert.DeserializeObject<TicketConfirmResponse>(rawResponse);
            return Task.Factory.StartNew(() => ticketConfirmResult);
        }
    }
}
