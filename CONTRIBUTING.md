# Contributing to SakhaERP

Thanks for considering a contribution. SakhaERP is a framework for building multi-tenant,
event-driven microservices on .NET — contributions to the framework itself (shared libraries,
service scaffolding, conventions, docs) are especially welcome.

## Ground rules

The architectural boundaries in [`docs/architecture/framework-rules.md`](docs/architecture/framework-rules.md)
are non-negotiable and apply to every PR:

- **No `<ProjectReference>` across service or shared-library boundaries.** Only `<PackageReference>`
  to a versioned NuGet package. Within a single service (`Api → Application → Domain → Infrastructure`)
  `<ProjectReference>` is fine.
- **No cross-service database access.** Services talk to each other via Service Bus events or HTTP
  through the gateway — never a direct connection to another service's schema.
- **Clean Architecture per service**: `Api/`, `Application/` (CQRS commands, queries, handlers,
  validators), `Domain/` (entities, value objects, domain events), `Infrastructure/` (EF Core,
  repositories).

## Getting set up

```bash
git clone <this-repo>
cd erp-mono-repo
./scripts/bootstrap.sh   # packs shared libs into the local NuGet feed, restores SakhaERP.sln
docker compose up -d     # Postgres + Redis for the reference service
dotnet run --project services/inventory-service/src/InventoryService.Api
```

## Making a change

1. Open an issue first for anything beyond a small fix — architectural changes to shared
   libraries affect every service built on the framework, so it's worth agreeing on the shape
   before code is written.
2. Follow [Conventional Commits](https://www.conventionalcommits.org/) for commit messages
   (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`).
3. Add or update tests alongside the change — unit tests co-located with the project under test,
   integration tests in the service's `tests/` folder.
4. Run the build locally before opening a PR:
   ```bash
   dotnet build SakhaERP.sln
   dotnet test SakhaERP.sln
   ```
5. Open a PR against `main`. CI runs the same build/test/CodeQL checks that gate merges.

## Adding a new shared library or service

- A new `SakhaERP.Shared.*` package: add it under `shared/`, register its version in
  `Directory.Packages.props`, and consume it from services via `PackageReference` only.
- A new service: `dotnet new install ./templates/sakhaerp-service` (once), then
  `dotnet new sakhaerp-service -n YourService -o services/your-service`. See
  [`templates/sakhaerp-service/README.md`](templates/sakhaerp-service/README.md).

## Using Claude Code on this repo

[`CLAUDE.md`](CLAUDE.md) is the entry point — it points to the architecture rules, the four
subagents in [`.claude/agents/`](.claude/agents/) (routed by task complexity, not always the
biggest model), the curated skills in [`.claude/skills/`](.claude/skills/), and
[`docs/development/claude-code-workflow.md`](docs/development/claude-code-workflow.md) for
token-efficiency and cross-session memory practices. If you're extending that setup — adding a
skill, an agent, or a doc — keep `CLAUDE.md`/`AGENTS.md` themselves short; put detail in a linked
file instead, since those two load into every session unconditionally.

## Reporting issues

Security issues: please do not open a public issue — see [`SECURITY.md`](SECURITY.md) (or the
repository's Security tab) for how to report privately. Everything else: GitHub Issues, with
enough detail (repro steps, expected vs. actual) for someone else to act on it without asking
follow-up questions.
