using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Pages.Forecast;

public class ForecastModel : PageModel
{
    private readonly WeatherService _weatherService;

    public List<ForecastItem> Forecast { get; set; } = new();
    public string City { get; set; } = "";
    public string ErrorMessage { get; set; } = "";

    public ForecastModel(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }
    public DateTime SelectedDate { get; set; }
    public async Task OnGetAsync(string? city, DateTime? selectedDate)
    {
        try
        {
            if (selectedDate.HasValue)
            {
                SelectedDate = selectedDate.Value;
            }
            else
            {
                SelectedDate = DateTime.Today;
            }
            if (string.IsNullOrWhiteSpace(city))
            {
                ErrorMessage = "Назву міста не передано.";
                return;
            }
            City = city;

            GeocodingResponse? location = await _weatherService.GetCityCoordinates(city);
            if (location is null)
            {
                ErrorMessage = "Місто не знайдено.";
                return;
            }
            if (location.Lat < -90 || location.Lat > 90 || location.Lon < -180 || location.Lon > 180)
            {
                ErrorMessage = "Отримані некоректні дані Міста.";
                return;
            }

            ForecastResponse? forecastResponse = await _weatherService.GetForecast(
                location.Lat,
                location.Lon
            );
            if (forecastResponse is null)
            {
                ErrorMessage = "Не вдалося отримати прогноз погоди.";
                return;
            }
            if (forecastResponse.Items is null || forecastResponse.Items.Count == 0)
            {
                ErrorMessage = "Прогноз погоди відсутній.";
                return;
            }
            foreach (var item in forecastResponse.Items)
            {
                if (item.Main is null || item.Wind is null || item.Clouds is null || item.Weather is null)
                {
                    continue;
                }
                if (!DateTime.TryParse(item.Time, out DateTime date))
                {
                    continue;
                }
                if (date.Date == SelectedDate.Date)
                {
                    Forecast.Add(new ForecastItem
                    {
                        DateTimeText = date.ToString("HH:mm"),
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
            }
            if (Forecast.Count == 0)
            {
                ErrorMessage = "Прогноз погоди на обрану дату відсутній.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            ErrorMessage = "Сталася непередбачувана помилка";
        }
        {
        }
    }
}