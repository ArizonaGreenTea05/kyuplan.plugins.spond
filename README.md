<p align="center">
  <b>kyuplan Spond plugin</b><br>
  Reads the groups, members and events of your club from <a href="https://spond.com">Spond</a> for <a href="https://github.com/ArizonaGreenTea05/kyuplan">kyuplan</a>, the exam planner for karate clubs.
</p>

<p align="center">
<a href="https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/releases"><img alt="Latest release" src="https://img.shields.io/github/v/release/ArizonaGreenTea05/kyuplan.plugins.spond?include_prereleases&sort=semver"></a> <a href="https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/actions/workflows/ci.yml"><img alt="CI" src="https://img.shields.io/github/actions/workflow/status/ArizonaGreenTea05/kyuplan.plugins.spond/ci.yml?branch=main&label=CI"></a> <a href="https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/releases"><img alt="Downloads" src="https://img.shields.io/github/downloads/ArizonaGreenTea05/kyuplan.plugins.spond/total"></a> <a href="https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/issues"><img alt="Open issues" src="https://img.shields.io/github/issues/ArizonaGreenTea05/kyuplan.plugins.spond"></a> <a href="https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/pulls"><img alt="Open pull requests" src="https://img.shields.io/github/issues-pr/ArizonaGreenTea05/kyuplan.plugins.spond"></a> <a href="https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/commits/main"><img alt="Last commit" src="https://img.shields.io/github/last-commit/ArizonaGreenTea05/kyuplan.plugins.spond"></a> <a href="https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/stargazers"><img alt="Stars" src="https://img.shields.io/github/stars/ArizonaGreenTea05/kyuplan.plugins.spond?style=flat"></a> <a href="LICENSE"><img alt="MIT licence" src="https://img.shields.io/github/license/ArizonaGreenTea05/kyuplan.plugins.spond"></a> <a href="https://github.com/ArizonaGreenTea05/kyuplan"><img alt="Made for kyuplan" src="https://img.shields.io/badge/for-kyuplan-blue"></a> <a href="https://ko-fi.com/idkeverygoodnameistaken"><img alt="Sponsor on Ko-fi" src="https://img.shields.io/badge/Ko--fi-sponsor-ff5e5b?logo=ko-fi&logoColor=white"></a>
</p>

<p align="center"><a href="docs/README.md">Documentation</a> · <a href="docs/getting-started.md">Getting started</a> · <a href="docs/configuration.md">Configuration</a> · <a href="docs/troubleshooting.md">Troubleshooting</a> · <a href="https://ko-fi.com/idkeverygoodnameistaken">☕ Sponsor on Ko-fi</a></p>

This is a **kyuplan plugin**: a data source with a settings form that kyuplan draws itself, in a package (`.kyuplugin`) that is attached to every release of this repository.
A new kyuplan installation installs it by default; administrators can also install or update it on the **Plugins** page with this repository's address.

- **Groups, members and events from Spond**: members with their subgroups and date of birth, events with the answer of every member and the hosts (trainers).
- **Several accounts**: backup logins that take over when Spond answers the first one with nothing.
- **Gentle on Spond**: results are cached (15 minutes by default), one request at a time, a login is reused for hours.
- **Club time zone aware**: a training at 23:30 UTC belongs to the next day in Germany, and the plugin knows.
- **Settings in kyuplan**: English and German form, passwords stored encrypted.

## Quick start

1. In kyuplan open **Settings**, choose the data source **Spond**.
2. Enter the e-mail address (or phone number) and the password of a Spond account that is an **administrator of the groups** to analyse.
3. Save. The **Members** page fills within a minute.

Not yet running kyuplan? See the [self-hosting guide](https://github.com/ArizonaGreenTea05/kyuplan/blob/main/docs/deployment.md). Details, backup accounts and tuning: [Getting started](docs/getting-started.md) and [Configuration](docs/configuration.md).

> Two-factor authentication is not supported: the server logs in unattended.

## Documentation

| Page | For | What it covers |
|---|---|---|
| [Getting started](docs/getting-started.md) | Administrators | Installing, updating, first connection, a good Spond account |
| [Configuration](docs/configuration.md) | Administrators | Every setting, environment variables, backup accounts, tuning |
| [How it works](docs/how-it-works.md) | Everyone curious | What is read from Spond and how it becomes kyuplan data |
| [Troubleshooting](docs/troubleshooting.md) | Administrators | Messages, empty lists, logging, known limits |
| [Architecture](docs/architecture.md) | Developers | Classes, caching, locking, account switching |
| [Development](docs/development.md) | Developers | Building, testing, packaging, trying it in kyuplan |
| [Releasing](docs/releasing.md) | Maintainers | Tags, the workflow, versions |
| [Contributing](CONTRIBUTING.md) · [Security](SECURITY.md) | Everyone | How to help, how to report a problem safely |

How plugins work in general and how to write one: [Plugin development](https://github.com/ArizonaGreenTea05/kyuplan/blob/main/docs/plugin-development.md) in the kyuplan repository.

## Licence

[MIT](LICENSE).
