using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Pages;

public class IndexModel : PageModel
{
    public Weather Weather { get; set; } = new Weather();

    public void OnGet()
    {  
    }
    

public void OnPost(string city)
{
    string json = """
    {
        "city": "Kyiv",
        "temperature": 18.5,
        "humidity": 65
    }
    """;

    Weather = JsonSerializer.Deserialize<Weather>(json)!;

    Weather.City = city;
}
}
