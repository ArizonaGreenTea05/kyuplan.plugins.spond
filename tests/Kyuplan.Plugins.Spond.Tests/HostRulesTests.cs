using System.Text.Json;
using Kyuplan.Contracts;

namespace Kyuplan.Plugins.Spond.Tests;

/// <summary>
/// What kyuplan checks when it installs the package and when an administrator saves the settings form: the manifest and the form have to pass its rules
/// (<see cref="PluginRules"/>), or the plugin is refused, and the form has to ask for the same login as before Spond was a plugin.
/// </summary>
public class HostRulesTests
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private static T Read<T>(string file) => JsonSerializer.Deserialize<T>(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, file)), Json)!;

    private static PluginUiLayout Form() => Read<PluginUiLayout>("ui/settings.json");

    private static IReadOnlyDictionary<string, JsonElement> Values(params (string Key, object? Value)[] values) =>
        values.ToDictionary(v => v.Key, v => JsonSerializer.SerializeToElement(v.Value)); // the names as they are: a form sends the ids of its fields

    [Fact]
    public void The_manifest_passes_the_rules_of_the_host()
    {
        var manifest = Read<PluginManifest>("plugin.json");

        Assert.Null(PluginRules.CheckManifest(manifest));
        Assert.True(PluginRules.IsSection(manifest.DataSources!.Single().ConfigSection));
    }

    [Fact]
    public void The_settings_form_passes_the_rules_of_the_host()
    {
        Assert.Null(PluginRules.CheckLayout(Form(), PluginUiUse.DataSourceSettings));
    }

    [Fact]
    public void The_passwords_of_the_form_are_the_ones_of_the_options()
    {
        Assert.Equal(["Password", "Accounts/*/Password"], PluginRules.SecretPaths(Form()));
    }

    [Theory]
    [InlineData("me@example.org", "", "pw", true)]
    [InlineData("", "+491701234567", "pw", true)]
    [InlineData("", "", "", true)] // no login yet: it can be added later
    [InlineData("me@example.org", "", "", false)]
    [InlineData("", "+491701234567", "", false)]
    [InlineData("", "", "pw", false)]
    public void A_login_needs_the_email_or_phone_number_together_with_the_password(string email, string phone, string password, bool fine)
    {
        var problem = PluginRules.CheckValues(Form(), Values(("Email", email), ("PhoneNumber", phone), ("Password", password)), _ => false);

        if (fine) Assert.Null(problem);
        else Assert.Equal("Password: A Spond login needs the email (or phone number) and the password.", problem);
    }

    [Fact]
    public void A_stored_password_is_enough_for_a_login_and_so_is_the_stored_password_of_a_backup_row()
    {
        Assert.Null(PluginRules.CheckValues(Form(), Values(("Email", "me@example.org"), ("Password", "")), path => path == "Password"));

        var kept = Values(("Accounts", new object[] { new { Email = "backup@example.org", Password = "", _keep = 0 } }));
        Assert.Null(PluginRules.CheckValues(Form(), kept, path => path == "Accounts/0/Password"));
        Assert.Equal("Accounts[1].Password: A backup Spond login needs the email (or phone number) and the password.", PluginRules.CheckValues(Form(), kept, _ => false));
    }

    [Fact]
    public void The_password_of_a_backup_row_is_tied_to_the_login_of_the_row()
    {
        var accounts = Form().Elements.Single(e => e.Type == "list").Elements!;

        Assert.Equal(["Email", "PhoneNumber"], PluginRules.RowIdentity(accounts));
    }

    [Fact]
    public void An_email_up_to_the_length_the_standard_allows_and_a_long_password_fit_the_form()
    {
        var email = new string('a', 242) + "@example.org"; // 254 characters

        Assert.Null(PluginRules.CheckValues(Form(), Values(("Email", email), ("Password", new string('p', 500))), _ => false));
        Assert.NotNull(PluginRules.CheckValues(Form(), Values(("Email", email + "x"), ("Password", "pw")), _ => false));
    }

    [Fact]
    public void The_german_texts_of_the_form_are_there_for_every_label_and_message()
    {
        var elements = Form().Elements.Concat(Form().Elements.SelectMany(e => e.Elements ?? []));

        Assert.All(elements, e =>
        {
            Assert.False(string.IsNullOrWhiteSpace(e.LabelDe), $"{e.Id}: labelDe");
            if (e.Together is { } rule) Assert.False(string.IsNullOrWhiteSpace(rule.MessageDe), $"{e.Id}: messageDe");
        });
    }
}
