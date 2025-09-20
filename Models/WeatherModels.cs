namespace WeatherApp.Models
{
    public class WeatherResponse
    {
        public string Name { get; set; } = string.Empty;
        public Main Main { get; set; } = new();
        public Weather[] Weather { get; set; } = Array.Empty<Weather>();
        public Wind Wind { get; set; } = new();
        public Sys Sys { get; set; } = new();
        public int Visibility { get; set; }
        public Clouds Clouds { get; set; } = new();
        public int Dt { get; set; }
    }

    public class Main
    {
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
    }

    public class Weather
    {
        public int Id { get; set; }
        public string Main { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class Wind
    {
        public double Speed { get; set; }
        public int Deg { get; set; }
    }

    public class Sys
    {
        public string Country { get; set; } = string.Empty;
        public int Sunrise { get; set; }
        public int Sunset { get; set; }
    }

    public class Clouds
    {
        public int All { get; set; }
    }

    public class ForecastResponse
    {
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public ForecastItem[] List { get; set; } = Array.Empty<ForecastItem>();
    }

    public class ForecastItem
    {
        public int Dt { get; set; }
        public Main Main { get; set; } = new();
        public Weather[] Weather { get; set; } = Array.Empty<Weather>();
        public Wind Wind { get; set; } = new();
        public string DtTxt { get; set; } = string.Empty;
    }

    public class WeatherViewModel
    {
        public string CityName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int WindDirection { get; set; }
        public int Pressure { get; set; }
        public int Visibility { get; set; }
        public int Cloudiness { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
        public List<ForecastDayViewModel> FiveDayForecast { get; set; } = new();
        public List<SearchHistoryItem> SearchHistory { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class ForecastDayViewModel
    {
        public DateTime Date { get; set; }
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
    }

    public class SearchHistoryItem
    {
        public string CityName { get; set; } = string.Empty;
        public DateTime SearchTime { get; set; }
    }

    public class ChartData
    {
        public List<string> Labels { get; set; } = new();
        public List<double> Temperatures { get; set; } = new();
        public List<double> Humidity { get; set; } = new();
        public List<double> WindSpeed { get; set; } = new();
    }
}
