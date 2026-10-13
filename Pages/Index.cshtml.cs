using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Pages;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    public string ErrorMessage {get; set;} ="";
    public Weather Weather { get; set; } = new Weather();

    public IndexModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnGet()
    {
    }

    public async Task OnPost(string city)
    {
        string apiKey = _configuration["OpenWeather:ApiKey"]!;

        string encodedCity = Uri.EscapeDataString(city);
        using HttpClient client = new HttpClient();
        string geoUrl =
            $"https://api.openweathermap.org/geo/1.0/direct?q={encodedCity},UA&limit=5&appid={apiKey}";


        HttpResponseMessage geoResponse = await client.GetAsync(geoUrl);
        

        if (!geoResponse.IsSuccessStatusCode)
        {
            ErrorMessage = "Не вдалося знайти місто.";
            return;
        }

        string geoJson = await geoResponse.Content.ReadAsStringAsync();

        List<GeocodingResponse> locations =
            JsonSerializer.Deserialize<List<GeocodingResponse>>(geoJson)!;

      string searchCity = city.Trim();

        GeocodingResponse? location = locations
            .FirstOrDefault(x =>
                x.Country == "UA" &&
                (
                    x.Name.Equals(searchCity, StringComparison.OrdinalIgnoreCase)
                    || (x.LocalNames?.Ukrainian?.Equals(searchCity, StringComparison.OrdinalIgnoreCase) ?? false)
                ));

        if (location == null)
        {
            ErrorMessage = "Місто не знайдено. Перевірте правильність назви.";
            return;
        }

        double latitude = location.Lat;
        double longitude = location.Lon;
        string url =
            $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&lang=uk";


        HttpResponseMessage responseMessage = await client.GetAsync(url);

        if (!responseMessage.IsSuccessStatusCode)
        {
            Weather.Description = "Місто не знайдено. Перевірте правильність назви.";

            return;
        } 

        string json = await responseMessage.Content.ReadAsStringAsync();

        OpenWeatherResponse response =
            JsonSerializer.Deserialize<OpenWeatherResponse>(json)!;
        
       
        string? ukrainianCityName = location.LocalNames?.Ukrainian;

        Weather.City = !string.IsNullOrWhiteSpace(ukrainianCityName)
            ? ukrainianCityName
            : location.Name;

        Weather.Temperature = response.Main.Temp;
        Weather.FeelsLike = response.Main.FeelsLike;
        Weather.Humidity = response.Main.Humidity;
        Weather.Pressure = response.Main.Pressure;
        Weather.WindSpeed = response.Wind.Speed;
        Weather.WindDirection = response.Wind.Deg;
        Weather.Cloudiness = response.Clouds.All;
        Weather.Visibility = response.Visibility / 1000.0;

        if (response.Weather.Count > 0)
        {
            Weather.Description = response.Weather[0].Description;
            Weather.Icon = response.Weather[0].Icon;
        }
    }
    
}
