## What and why

<!-- What does this change, and why? Link the issue if there is one. -->

## Checklist

- [ ] Follows the boundaries in [`docs/architecture/framework-rules.md`](../docs/architecture/framework-rules.md)
      (no `<ProjectReference>` across service/shared-library boundaries, etc.)
- [ ] Tests added or updated (`dotnet test SakhaERP.sln` passes locally)
- [ ] `dotnet build SakhaERP.sln` has 0 warnings
- [ ] If this changes a shared library's public API: version bump considered in
      `Directory.Packages.props` / the library's own `<Version>`
- [ ] Commit messages follow [Conventional Commits](https://www.conventionalcommits.org/)
