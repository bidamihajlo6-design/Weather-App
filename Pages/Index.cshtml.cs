using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WeatherApp.Pages;

public class IndexModel : PageModel
{
    public string City { get; set;} = "";
    public void OnGet()
    {
    }

    public void Onpost(string city)
    {
        City = city;
    }
}
