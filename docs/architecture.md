# Architecture

```
kyuplan (host)                          Spond plugin
┌──────────────┐  IKyuplanPlugin   ┌──────────────────────┐
│ PluginHost   │ ───Initialize───▶ │ SpondPlugin          │
│              │ ◀──AddDataSource──│  registers "spond"   │
│ sync / stats │  IDataSource      ├──────────────────────┤
│              │ ◀───────────────▶ │ SpondApiDataSource   │──▶ ISpondAccountClient ×N ──▶ Spond.API ──▶ Spond
└──────────────┘                   │  cache, lock, mapping│
                                   │ ClubClock, SpondOptions
                                   └──────────────────────┘
```

## Files (`src/Kyuplan.Plugins.Spond/`)

| File | Role |
|---|---|
| `plugin.json` | The manifest: id `spond`, the assembly, the data source with its configuration section `Spond` and the settings form. The release workflow writes the tag's version into it. |
| `ui/settings.json` | The settings form (`PluginUiLayout` JSON): fields, a list of backup accounts, `password` inputs (encrypted by kyuplan), English and German labels. |
| `SpondPlugin.cs` | `IKyuplanPlugin`: registers the data source under `DataSourceId = "spond"`, builds it from the bound `SpondOptions`. The only implementation of the interface in the assembly (a test enforces it). |
| `SpondOptions.cs` | The `Spond` section: `SpondOptions`, `SpondAccount`, defaults, `GetAccounts()`. |
| `SpondApiDataSource.cs` | The `IDataSource`: caching, serialisation, login handling, account switching, mapping. |
| `SpondAccountClient.cs` | `ISpondAccountClient` and its Spond.API implementation for one account; the seam for tests. |
| `ClubClock.cs` | "Now" and the start of a day in the club time zone. |

## Rules the code keeps

- **One call at a time.** `SpondApiDataSource` holds a `SemaphoreSlim` (`_gate`) around everything that talks to Spond, because Spond.API's client is not thread-safe. Cache reads happen before and after taking it (double-checked), so concurrent callers share one fetch.
- **`GetMemberIdsAsync` runs without the gate held**: it may load the groups, which takes the gate itself.
- **Empty means suspicious.** Spond.API returns an empty list for any failed request. Empty groups → log in again and retry once; empty events on a session older than five minutes (`SuspiciousSessionAge`) → same; then the next account.
- **Account order.** `QueryAccountsAsync` iterates the accounts with a *stable* `OrderBy` on "still in cooldown", so configured order is kept and cooled-down accounts go last. First non-empty answer wins; empties seen before it are put in cooldown. If all failed outright the last `DataSourceUnavailableException` is thrown; otherwise an empty list.
- **UTC kinds for Spond.API.** Dates passed to it are `DateTimeKind.Utc`, otherwise it would reinterpret `Unspecified` values as machine-local.
- **Raw timestamps.** `SpondEvent.StartTime` is converted to the machine's zone by the library, so the ISO strings are parsed instead and moved into club time.
- **Cache entries share one invalidation token.** `InvalidateCache()` swaps and cancels it, expiring everything at once. Short-lived entries (`EmptyResultCacheDuration`, two minutes) are used for empty or unmapped results.
- **No secrets in logs or messages.** Messages name the account by number and login, never the password.

## Host contract

The plugin gets a `PluginDataSourceContext` from kyuplan: `Configuration` (the `Spond` section), `Time` (a `TimeProvider`), `ClubTimeZone`, `Loggers`. Everything time-related goes through `Time`/`ClubClock`, which is why tests can control the clock and the DST edge cases.
The plugin is loaded in its own `AssemblyLoadContext` from memory, so its dependencies (Spond.API, caching) do not clash with kyuplan's.

## Tests (`tests/Kyuplan.Plugins.Spond.Tests/`)

| File | Covers |
|---|---|
| `SpondPluginTests.cs` | Registration, options binding, manifest consistency, every form field is an option, password fields, a single `IKyuplanPlugin`, the club clock. |
| `SpondMappingTests.cs` | Time zones and DST, answers and their priority, cancelled events, end before start, groups/subgroups, owners as trainers (also by profile id), birth dates. |
| `SpondAccountFallbackTests.cs` | Account order and cooldown, rejected logins, empty groups/events, caching, hosts recognised without prior group load. Uses a fake `ISpondAccountClient`. |
