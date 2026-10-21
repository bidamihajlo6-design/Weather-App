namespace WeatherApp.Services;
using Microsoft.Extensions.Configuration;
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
        string apiKey = _configuration["OpenWeather:ApiKey"]!;
        string encodedCity = Uri.EscapeDataString(city);
        string geoUrl =
            $"https://api.openweathermap.org/geo/1.0/direct?q={encodedCity},UA&limit=5&appid={apiKey}";
        
        HttpResponseMessage response = await _httpClient.GetAsync(geoUrl);

        if (!response.IsSuccessStatusCode)
        {
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

        GeocodingResponse? location = locations.FirstOrDefault(x => x.Country == "UA" &&(
            x.Name.Equals(searchCity, StringComparison.OrdinalIgnoreCase) || (x.LocalNames?.Ukrainian?.Equals(searchCity, StringComparison.OrdinalIgnoreCase) ?? false)
        ));
        return location;
    }

    public async Task<OpenWeatherResponse?> GetWeather(double latitude, double longitude)
    {
        string apiKey = _configuration["OpenWeather:ApiKey"]!;
        string url =        $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&lang=uk";

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        if(!response.IsSuccessStatusCode)
        {
            return null;
        }
        string json = await response.Content.ReadAsStringAsync();

        OpenWeatherResponse? weather = 
            JsonSerializer.Deserialize<OpenWeatherResponse>(json);
        return weather;
    }
}