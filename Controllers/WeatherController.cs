using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IWeatherService weatherService, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string city = "")
        {
            var searchHistory = GetUserSearchHistory();
            var viewModel = new WeatherViewModel
            {
                SearchHistory = searchHistory
            };

            if (!string.IsNullOrEmpty(city))
            {
                viewModel = await _weatherService.GetWeatherViewModelAsync(city, searchHistory);
                SaveSearchHistory(city);
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                TempData["ErrorMessage"] = "Please enter a city name.";
                return RedirectToAction("Index");
            }

            var searchHistory = GetUserSearchHistory();
            var viewModel = await _weatherService.GetWeatherViewModelAsync(city, searchHistory);
            
            if (!string.IsNullOrEmpty(viewModel.ErrorMessage))
            {
                TempData["ErrorMessage"] = viewModel.ErrorMessage;
                return RedirectToAction("Index");
            }

            SaveSearchHistory(city);
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return Json(new { error = "City name is required" });
            }

            try
            {
                var forecastData = await _weatherService.FetchForecastDataAsync(city);
                if (forecastData == null)
                {
                    return Json(new { error = "Unable to fetch forecast data" });
                }

                var chartData = _weatherService.GenerateChartData(forecastData);
                return Json(chartData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating chart data for city: {city}");
                return Json(new { error = "Error generating chart data" });
            }
        }

        [HttpPost]
        public IActionResult ClearHistory()
        {
            HttpContext.Session.Remove("SearchHistory");
            return RedirectToAction("Index");
        }

        private void SaveSearchHistory(string city)
        {
            var searchHistory = GetUserSearchHistory();
            
            // Remove if already exists to avoid duplicates
            searchHistory.RemoveAll(x => x.CityName.Equals(city, StringComparison.OrdinalIgnoreCase));
            
            // Add to beginning of list
            searchHistory.Insert(0, new SearchHistoryItem
            {
                CityName = city,
                SearchTime = DateTime.Now
            });

            // Keep only last 10 searches
            if (searchHistory.Count > 10)
            {
                searchHistory = searchHistory.Take(10).ToList();
            }

            var serializedHistory = System.Text.Json.JsonSerializer.Serialize(searchHistory);
            HttpContext.Session.SetString("SearchHistory", serializedHistory);
        }

        private List<SearchHistoryItem> GetUserSearchHistory()
        {
            var historyJson = HttpContext.Session.GetString("SearchHistory");
            if (string.IsNullOrEmpty(historyJson))
            {
                return new List<SearchHistoryItem>();
            }

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<SearchHistoryItem>>(historyJson) ?? new List<SearchHistoryItem>();
            }
            catch
            {
                return new List<SearchHistoryItem>();
            }
        }
    }
}
