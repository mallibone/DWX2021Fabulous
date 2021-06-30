using System;

namespace Forecast.Models
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public float TemperatureC { get; set; }
        public float TemperatureF => 32 + (int) (TemperatureC / 0.5556);
        public int Windspeed { get; set; } // kph
        public int Humidity { get; set; } // %
        public string Summary { get; set; }
        public int PostalCode { get; set; }
    }
}