using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherApp.Models;
using WeatherApp.Services;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace WeatherApp.Pages;

public class IndexModel : PageModel
{
    private readonly WeatherService _weathersServise;
    public string ErrorMessage {get; set;} ="";
    public bool HasWeatherData {get; set;} = false;
    public Weather Weather { get; set; } = new Weather();
    public List<ForecastItem> Forecast {get; set;} = new List<ForecastItem>();

    public IndexModel(WeatherService weatherServise)
    {
        _weathersServise = weatherServise;
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

        try
        {
            GeocodingResponse? location = await _weathersServise.GetCityCoordinates(city);
            if(location is null)
            {
                ErrorMessage = "Місто не знайдено. Перевірте назву.";
                return;
            }

            if(location.Lat < -90 || location.Lat > 90 || location.Lon < -180  || location.Lon > 180)
            {
                ErrorMessage = "Отримані некоректні дані Міста.";
                return;
            }
            double latitude = location.Lat;
            double longitude = location.Lon; 

            OpenWeatherResponse? response =
                await _weathersServise.GetWeather(latitude,longitude);

            if(response is null)
            {
                ErrorMessage = "Помилка перетворення файлів.";
                return;
            }

            ForecastResponse? forecastResponse = 
                await _weathersServise.GetForecast(latitude,longitude);

            if(forecastResponse is null || forecastResponse.Items is null || forecastResponse.Items.Count == 0)
            {
                ErrorMessage = "Прогноз погоди для цього міста недоступний.";
                return;
            }
            
            Forecast = new List<ForecastItem>();
            
            foreach(var item in forecastResponse.Items)
            {
                Forecast.Add(new ForecastItem
                {
                    DateTimeText = item.Time,
                    Temperature = item.Main.Temp,
                    FeelsLike = item.Main.FeelsLike,
                    Humidity = item.Main.Humidity,
                    Pressure = item.Main.Pressure,
                    WindSpeed = item.Wind.Speed,
                    WindDirection = item.Wind.Deg,
                    Cloudiness = item.Clouds.All,
                    Visibility = item.Visibility / 1000.0,
                    Description = item.Weather.Count > 0 ? item.Weather[0].Description : "",
                    Icon = item.Weather.Count > 0 ? item.Weather[0].Icon : "",
                    Rain = item.Rain?.ThreeHour,
                    Snow = item.Snow?.ThreeHour
                });
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
            Weather.Rain = response.Rain?.OneHour;
            Weather.Snow = response.Snow?.OneHour;

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