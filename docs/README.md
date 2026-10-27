# Documentation of the Spond plugin

The Spond plugin is a data source for [kyuplan](https://github.com/ArizonaGreenTea05/kyuplan). Pick the page that fits what you want to do.

**I run a kyuplan instance**
- [Getting started](getting-started.md): install or update the plugin, connect Spond for the first time.
- [Configuration](configuration.md): all settings and what they do.
- [Troubleshooting](troubleshooting.md): what a message means and what to do.

**I want to understand it**
- [How it works](how-it-works.md): what is read from Spond and how it is turned into kyuplan's groups, members and events.

**I want to change it**
- [Architecture](architecture.md): the classes and the rules they keep.
- [Development](development.md): build, test, package.
- [Releasing](releasing.md): how a version gets out.
- [Contributing](../CONTRIBUTING.md), [Security policy](../SECURITY.md).

Words used throughout: a **data source** is where kyuplan reads its groups, members and events from; a **plugin** is a package (`.kyuplugin`) that adds data sources, pages or endpoints;
an **account** is one Spond login (e-mail or phone number plus password).
