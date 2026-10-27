using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kyuplan.Plugins.Spond.Tests;

/// <summary>What makes the code a plugin: it registers its data source, reads its section of the configuration, and its manifest and settings form fit it.</summary>
public class SpondPluginTests
{
    private sealed class Builder : IPluginBuilder
    {
        public Dictionary<string, Func<PluginDataSourceContext, IDataSource>> DataSources { get; } = [];

        public string Id => "spond";

        public string Version => "1.0.0";

        public string Directory => AppContext.BaseDirectory;

        public ILoggerFactory Loggers => NullLoggerFactory.Instance;

        public void AddDataSource(string id, Func<PluginDataSourceContext, IDataSource> create) => DataSources.Add(id, create);

        public void MapEndpoints(Action<RouteGroupBuilder> map) => throw new InvalidOperationException("the Spond plugin adds no endpoints");

        public string Permission(string name) => throw new InvalidOperationException("the Spond plugin adds no permissions");
    }

    private static PluginDataSourceContext Context(params (string Key, string Value)[] settings) => new(
        new ConfigurationBuilder().AddInMemoryCollection(settings.ToDictionary(s => "Spond:" + s.Key, s => (string?)s.Value)).Build().GetSection("Spond"),
        TimeProvider.System, TimeZoneInfo.Utc, NullLoggerFactory.Instance, new ServiceCollection().BuildServiceProvider());

    private static JsonElement ReadJson(string file) =>
        JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, file))).RootElement;

    [Fact]
    public void The_plugin_registers_one_data_source_with_the_id_of_the_manifest()
    {
        var builder = new Builder();

        new SpondPlugin().Initialize(builder);

        Assert.Equal(["spond"], builder.DataSources.Keys);
        Assert.Equal(SpondPlugin.DataSourceId, builder.DataSources.Keys.Single());
    }

    [Fact]
    public async Task The_data_source_is_built_from_the_section_and_says_what_is_missing_without_a_login()
    {
        var builder = new Builder();
        new SpondPlugin().Initialize(builder);

        using var source = (IDisposable)builder.DataSources["spond"](Context());
        var data = (IDataSource)source;

        Assert.Equal("spond", data.Kind);
        Assert.False(data.IsDemo);
        var ex = await Assert.ThrowsAsync<DataSourceUnavailableException>(() => data.GetGroupsAsync(default));
        Assert.Contains("credentials are not configured", ex.Message);
    }

    [Fact]
    public void The_options_are_read_from_the_section_including_the_list_of_backup_accounts_and_the_times()
    {
        var section = Context(
            ("Email", "first@example.org"), ("Password", "one"), ("Accounts:0:PhoneNumber", "+491701234567"), ("Accounts:0:Password", "two"),
            ("AccountCooldown", "00:10:00"), ("LoginLifetime", "02:00:00"), ("CacheDuration", "00:05:00")).Configuration;

        var options = section.Get<SpondOptions>()!;

        Assert.Equal(["first@example.org", null], options.GetAccounts().Select(a => a.Email));
        Assert.Equal("+491701234567", options.GetAccounts()[1].PhoneNumber);
        Assert.Equal((TimeSpan.FromMinutes(10), TimeSpan.FromHours(2), TimeSpan.FromMinutes(5)), (options.AccountCooldown, options.LoginLifetime, options.CacheDuration));
    }

    [Fact]
    public void The_manifest_names_the_assembly_the_data_source_and_the_section_of_the_plugin()
    {
        var manifest = ReadJson("plugin.json");

        Assert.Equal("spond", manifest.GetProperty("id").GetString());
        Assert.Equal(typeof(SpondPlugin).Assembly.GetName().Name + ".dll", manifest.GetProperty("assembly").GetString());
        var source = manifest.GetProperty("dataSources")[0];
        Assert.Equal(SpondPlugin.DataSourceId, source.GetProperty("id").GetString());
        Assert.Equal(SpondOptions.Section, source.GetProperty("configSection").GetString());
        Assert.True(File.Exists(Path.Combine(AppContext.BaseDirectory, source.GetProperty("settings").GetString()!)));
        Assert.Matches(@"^\d+(\.\d+){0,3}$", manifest.GetProperty("version").GetString());
    }

    [Fact]
    public void Every_field_of_the_settings_form_is_an_option_of_the_plugin_and_the_passwords_are_password_fields()
    {
        var form = ReadJson("ui/settings.json");
        var options = typeof(SpondOptions).GetProperties().Select(p => p.Name).ToHashSet();
        var accountFields = typeof(SpondAccount).GetProperties().Select(p => p.Name).ToHashSet();

        foreach (var element in form.GetProperty("elements").EnumerateArray())
        {
            var id = element.GetProperty("id").GetString()!;
            Assert.Contains(id, options);
            if (element.GetProperty("type").GetString() != "list") continue;

            Assert.Equal(typeof(SpondOptions).GetProperty(id)!.PropertyType, typeof(List<SpondAccount>));
            foreach (var row in element.GetProperty("elements").EnumerateArray()) Assert.Contains(row.GetProperty("id").GetString()!, accountFields);
        }

        var passwords = form.GetProperty("elements").EnumerateArray().Where(e => e.GetProperty("id").GetString() == "Password").ToList();
        Assert.Equal("password", Assert.Single(passwords).GetProperty("input").GetString());
    }

    [Fact]
    public void The_plugin_class_is_the_only_implementation_of_the_interface_in_the_assembly()
    {
        var plugins = typeof(SpondPlugin).Assembly.GetExportedTypes().Where(t => !t.IsAbstract && typeof(IKyuplanPlugin).IsAssignableFrom(t)).ToList();

        Assert.Equal([typeof(SpondPlugin)], plugins);
        Assert.NotNull(typeof(SpondPlugin).GetConstructor(Type.EmptyTypes));
    }

    [Fact]
    public void The_clock_tells_the_clubs_days_and_not_the_machines()
    {
        var berlin = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
        var clock = new ClubClock(TimeProvider.System, berlin);

        Assert.Equal(new DateTimeOffset(2026, 3, 10, 0, 0, 0, TimeSpan.FromHours(1)), clock.StartOfDay(new DateOnly(2026, 3, 10)));
        Assert.Equal(new DateTimeOffset(2026, 7, 10, 0, 0, 0, TimeSpan.FromHours(2)), clock.StartOfDay(new DateOnly(2026, 7, 10))); // summer time
        Assert.Equal(TimeSpan.FromHours(1), clock.ToClub(new DateTimeOffset(2026, 1, 10, 23, 30, 0, TimeSpan.Zero)).Offset);
        Assert.Equal(11, clock.ToClub(new DateTimeOffset(2026, 1, 10, 23, 30, 0, TimeSpan.Zero)).Day); // the next day in Germany
    }
}
