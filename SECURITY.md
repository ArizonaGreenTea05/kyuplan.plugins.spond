# Security policy

## Reporting a vulnerability

Please do **not** open a public issue for a security problem. Use GitHub's private reporting:
[Report a vulnerability](https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/security/advisories/new).
Describe what you found, how to reproduce it and what it could allow.

## What matters here

The plugin handles the **login of a Spond account**. In scope: leaking or logging a password, storing it unencrypted, sending it anywhere but Spond, and vulnerabilities in the package or its release workflow.

## How secrets are handled

- Passwords are `password` fields of the settings form; kyuplan stores them encrypted and never shows them again.
- The plugin never writes passwords to the log or into an error message.
- The plugin keeps nothing on disk; tokens live in memory.
- Use a dedicated Spond account for kyuplan so it can be revoked without affecting anyone.

## Supported versions

Only the latest release receives fixes.
