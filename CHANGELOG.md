# Changelog

All notable changes to this project are documented here. Format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/). This project is pre-1.0 — breaking
changes may land in a minor version until 1.0.0.

## [Unreleased]

### Added
- Open-source project scaffolding: MIT license, `CONTRIBUTING.md`, `CODE_OF_CONDUCT.md`,
  `SECURITY.md`, issue/PR templates.
- CI: build + test workflow and CodeQL analysis (GitHub Actions).
- Central package version management (`Directory.Packages.props`) and shared build settings
  (`Directory.Build.props`) across all projects.
- `docs/architecture/framework-rules.md` — the mono-repo's non-negotiable architectural
  boundaries, now versioned in the repo itself.
- `templates/sakhaerp-service` — a `dotnet new` template that scaffolds a new service with the
  same Clean Architecture layout, CQRS pipeline, and multi-tenancy wiring as `inventory-service`.
- Unit and integration test coverage for `inventory-service`'s `ItemWarehouseMappings` feature
  slice, demonstrating the framework's testing conventions.

### Changed
- Renamed the framework and all its packages from `HorizonERP` to `SakhaERP`.
- `services/inventory-service` now consumes `SakhaERP.Shared.Security` via `PackageReference`
  instead of `ProjectReference`, closing a violation of the framework's own package-boundary rule.

## [0.1.0] — unreleased

Pre-history: `inventory-service` MVP (backend + frontend) and the initial shared library set,
before this project was prepared for open source.
