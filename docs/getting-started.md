# Getting started

## What you need

- A running kyuplan (the [self-hosting guide](https://github.com/ArizonaGreenTea05/kyuplan/blob/main/docs/deployment.md) shows how; it takes minutes with Docker).
- A Spond account that is an **administrator of every group** kyuplan should analyse. Members of a group that are not administrators usually do not see the answers of the others.
- The kyuplan permission to change settings (administrators have it) and, to install or update plugins, `plugins.edit`.

Use a **dedicated account** for kyuplan if you can (a club e-mail address that is added to the groups as an administrator). Then a change of your own password, or a login on a new phone, does not interrupt the planner.
The account must not use two-factor authentication: kyuplan logs in unattended and cannot enter a code.

## Installing

### A new kyuplan installation

Nothing to do: the first start installs the plugin (from the container image, which carries it, or else from the latest release of this repository). If that fails (no network), it is tried again at the next start.

### An existing installation

1. Open the **Plugins** page as an administrator.
2. Install a plugin from a repository: enter `ArizonaGreenTea05/kyuplan.plugins.spond` (or the address `https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond`).
3. kyuplan downloads the `.kyuplugin` of the latest release, stores it with its SHA-256 and restarts. Plugins are loaded once at start, so every install, update or removal restarts the API.

### Updating

On the **Plugins** page choose update for Spond. The newest release is fetched; the old version's files are deleted at the next start. Your settings stay (they belong to kyuplan, not to the plugin version).

### Without GitHub access

Download `kyuplan-plugin-spond-<version>.kyuplugin` from the [releases](https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/releases) and put it into the folder `Plugins:BundledDirectory` of the machine before the **first** start. See kyuplan's documentation on [plugins](https://github.com/ArizonaGreenTea05/kyuplan/blob/main/docs/admin-guide.md#plugins).

## Connecting Spond

1. **Settings** → data source **Spond**.
2. Fill in either *Spond email* or *Spond phone number (instead of the email)*, and *Spond password*. A login needs both an identifier and a password; an entry with only one of them is ignored.
3. Optional: add up to ten **backup accounts** (see [Configuration](configuration.md#backup-accounts)).
4. Save. kyuplan restarts for a moment, applies the data source and starts its first synchronisation.
5. Open **Members**: your groups' members appear within about a minute. **Exams** and the statistics follow from the events.

The password is stored **encrypted** in kyuplan's settings file and is never shown again; leave the field empty when you edit other settings and the stored one is kept.

Without credentials the data source starts anyway and reports *Spond credentials are not configured*; kyuplan then shows no data until you add them.

## Trying it without a Spond account

kyuplan has demo data for this: start it with `Spond:UseDemoData=true` (environment variable `Spond__UseDemoData`). Demo data does not use this plugin; it is part of kyuplan.

## What happens next

kyuplan synchronises on a schedule (its own setting). Each synchronisation asks this plugin for the groups and for the events of the period it analyses; the plugin answers from its cache when it can.
See [How it works](how-it-works.md) for details and [Troubleshooting](troubleshooting.md) if nothing shows up.
