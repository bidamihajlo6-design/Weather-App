namespace WeatherApp.Models;

public static class WeatherHelper
{
    public static string GetWindDirection(double degrees)
    {
        if (degrees >= 337.5 || degrees < 22.5)
            return "Північний";
        else if (degrees >= 22.5 && degrees < 67.5)
            return "Північно-Східний";
        else if (degrees >= 67.5 && degrees < 112.5)
            return "Східний";
        else if (degrees >= 112.5 && degrees < 157.5)
            return "Південно-Східний";
        else if (degrees >= 157.5 && degrees < 202.5)
            return "Південний";
        else if (degrees >= 202.5 && degrees < 247.5)
            return "Південно-Західний";
        else if (degrees >= 247.5 && degrees < 292.5)
            return "Західний";
        else
            return "Північно-Західний";
         
    }
    public static double ConvertPressureToMmHg(double pressure)
    {
        return pressure * 0.750062;
    }
}