namespace WeatherApp.Models;

public class Weather
{
    public string City {get; set;} = "";
    public double Temperature {get; set;}
    public double FeelsLike {get; set;}
    public int Humidity {get; set;}
    public double Pressure {get; set;}
    public double WindSpeed {get; set;}
    public double WindDirection {get; set;}
    public int Cloudiness {get; set;}
    public double Visibility {get; set;}
    public double? Rain {get; set;}
    public double? Snow {get; set;}
    public string Description {get; set;} = "";
    public string Icon {get; set;} = "";
}