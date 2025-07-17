namespace nexus.plugins.floor.Services.Caching;
using WeatherForecast = nexus.plugins.floor.Client.Models.WeatherForecast;
public interface IWeatherCache
{
    ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
}
