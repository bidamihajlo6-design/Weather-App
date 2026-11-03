namespace WeatherApp.Services;

using WeatherApp.Models;
using System.Text.Json;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        this._httpClient = httpClient;
        this._configuration = configuration;
    }

    public async Task<GeocodingResponse?> GetCityCoordinates(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return null;
        }
        string? apiKey = _configuration["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }
        string encodedCity = Uri.EscapeDataString(city);
        string geoUrl =
            $"https://api.openweathermap.org/geo/1.0/direct?q={encodedCity},UA&limit=5&appid={apiKey}";

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(geoUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Код помилки: {(int)response.StatusCode}");
                return null;
            }
            string json = await response.Content.ReadAsStringAsync();


            List<GeocodingResponse>? locations =
                JsonSerializer.Deserialize<List<GeocodingResponse>>(json);

            if (locations is null || locations.Count == 0)
            {
                return null;
            }

            string searchCity = city.Trim();

            GeocodingResponse? location = locations.FirstOrDefault(x => x.Country == "UA" && (
                x.Name.Equals(searchCity, StringComparison.OrdinalIgnoreCase) || (x.LocalNames?.Ukrainian?.Equals(searchCity, StringComparison.OrdinalIgnoreCase) ?? false)
            ));
            return location;

        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<OpenWeatherResponse?> GetWeather(double latitude, double longitude)
    {
        string? apiKey = _configuration["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }
        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&lang=uk";

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Код помилки: {(int)response.StatusCode}");
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();

            OpenWeatherResponse? weather =
                JsonSerializer.Deserialize<OpenWeatherResponse>(json);

            if (weather is null)
            {
                return null;
            }

            return weather;

        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    public async Task<ForecastResponse?> GetForecast(double latitude, double longitude)
    {
        string? apiKey = _configuration["OpenWeather:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }

        string url = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&lang=uk";

        try
        {

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Код помилки: {(int)response.StatusCode}");
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();


            ForecastResponse? forecast =
                JsonSerializer.Deserialize<ForecastResponse>(json);

            if (forecast is null)
            {
                return null;
            }

            return forecast;
        }

        catch (HttpRequestException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
        catch (JsonException ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
}
