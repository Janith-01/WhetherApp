using WeatherApp.Models;

namespace WeatherApp.Services
{
    public interface IWeatherService
    {
        Task<WeatherResponse?> FetchWeatherDataAsync(string city);
        Task<ForecastResponse?> FetchForecastDataAsync(string city);
        Task<WeatherViewModel> GetWeatherViewModelAsync(string city, List<SearchHistoryItem> searchHistory);
        string HandleAPIError(string errorMessage);
        string FormatTemperature(double temperature);
        ChartData GenerateChartData(ForecastResponse forecastData);
    }
}
