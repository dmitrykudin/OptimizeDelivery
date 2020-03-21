using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Models.ApiModels;
using Newtonsoft.Json;

namespace OptimizeDelivery.TelegramBot
{
    public class OptimizeDeliveryApiClient
    {
        private static HttpClient ApiClient { get; set; }

        private static string OptimizeDeliveryApiBaseUrl => ConfigurationManager.AppSettings["OptimizeDeliveryAPIUrl"];

        private static string DefaultMediaType = "application/json"; 

        public OptimizeDeliveryApiClient()
        {
            ApiClient = new HttpClient()
            {
                BaseAddress = new Uri(OptimizeDeliveryApiBaseUrl)
            };
        }

        public async Task<CreateCourierResult> CreateCourier(int telegramId, string firstName, string lastName)
        {
            var result = await ApiClient.PostAsync("courier/create",
                new StringContent(JsonConvert.SerializeObject(new CreateCourierRequest
                {
                    TelegramId = telegramId,
                    FirstName = firstName,
                    LastName = lastName,
                }), Encoding.UTF8, DefaultMediaType));
            var resultContent = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateCourierResult>(resultContent);
        }

        public async Task<GetRouteResult> GetRouteForToday(int telegramId)
        {
            var result = await ApiClient.PostAsync("courier/route",
                new StringContent(JsonConvert.SerializeObject(new GetRouteRequest
                {
                    TelegramId = telegramId,
                }), Encoding.UTF8, DefaultMediaType));
            var resultContent = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GetRouteResult>(resultContent);
        }
    }
}