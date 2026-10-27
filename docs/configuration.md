# Configuration

The plugin reads the configuration section **`Spond`**. In normal use you never write these keys yourself: the **Settings** page of kyuplan draws a form from the plugin's `ui/settings.json` and stores the values for you.
The keys matter when you configure kyuplan by file or environment (containers): a section key `Spond:Email` is the environment variable `Spond__Email`, a list entry `Spond:Accounts:0:Email` is `Spond__Accounts__0__Email`.

## Settings

| Key | Default | In the form | Meaning |
|---|---|---|---|
| `Spond:Email` | none | *Spond email* | Login of the first account. Use this **or** the phone number. |
| `Spond:PhoneNumber` | none | *Spond phone number (instead of the email)* | Login of the first account by phone. Ignored when the e-mail is set. |
| `Spond:Password` | none | *Spond password* | Password of the first account. Stored encrypted by kyuplan. |
| `Spond:Accounts` | empty list | *Backup Spond accounts* (max 10) | More accounts, each with `Email` / `PhoneNumber` / `Password`. See below. |
| `Spond:AccountCooldown` | `06:00:00` | not in the form | How long an account that came back empty is asked after the others. |
| `Spond:LoginLifetime` | `06:00:00` | not in the form | After this long the plugin logs in again (tokens are not refreshed). |
| `Spond:CacheDuration` | `00:15:00` | not in the form | How long groups and events are reused before Spond is asked again. |

Durations are .NET time spans: `hh:mm:ss`, or `d.hh:mm:ss`.

Two keys with the prefix `Spond:` belong to kyuplan, not to the plugin: `Spond:UseDemoData` and `Spond:UnregisteredSubGroup` (the subgroup of people not registered in the club yet). They are described in kyuplan's [admin guide](https://github.com/ArizonaGreenTea05/kyuplan/blob/main/docs/admin-guide.md).

The data source itself is chosen with kyuplan's `DataSource:Kind` (`spond` for this plugin).

## Accounts

An account is usable when it has a **password** and an **e-mail or phone number**. The accounts are asked in this order: the single login (`Email`/`PhoneNumber`/`Password`) first, then the entries of `Accounts` as listed. Unusable entries are dropped silently.

### Backup accounts

Spond sometimes answers an account that sent too many requests with *nothing at all* instead of an error. To survive that, add more accounts:

- When an account yields no groups or no events, the next account is asked.
- An account that came back empty while another one delivered is **asked last** for `AccountCooldown` (six hours), so an account Spond ignores is not asked again and again. An account whose login was rejected or that could not reach Spond is treated the same way.
- If no account delivers, the answer is empty; if every account failed outright, the last failure is reported.

**All accounts must see the same groups.** The plugin takes the first non-empty answer; it does not merge accounts.

### Example: environment variables (Docker Compose)

```yaml
environment:
  DataSource__Kind: spond
  Spond__Email: club-planner@example.org
  Spond__Password: ${SPOND_PASSWORD}
  Spond__Accounts__0__Email: second-trainer@example.org
  Spond__Accounts__0__Password: ${SPOND_PASSWORD_2}
  Spond__CacheDuration: "00:30:00"
```

Keep the passwords in an `.env` file or a secret, not in the Compose file.

## Tuning

- **`CacheDuration`**: raise it (30 to 60 minutes) if Spond throttles you or the club is large; lower it if changes in Spond must reach kyuplan faster. kyuplan can also drop the cache when you press its *refresh* action. Results that are empty, or were mapped without the member lookup, are cached for only two minutes whatever you set.
- **`LoginLifetime`**: Spond tokens expire; six hours is a safe default. Lower it only if you see rejected requests after long idle periods. An empty answer on a session older than five minutes also triggers a fresh login once.
- **`AccountCooldown`**: shorten it if your backup accounts recover quickly, lengthen it if Spond ignores an account for a day.

## Permissions

The plugin adds no kyuplan permissions, pages or endpoints. Who may change the settings is kyuplan's concern (the settings permission); who may install the plugin is `plugins.edit`.

## Where things are stored

- Settings: kyuplan's `data/settings.json`; `Spond:Password` and the passwords in `Spond:Accounts` are encrypted there (the form's `password` fields).
- The plugin: `Plugins:Directory` (`index.json` and a folder per plugin and version).
- The plugin keeps nothing on disk itself; its cache lives in memory and is lost at every restart.
