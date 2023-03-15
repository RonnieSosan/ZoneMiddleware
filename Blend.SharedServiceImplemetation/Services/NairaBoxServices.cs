using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities.NairaBox;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AppZoneMiddleware.Shared.Utility;
using System.Net.Http;

namespace Blend.SharedServiceImplementation.Services
{
    public class NairaBoxServices : INairaBoxService
    {
        public string NairaBoxAuth = System.Configuration.ConfigurationManager.AppSettings.Get("NairaBoxAuth");
        public string NairaBoxUrl = System.Configuration.ConfigurationManager.AppSettings.Get("NairaBoxUrl");
        public string NairaBoxStagingUrl = System.Configuration.ConfigurationManager.AppSettings.Get("NairaBoxUrlStaging");

        public Task<GetByCinemaResponse> GetByCinema()
        {
            string rawResponse = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/v1/tickets?auth={1}&as=cinemas", NairaBoxUrl, NairaBoxAuth)), Method = HttpMethod.Get };

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    rawResponse = response.Content.ReadAsStringAsync().Result;
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to fetch token");
            }

            var jsonResponse = JsonConvert.DeserializeObject<JObject[]>(rawResponse);
            var actualResponse = jsonResponse.FirstOrDefault();
            var obj = new GetByCinemaResponse
            {
                ResponseCode = Convert.ToString(actualResponse["status"]) == "200" ? "00" : "01",
                ResponseDescription = Convert.ToString(actualResponse["message"]),
                Cinemas = JsonConvert.DeserializeObject<GetByCinema[]>(Convert.ToString(actualResponse["cinemas"]))
            };
            return Task.Run(() => obj);
        }

        public Task<GetEventsResponse> GetEvents()
        {
            string rawResponse = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/v1/tickets?auth={1}&as=events", NairaBoxUrl, NairaBoxAuth)), Method = HttpMethod.Get };

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    rawResponse = response.Content.ReadAsStringAsync().Result;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to fetch token");
            }

