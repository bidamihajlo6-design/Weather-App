using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
namespace WeatherApp.Models;

public class ForecastItem
{
    public string DateTimeText {get; set;} ="";
    public double Temperature {get; set;}
    public double FeelsLike {get; set;}
    public int Humidity {get; set;}
    public double Pressure {get; set;}
    public double WindSpeed {get; set;}
    public double WindDirection {get; set;}
    public  int Cloudiness {get; set;}
    public double Visibility {get; set;}
    public string Description {get; set;} = "";
    public double? Snow {get; set;}
    public double? Rain {get; set;}
    public string Icon {get; set;} = "";
}