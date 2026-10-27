namespace Kyuplan.Plugins.Spond;

/// <summary>
/// The settings of the Spond data source: the section <c>Spond</c> of the configuration, filled from the settings form (<c>ui/settings.json</c>) on the settings page of
/// Kyuplan, or set by hand (<c>Spond:Email</c>, environment variables <c>Spond__Email</c>, ...).
/// </summary>
public sealed class SpondOptions
{
    public const string Section = "Spond";

    /// <summary>Spond login (email). Use either this or <see cref="PhoneNumber"/>. Together with the password it is the first account.</summary>
    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Password { get; set; }

    /// <summary>
    /// More Spond logins, tried in this order after the one above. Spond sometimes answers an account that sent too many requests with
    /// nothing at all: when an account yields no groups or events, the next one is asked, and an account that came back empty while
    /// another one delivered is asked last for <see cref="AccountCooldown"/>. All accounts must see the same groups.
    /// </summary>
    public List<SpondAccount> Accounts { get; set; } = [];

    /// <summary>How long an account that came back empty (or failed) while another one delivered is only asked after the others.</summary>
    public TimeSpan AccountCooldown { get; set; } = TimeSpan.FromHours(6);

    /// <summary>Spond tokens are not refreshed by the client library, so the plugin logs in again after this long.</summary>
    public TimeSpan LoginLifetime { get; set; } = TimeSpan.FromHours(6);

    /// <summary>How long fetched events are reused before Spond is asked again.</summary>
    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>The usable accounts in the order they are asked: <see cref="Email"/> / <see cref="PhoneNumber"/> first, then <see cref="Accounts"/>.</summary>
    public IReadOnlyList<SpondAccount> GetAccounts() =>
        [.. new[] { new SpondAccount { Email = Email, PhoneNumber = PhoneNumber, Password = Password } }.Concat(Accounts).Where(a => a.HasCredentials)];
}

/// <summary>One Spond login.</summary>
public sealed class SpondAccount
{
    /// <summary>Spond login (email). Use either this or <see cref="PhoneNumber"/>.</summary>
    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Password { get; set; }

    public bool HasCredentials =>
        !string.IsNullOrWhiteSpace(Password) &&
        (!string.IsNullOrWhiteSpace(Email) || !string.IsNullOrWhiteSpace(PhoneNumber));
}
