using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kyuplan.Plugins.Spond;

/// <summary>The Spond plugin: one data source, <c>spond</c>, whose settings form is <c>ui/settings.json</c> (see <c>plugin.json</c>).</summary>
public sealed class SpondPlugin : IKyuplanPlugin
{
    /// <summary>The id of the data source, as <c>plugin.json</c> declares it and as the settings choose it (<c>DataSource:Kind</c>).</summary>
    public const string DataSourceId = "spond";

    public void Initialize(IPluginBuilder builder) => builder.AddDataSource(DataSourceId, Create);

    private static IDataSource Create(PluginDataSourceContext context)
    {
        var options = context.Configuration.Get<SpondOptions>() ?? new SpondOptions();
        return new SpondApiDataSource(
            options,
            new ClubClock(context.Time, context.ClubTimeZone),
            new MemoryCache(new MemoryCacheOptions()),
            context.Time,
            context.Loggers.CreateLogger<SpondApiDataSource>(),
            context.Loggers);
    }
}
