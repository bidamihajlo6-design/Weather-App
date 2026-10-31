using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class OpenWeatherResponse
{
    [JsonPropertyName("main")]
    public MainData Main { get; set; } = new();

    [JsonPropertyName("wind")]
    public WindData Wind { get; set; } = new();

    [JsonPropertyName("clouds")]
    public CloudsData Clouds { get; set; } = new();

    [JsonPropertyName("rain")]
    public RainData? Rain {get; set;}
 
    [JsonPropertyName("snow")]
    public SnowData? Snow {get; set;}


    [JsonPropertyName("weather")]
    public List<WeatherData> Weather { get; set; } = new();

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("visibility")]
    public int Visibility { get; set; }
}

public class MainData
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("pressure")]
    public int Pressure { get; set; }
}

public class WindData
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }

    [JsonPropertyName("deg")]
    public int Deg { get; set; }
}

public class CloudsData
{
    [JsonPropertyName("all")]
    public int All { get; set; }
}

public class RainData
{
    [JsonPropertyName("1h")]
    public double? OneHour {get; set;}

    [JsonPropertyName("3h")]
    public double? ThreeHour {get; set;}
}

public class SnowData
{
    [JsonPropertyName("1h")]
    public double? OneHour {get; set;}
    [JsonPropertyName("3h")]
    public double? ThreeHour {get; set;}
}

public class WeatherData
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("icon")]
    public string Icon {get; set; } = "";
}