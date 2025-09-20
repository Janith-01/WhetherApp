using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<WeatherResponse?> FetchWeatherDataAsync(string city)
        {
            try
            {
                var apiKey = _configuration["WeatherApi:ApiKey"];
                var baseUrl = _configuration["WeatherApi:BaseUrl"];
                var url = $"{baseUrl}/weather?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";

                _logger.LogInformation($"Fetching weather data for city: {city}");

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonSerializer.Deserialize<WeatherResponse>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return weatherData;
                }
                else
                {
                    _logger.LogWarning($"API request failed with status: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching weather data for city: {city}");
                return null;
            }
        }

        public async Task<ForecastResponse?> FetchForecastDataAsync(string city)
        {
            try
            {
                var apiKey = _configuration["WeatherApi:ApiKey"];
                var baseUrl = _configuration["WeatherApi:BaseUrl"];
                var url = $"{baseUrl}/forecast?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";

                _logger.LogInformation($"Fetching forecast data for city: {city}");

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var forecastData = JsonSerializer.Deserialize<ForecastResponse>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return forecastData;
                }
                else
                {
                    _logger.LogWarning($"API request failed with status: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching forecast data for city: {city}");
                return null;
            }
        }

        public async Task<WeatherViewModel> GetWeatherViewModelAsync(string city, List<SearchHistoryItem> searchHistory)
        {
            var viewModel = new WeatherViewModel
            {
                SearchHistory = searchHistory
            };

            try
            {
                var weatherTask = FetchWeatherDataAsync(city);
                var forecastTask = FetchForecastDataAsync(city);

                await Task.WhenAll(weatherTask, forecastTask);

                var weatherData = await weatherTask;
                var forecastData = await forecastTask;

                if (weatherData == null)
                {
                    viewModel.ErrorMessage = HandleAPIError($"Unable to fetch weather data for {city}");
                    return viewModel;
                }

                // Map current weather data
                viewModel.CityName = weatherData.Name;
                viewModel.Country = weatherData.Sys.Country;
                viewModel.Temperature = weatherData.Main.Temp;
                viewModel.FeelsLike = weatherData.Main.FeelsLike;
                viewModel.MinTemp = weatherData.Main.TempMin;
                viewModel.MaxTemp = weatherData.Main.TempMax;
                viewModel.Description = weatherData.Weather.FirstOrDefault()?.Description ?? "N/A";
                viewModel.Icon = weatherData.Weather.FirstOrDefault()?.Icon ?? "01d";
                viewModel.Humidity = weatherData.Main.Humidity;
                viewModel.WindSpeed = weatherData.Wind.Speed;
                viewModel.WindDirection = weatherData.Wind.Deg;
                viewModel.Pressure = weatherData.Main.Pressure;
                viewModel.Visibility = weatherData.Visibility / 1000; // Convert to km
                viewModel.Cloudiness = weatherData.Clouds.All;
                viewModel.Sunrise = DateTimeOffset.FromUnixTimeSeconds(weatherData.Sys.Sunrise).DateTime;
                viewModel.Sunset = DateTimeOffset.FromUnixTimeSeconds(weatherData.Sys.Sunset).DateTime;

                // Map 5-day forecast data
                if (forecastData != null)
                {
                    viewModel.FiveDayForecast = ProcessForecastData(forecastData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing weather data for city: {city}");
                viewModel.ErrorMessage = HandleAPIError($"An error occurred while processing weather data for {city}");
            }

            return viewModel;
        }

        private List<ForecastDayViewModel> ProcessForecastData(ForecastResponse forecastData)
        {
            var dailyForecasts = new Dictionary<string, List<ForecastItem>>();

            // Group forecast items by date
            foreach (var item in forecastData.List)
            {
                var date = DateTime.Parse(item.DtTxt).Date.ToString("yyyy-MM-dd");
                if (!dailyForecasts.ContainsKey(date))
                {
                    dailyForecasts[date] = new List<ForecastItem>();
                }
                dailyForecasts[date].Add(item);
            }

            var result = new List<ForecastDayViewModel>();

            // Process each day
            foreach (var day in dailyForecasts.Take(5))
            {
                var dayItems = day.Value;
                var minTemp = dayItems.Min(x => x.Main.TempMin);
                var maxTemp = dayItems.Max(x => x.Main.TempMax);
                var avgHumidity = (int)dayItems.Average(x => x.Main.Humidity);
                var avgWindSpeed = dayItems.Average(x => x.Wind.Speed);

                // Get the most common weather condition for the day
                var mostCommonWeather = dayItems
                    .SelectMany(x => x.Weather)
                    .GroupBy(x => x.Main)
                    .OrderByDescending(x => x.Count())
                    .FirstOrDefault();

                result.Add(new ForecastDayViewModel
                {
                    Date = DateTime.Parse(day.Key),
                    MinTemp = minTemp,
                    MaxTemp = maxTemp,
                    Description = mostCommonWeather?.FirstOrDefault()?.Description ?? "N/A",
                    Icon = mostCommonWeather?.FirstOrDefault()?.Icon ?? "01d",
                    Humidity = avgHumidity,
                    WindSpeed = avgWindSpeed
                });
            }

            return result;
        }

        public string HandleAPIError(string errorMessage)
        {
            return errorMessage.Contains("404") || errorMessage.Contains("not found") 
                ? "City not found. Please check the spelling and try again." 
                : "Unable to fetch weather data. Please try again later.";
        }

        public string FormatTemperature(double temperature)
        {
            return $"{Math.Round(temperature, 1)}°C";
        }

        public ChartData GenerateChartData(ForecastResponse forecastData)
        {
            var chartData = new ChartData();
            
            if (forecastData?.List == null) return chartData;

            // Get next 24 hours of data (8 data points, 3-hour intervals)
            var next24Hours = forecastData.List.Take(8);

            foreach (var item in next24Hours)
            {
                var time = DateTime.Parse(item.DtTxt);
                chartData.Labels.Add(time.ToString("HH:mm"));
                chartData.Temperatures.Add(item.Main.Temp);
                chartData.Humidity.Add(item.Main.Humidity);
                chartData.WindSpeed.Add(item.Wind.Speed);
            }

            return chartData;
        }
    }
}
