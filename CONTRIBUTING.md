# Contributing

Thanks for helping. Small fixes and big ideas are both welcome.

## Before you start

- Look through the [open issues](https://github.com/ArizonaGreenTea05/kyuplan.plugins.spond/issues). For a larger change, open an issue first so we agree on the direction.
- Problems with kyuplan itself (pages, statistics, other data sources) belong in the [kyuplan repository](https://github.com/ArizonaGreenTea05/kyuplan/issues).

## Workflow

1. Fork and create a branch from `main`.
2. Set up as in [Development](docs/development.md) (kyuplan checked out next to this repository).
3. Make the change **with tests** (bug fixes need a test that fails without the fix), and update the documentation in `docs/` if behaviour or settings change.
4. `dotnet test Kyuplan.Plugins.Spond.sln` must pass.
5. Open a pull request. Say what changed and why; link the issue.

## Guidelines

- Keep the plugin read-only towards Spond.
- Never include real credentials, real names or club data in code, tests, issues or logs.
- Settings: every form field needs an English and a German text and must be an option of `SpondOptions`.
- Match the surrounding code: naming, comment density, LF line endings.
