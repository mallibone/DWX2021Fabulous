using System;
using System.Net.Http;
using System.Threading.Tasks;
using Forecast.Models;
using Newtonsoft.Json;

namespace Forecast
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private const string BackendUrl = "https://dwxweatherforecast.azurewebsites.net/api/weatherforecast/forpostalcode/{0}";

        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<WeatherForecast> GetWeatherForecast(int postalCode)
        {
            var url = string.Format(BackendUrl, postalCode);
                var jsonResponse = await _httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<WeatherForecast>(jsonResponse);
        }
    }
}