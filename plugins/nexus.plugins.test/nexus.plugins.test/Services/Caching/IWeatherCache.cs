namespace nexus.plugins.test.Services.Caching;
using WeatherForecast = nexus.plugins.test.Client.Models.WeatherForecast;
public interface IWeatherCache
{
    ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
}
