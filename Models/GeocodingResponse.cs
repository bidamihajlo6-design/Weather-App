using System.Text.Json.Serialization;

namespace WeatherApp.Models;

public class GeocodingResponse
{
    [JsonPropertyName("name")]
    public string Name {get; set;} = "";
    [JsonPropertyName("lat")]
    public double Lat {get; set;}
    [JsonPropertyName("lon")]
    public double Lon {get; set;}
    [JsonPropertyName("country")]
    public string Country {get; set;} = "";
    [JsonPropertyName("state")]
    public string State {get; set;} = "";
    [JsonPropertyName("local_names")]
    public LocalNamesData LocalNames {get; set;} = new();
}

public class LocalNamesData
{
    [JsonPropertyName("uk")]
    public string Ukrainian {get; set;} = "";

    [JsonPropertyName("en")]
    public string English {get; set;} = "";
}