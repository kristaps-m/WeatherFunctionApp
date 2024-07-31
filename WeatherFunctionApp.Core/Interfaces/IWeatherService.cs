namespace WeatherFunctionApp.Core.Interfaces
{
    public interface IWeatherService
    {
        Task<string> FetchWeatherDataAsync(string url);
    }
}
