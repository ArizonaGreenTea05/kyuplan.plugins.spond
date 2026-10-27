# Development

## Requirements

- .NET SDK as pinned in `global.json` (.NET 10).
- The kyuplan repository checked out **next to this one**: the plugin compiles against its `Kyuplan.Plugins.Abstractions` project.

```bash
git clone https://github.com/ArizonaGreenTea05/kyuplan.git
git clone https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond.git
cd kyuplan.plugins.spond
```

If kyuplan lives elsewhere, pass `-p:KyuplanRoot=<path>/` to every `dotnet` command (default `../Kyuplan/`, see `Directory.Build.props`).

## Build and test

```bash
dotnet test Kyuplan.Plugins.Spond.sln
```

Narrow it: `dotnet test --filter "FullyQualifiedName~SpondMappingTests"`. The tests need no network and no Spond account.
Package versions are central in `Directory.Packages.props`; the `.csproj` files carry none.

## Package

```bash
dotnet publish src/Kyuplan.Plugins.Spond -c Release -o publish
(cd publish && zip -r ../kyuplan-plugin-spond.kyuplugin .)
```

The zip must have `plugin.json`, the assembly named in it and `ui/settings.json` at its root. (On Windows without `zip`: `Compress-Archive` the contents of `publish`, then rename to `.kyuplugin`.)

## Trying it in kyuplan

1. Build the package as above.
2. Copy it to the folder `Plugins:BundledDirectory` of a **new** kyuplan installation (before its first start), or install it from a release on the **Plugins** page.
3. Run kyuplan from the kyuplan repository, e.g. `dotnet run --project src/Kyuplan.Api --launch-profile http`, and choose the data source *Spond* with a test account.

A change to the plugin needs the API to restart, as plugins are loaded once at start.

## Changing things

| I want to … | Do |
|---|---|
| add a setting | add the property to `SpondOptions`, the field to `ui/settings.json` (English and German label), and a line to [Configuration](configuration.md). `Every_field_of_the_settings_form_is_an_option_of_the_plugin…` fails if they differ. |
| change how an event is mapped | `SpondApiDataSource.MapEvent`, plus a test in `SpondMappingTests`. |
| change the account logic | `QueryAccountsAsync`; tests in `SpondAccountFallbackTests` with the fake client. |
| ask Spond for more | `SpondAccountClient`; keep the interface small so it can be faked. |

## Conventions

- Tests are named as sentences (`When_the_first_account_yields_no_events_the_second_is_used`). Every change ships with tests, including the edge cases: empty and duplicate data, DST, midnight, unusual strings.
- Files use LF line endings; C# latest, nullable on.
- Documentation changes with the code. Keep [Configuration](configuration.md) and `ui/settings.json` in step.
- Never log or put a password in a message; never commit real credentials or real club data (test data is made up).

The kyuplan side of plugins (manifest, JSON of forms, safety) is in kyuplan's [Plugin development](https://github.com/ArizonaGreenTea05/kyuplan/blob/main/docs/plugin-development.md).
