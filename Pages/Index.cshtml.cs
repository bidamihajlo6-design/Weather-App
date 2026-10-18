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
    public bool HasWeatherData {get; set;} = false;
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
        if (string.IsNullOrWhiteSpace(city))
        {
            ErrorMessage = "Введіть назву міста.";
            return;
        }
        string? apiKey = _configuration["OpenWeather:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            ErrorMessage = "Невдалося отримати API-ключ.Перевірте конфігурацію.";
            return;
        }

        string encodedCity = Uri.EscapeDataString(city);
        using HttpClient client = new HttpClient();
        string geoUrl =
            $"https://api.openweathermap.org/geo/1.0/direct?q={encodedCity},UA&limit=5&appid={apiKey}";


        try
        {
            HttpResponseMessage geoResponse = await client.GetAsync(geoUrl);
            if (!geoResponse.IsSuccessStatusCode)
                {
                    ErrorMessage = "Не вдалося знайти місто.";
                    return;
                }

            string geoJson = await geoResponse.Content.ReadAsStringAsync();

            List<GeocodingResponse>? locations =
                JsonSerializer.Deserialize<List<GeocodingResponse>>(geoJson);
            
            if(locations is null)
            {
                ErrorMessage = "Не вдалося обробити дані міста.";
                return;
            }

            string searchCity = city.Trim();

            if (locations == null || locations.Count == 0)
            {
                ErrorMessage = "Місто не знайдено. Перевірте правильність назви.";
                return;
            }

            GeocodingResponse? location = locations
                .FirstOrDefault(x =>
                    x.Country == "UA" &&
                    (
                        x.Name.Equals(searchCity, StringComparison.OrdinalIgnoreCase)
                        || (x.LocalNames?.Ukrainian?.Equals(searchCity, StringComparison.OrdinalIgnoreCase) ?? false)
                    ));

            if(location is null)
            {
                ErrorMessage = "Координати не знайдено.";
                return;
            }

            if(location.Lat < -90 || location.Lat > 90 || location.Lon < -180  || location.Lon > 180)
            {
                ErrorMessage = "Отримані некоректні дані Міста.";
                return;
            } 

            double latitude = location.Lat;
            double longitude = location.Lon;
            string url =
                $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&lang=uk";

            HttpResponseMessage responseMessage = await client.GetAsync(url);

            if (!responseMessage.IsSuccessStatusCode)
            {
                ErrorMessage = "Помилка отримання погоди.";

                return;
            } 

            string json = await responseMessage.Content.ReadAsStringAsync();

            OpenWeatherResponse? response =
                JsonSerializer.Deserialize<OpenWeatherResponse>(json);
            
            if(response is null)
            {
                ErrorMessage = "Помилка перетворення файлів.";
                return;
            }

            if (response.Main is null || response.Wind is null || response.Clouds is null || response.Weather is null)
            {
                ErrorMessage = "Отримані дані про погоду неповні або некоректні";
                return;
            }
            
        
            string? ukrainianCityName = location.LocalNames?.Ukrainian;

            Weather.City = !string.IsNullOrWhiteSpace(ukrainianCityName)
                ? ukrainianCityName
                : location.Name;

            if(string.IsNullOrWhiteSpace(Weather.City))
            {
                ErrorMessage = "Невдалий пошук міста.";
                return;
            }

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
             if (string.IsNullOrWhiteSpace(Weather.Description))
            {
                ErrorMessage = "Опис погоди відсутній.";
                return;
            }
            HasWeatherData = true;


        }
        catch(HttpRequestException)
        {
            ErrorMessage = "Виникла проблема з мережею";
            return;
        }
        catch(JsonException)
        {
            ErrorMessage = "Виникла проблема з обробкою даних.";
            return;
        }
        catch(Exception)
        {
            ErrorMessage = "Виникла непедбачуванна помилка.";
            return;
        }

    }
}