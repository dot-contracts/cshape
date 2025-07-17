namespace Nexus.Services.Caching;
using WeatherForecast = Nexus.Client.Models.WeatherForecast;
public interface IWeatherCache
{
    ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
}
