# Weather Application

A comprehensive weather application built with ASP.NET Core that provides real-time weather data, 5-day forecasts, and interactive charts.

## Features

- **Current Weather Display**: Shows temperature, humidity, wind speed, visibility, and more
- **5-Day Forecast**: Detailed weather predictions for the next 5 days
- **Interactive Charts**: Visual representation of temperature, humidity, and wind speed trends
- **Search History**: Keeps track of recently searched cities
- **Responsive Design**: Works seamlessly on desktop and mobile devices
- **Error Handling**: Comprehensive error handling with user-friendly messages

## Prerequisites

- .NET 8.0 SDK
- OpenWeatherMap API key (free registration at https://openweathermap.org/api)

## Setup Instructions

1. **Clone or download the project files**

2. **Get your OpenWeatherMap API key**:
   - Visit https://openweathermap.org/api
   - Sign up for a free account
   - Generate an API key

3. **Configure the API key**:
   - Open `appsettings.json`
   - Replace `##########` with your actual API key:
   ```json
   {
     "WeatherApi": {
       "BaseUrl": "https://api.openweathermap.org/data/2.5",
       "ApiKey": "##############"
     }
   }
   ```

4. **Run the application**:
   ```bash
   dotnet restore
   dotnet run
   ```

5. **Open your browser** and navigate to `https://localhost:5001` or `http://localhost:5000`

## Usage

1. **Search for Weather**: Enter a city name in the search box and click "Get Weather"
2. **View Current Weather**: See detailed current weather information including temperature, humidity, wind speed, and more
3. **Check 5-Day Forecast**: Scroll down to see the 5-day weather forecast
4. **Analyze Trends**: View interactive charts showing temperature, humidity, and wind speed trends over the next 24 hours
5. **Access Search History**: Click on any city in your search history to quickly view its weather

## Technical Implementation

### Backend (ASP.NET Core)
- **WeatherService**: Handles API calls to OpenWeatherMap
- **WeatherController**: Manages HTTP requests and responses
- **Models**: Data transfer objects for weather information
- **Session Management**: Stores search history in browser sessions

### Frontend
- **Bootstrap 5**: Responsive UI framework
- **Chart.js**: Interactive weather charts
- **Font Awesome**: Weather and UI icons
- **Custom CSS**: Modern glassmorphism design
- **JavaScript**: Dynamic interactions and chart rendering

### Key Functions Implemented

1. **FetchWeatherData(city)**: Retrieves current weather data
2. **FetchForecastData(city)**: Gets 5-day weather forecast
3. **SaveSearchHistory(city)**: Stores user search history
4. **GenerateWeatherChart(weatherData)**: Creates interactive charts
5. **HandleAPIError(response)**: Manages error scenarios
6. **FormatTemperature(temperature)**: Formats temperature display

## API Integration

The application integrates with OpenWeatherMap API:
- **Current Weather**: `/weather` endpoint
- **5-Day Forecast**: `/forecast` endpoint
- **Units**: Metric (Celsius, meters/second)
- **Rate Limiting**: Free tier allows 1000 calls/day

## Error Handling

- Invalid city names
- API service unavailability
- Network connectivity issues
- Malformed API responses

## Browser Compatibility

- Chrome (recommended)
- Firefox
- Safari
- Edge

## Security Features

- API key stored in configuration (not in client-side code)
- Input validation and sanitization
- Session-based search history (no persistent storage)
- HTTPS support in production

## Future Enhancements

- Weather alerts and notifications
- Location-based weather (GPS)
- Weather maps integration
- Historical weather data
- Multiple language support
- Dark/light theme toggle

## Troubleshooting

**Common Issues:**

1. **"City not found" error**: Check spelling and try different city names
2. **API errors**: Verify your API key is correct and active
3. **Charts not loading**: Ensure JavaScript is enabled in your browser
4. **Styling issues**: Clear browser cache and reload the page

**API Key Issues:**
- Ensure your OpenWeatherMap account is activated
- Check that you haven't exceeded the free tier limits
- Verify the API key is correctly set in `appsettings.json`

## License

This project is for educational purposes. Please respect OpenWeatherMap's terms of service when using their API.
