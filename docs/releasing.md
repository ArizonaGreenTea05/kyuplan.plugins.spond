# Releasing

One tag `v<version>` is one release.

1. Make sure `main` is green (the **CI** workflow builds, tests and packages on every push and pull request).
2. Tag and push (semantic versioning, e.g. a new setting is a minor version):
   ```bash
   git tag v1.1.0
   git push origin v1.1.0
   ```
3. The **Release** workflow (`.github/workflows/release.yml`):
   - checks out this repository and the kyuplan repository next to it,
   - runs the tests with the version from the tag,
   - writes the version into `src/Kyuplan.Plugins.Spond/plugin.json` (the manifest must carry the version of the tag),
   - publishes and zips the plugin as `kyuplan-plugin-spond-<version>.kyuplugin`,
   - creates the GitHub release with that **one** file.
4. kyuplan finds the plugin through the *latest* release of this repository and expects exactly one `.kyuplugin` asset. Do not attach a second one.

## After the release

- Installations update on the **Plugins** page. A new kyuplan image carries the version that was current when the image was built.
- Check that the release has the file and that `plugin.json` inside shows the new version.

## If something goes wrong

Do not move a tag that has been published: kyuplan stores a SHA-256 of the installed package. Fix forward with a new patch version. A broken release can be deleted on GitHub; the previous release then becomes the latest.

## Version notes

- `Directory.Build.props` and `plugin.json` carry `1.0.0` in the source; the workflow overrides both from the tag.
- Pin new GitHub Actions by commit SHA like the existing ones.
