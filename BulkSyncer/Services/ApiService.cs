using BulkSyncer.Core;
using BulkSyncer.Model;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace BulkSyncer.Services
{
    public class ApiService
    {
        private string _baseUrl = "https://localhost:32769";

        /// <summary>
        /// DateTime in IST
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public async Task<Weather> GetWeatherData(DateTime date)
        {
            var client = ApiClient.GetClient();
            var result = await client.GetAsync($"{_baseUrl}/WeatherForecast?date={date.ToString("dd-MM-yyyy")}");
            if (!result.IsSuccessStatusCode)
            {
                return null;
            }
            var stream = await result.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Weather>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}