            var jsonResponse = JsonConvert.DeserializeObject<JObject>(rawResponse);
            var obj = new GetEventsResponse
            {
                ResponseCode = Convert.ToString(jsonResponse["status"]) == "200" ? "00" : "01",
                ResponseDescription = Convert.ToString(jsonResponse["message"]),
                Events = JsonConvert.DeserializeObject<Event[]>(Convert.ToString(jsonResponse["events"]))
            };
            return Task.Run(() => obj);
        }

        public Task<GetNairaBoxMovieResponse> GetMovieById(string MovieId)
        {
            string rawResponse = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/v1/movies/{1}", NairaBoxUrl, MovieId)), Method = HttpMethod.Get };

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    rawResponse = response.Content.ReadAsStringAsync().Result;
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to fetch token");
            }

            var jsonResponse = JsonConvert.DeserializeObject<JObject>(rawResponse);
            var obj = new GetNairaBoxMovieResponse
            {
                ResponseCode = Convert.ToString(jsonResponse["status"]) == "200" ? "00" : "01",
                ResponseDescription = Convert.ToString(jsonResponse["message"]),
                Movie = JsonConvert.DeserializeObject<NairaBoxMovie>(Convert.ToString(jsonResponse["data"]))
            };
            return Task.Run(() => obj);
        }

        public Task<GetMovieDetailsResponse> GetMovieDetails(string MovieId)
        {
            string rawResponse = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/v2/tickets/?as=MovieDetail&mid={2}&auth={1}", NairaBoxUrl, NairaBoxAuth, MovieId)), Method = HttpMethod.Get };

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    rawResponse = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to fetch movie");
            }

            var jsonResponse = JsonConvert.DeserializeObject<JObject>(rawResponse);
            var obj = new GetMovieDetailsResponse
            {
                ResponseCode = Convert.ToString(jsonResponse["status"]) == "200" ? "00" : "01",
                ResponseDescription = Convert.ToString(jsonResponse["message"]),
                Movie = JsonConvert.DeserializeObject<Movie>(Convert.ToString(jsonResponse["movie"]))
            };
            return Task.Run(() => obj);
        }

        public Task<GetMoviesResponse> GetMovies()
        {
            string rawResponse = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/v2/tickets?auth={1}&as=movies", NairaBoxStagingUrl, NairaBoxAuth)), Method = HttpMethod.Get };

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    rawResponse = response.Content.ReadAsStringAsync().Result;
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to fetch token");
            }

            var jsonResponse = JsonConvert.DeserializeObject<JObject>(rawResponse);
            var obj = new GetMoviesResponse
            {
                ResponseCode = Convert.ToString(jsonResponse["status"]) == "200" ? "00" : "01",
                ResponseDescription = Convert.ToString(jsonResponse["message"]),
                Movies = JsonConvert.DeserializeObject<Movie[]>(Convert.ToString(jsonResponse["movies"]))
            };
            return Task.Run(() => obj);
        }

        public Task<GetShowtimeResponse> GetShowtime(GetShowtimeRequest Request)
        {
            string rawResponse = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/v2/tickets?as=showtimes&cinemaId={2}&ticketId={3}&auth={1}", NairaBoxStagingUrl, NairaBoxAuth, Request.CinemaId, Request.TicketId)), Method = HttpMethod.Get };

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    rawResponse = response.Content.ReadAsStringAsync().Result;
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to fetch token");
            }

            var jsonResponse = JsonConvert.DeserializeObject<JObject>(rawResponse);
            var obj = new GetShowtimeResponse
            {
                ResponseCode = Convert.ToString(jsonResponse["status"]) == "200" ? "00" : "01",
                ResponseDescription = Convert.ToString(jsonResponse["message"]),
                Showtimes = JsonConvert.DeserializeObject<Showtime[]>(Convert.ToString(jsonResponse["showtimes"]))
            };
            return Task.Run(() => obj);
        }

        public Task<PurcaseTicketResponse> PurchaseTicket(PurchaseTicketRequest Request, string PurchaseType)
        {
            Logger.LogInfo("AriiyaTicketService.PurchaseTicket.Request", Request);
            string rawResponse = string.Empty;
            PurcaseTicketResponse response = new PurcaseTicketResponse();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    switch (PurchaseType)
                    {
                        case "Movie":
                            var niraboxTicketRequest = new NiraboxTicketRequest
                            {
                                auth = NairaBoxAuth,
                                userDetails = new UserDetails { fullname = Request.fullname, email = Request.Email, phone = Request.Phone, quantity = Convert.ToInt32(Request.Qty), ticketType = Request.TicketType, ticketTypeId = Request.TicketTypeId },
                                ticketInfo = new TicketInfo { showTimeId = Request.showTimeId },
                                reference = Request.showTimeId,
                                totalTickets = Convert.ToInt32(Request.Qty)
                            };
                            request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/v2/tickets?as=pay", NairaBoxStagingUrl)), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(niraboxTicketRequest), Encoding.UTF8, "application/json") };
                            break;
                        case "Event":
                            var nairaBoxEventTicketRequest = JsonConvert.SerializeObject(new NairaBoxEventTicketRequest { classid = Request.classid, email = Request.Email, phone = Request.Phone, qty = Request.Qty, reference = Request.Reference, auth = NairaBoxAuth });
                            request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/v1/tickets/", NairaBoxUrl)), Method = HttpMethod.Post, Content = new StringContent(nairaBoxEventTicketRequest, Encoding.UTF8, "application/json") };
                            break;
                        default:
                            throw new Exception("unable to define ticket purchase type");
                    }

                    HttpResponseMessage httpResponse = client.SendAsync(request).Result;
                    rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                    Logger.LogInfo("NairaBoxServices.PurchaseTicket.ServerResponse", rawResponse);

                    if (httpResponse.IsSuccessStatusCode)
                    {

                        var jsonResponse = JsonConvert.DeserializeObject<JObject>(rawResponse);
                        response = new PurcaseTicketResponse
                        {
                            ResponseCode = Convert.ToString(jsonResponse["status"]) == "201" ? "00" : "01",
                            ResponseDescription = Convert.ToString(jsonResponse["message"]),
                            TransactionReference = Convert.ToString(jsonResponse["Transaction reference"]),
                            ticketID = Convert.ToString(jsonResponse["ticketID"])
                        };
                    }
                    else
                    {
                        response = new PurcaseTicketResponse
                        {
                            ResponseCode = "06",
                            ResponseDescription = "unable to get response from Nairabox"
                        };
                    }
                }


            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw ex;
            }

            Logger.LogInfo("NairaBoxServices.PurchaseTicket.Response", response);
            return Task.Run(() => response);
        }

        public Task<VerifyTicketResponse> VerifyTicket(string RefId)
        {
            string rawResponse = string.Empty;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage();

                    request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/v1/tickets/?auth={1}&ref={2}&as=verify", NairaBoxUrl, NairaBoxAuth, RefId)), Method = HttpMethod.Get };

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    rawResponse = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw new Exception("unable to fetch movie");
            }

            var jsonResponse = JsonConvert.DeserializeObject<JObject>(rawResponse);
            var obj = new VerifyTicketResponse
            {
                ResponseCode = Convert.ToString(jsonResponse["status"]) == "SUCCESS" ? "00" : "01",
                ResponseDescription = Convert.ToString(jsonResponse["reason"]),
                reference = Convert.ToString(jsonResponse["reference"]),
                Hash = Convert.ToString(jsonResponse["Hash"]),
                reason = Convert.ToString(jsonResponse["reason"]),
                ticket = Convert.ToString(jsonResponse["ticket"])
            };
            return Task.Run(() => obj);
        }
    }
}
