using System.Text.Json.Serialization;
namespace WeatherApp.Models;

public class ForecastResponse
{
    [JsonPropertyName("list")]
    public List<ForecastEntry> Items {get; set;} = new();
}

public class ForecastEntry
{
    [JsonPropertyName("main")]
    public ForecastMainData Main {get; set;} = new();
    [JsonPropertyName("wind")]
    public ForecastWindData Wind {get; set;} = new();
    [JsonPropertyName("clouds")]
    public ForecastCloudsData Clouds {get; set;} = new();
    [JsonPropertyName("weather")]
    public List<ForecastWeatherData> Weather {get; set;} = new();
    [JsonPropertyName("rain")]
    public ForecastRainData? Rain {get; set;}
    [JsonPropertyName("snow")]
    public ForecastSnowData? Snow {get; set;} 
    [JsonPropertyName("dt_txt")]
    public string Time {get; set;} = "";
    [JsonPropertyName("visibility")]
    public int Visibility {get; set;}
}

public class ForecastMainData
{
    [JsonPropertyName("temp")]
    public double Temp {get; set;}
    [JsonPropertyName("feels_like")]
    public double FeelsLike {get; set;}
    [JsonPropertyName("humidity")]
    public int Humidity {get; set;}
    [JsonPropertyName("pressure")]
    public int Pressure {get; set;}
}

public class ForecastWindData
{
    [JsonPropertyName("speed")]
    public double Speed {get; set;}
    [JsonPropertyName("deg")]
    public int Deg {get; set;}
}

public class ForecastCloudsData
{
    [JsonPropertyName("all")]
    public int All {get; set;}
}

public class ForecastWeatherData
{
    [JsonPropertyName("description")]
    public string Description {get; set;} = "";
    [JsonPropertyName("icon")]
    public string Icon {get; set;} = "";
}

public class ForecastRainData
{
    [JsonPropertyName("3h")]
    public double? ThreeHour {get; set;}
}

public class ForecastSnowData
{
    [JsonPropertyName("3h")]
    public double? ThreeHour {get; set;}
}

