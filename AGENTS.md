# SakhaERP — Claude Code Guide

SakhaERP is a .NET 9 framework for multi-tenant, event-driven microservices: Clean Architecture
per service, CQRS via MediatR, versioned shared libraries instead of shared source,
schema-per-tenant multi-tenancy. `services/inventory-service` is the reference implementation —
read it to see the conventions applied, not as a spec for an inventory system.

## Read first

1. [`docs/architecture/framework-rules.md`](docs/architecture/framework-rules.md) — the
   non-negotiable rules. The one that matters most: **never `<ProjectReference>` across a service
   or shared-library boundary, `PackageReference` only.** This was violated by mistake once
   already in this codebase and had to be fixed — don't repeat it.
2. [`docs/architecture/README.md`](docs/architecture/README.md) — index of the rest (tech stack,
   multi-tenancy, security, resilience, DTO conventions). Read the one doc relevant to your task,
   not all of them.
3. [`CONTRIBUTING.md`](CONTRIBUTING.md) — dev setup and PR process.

## Route work by complexity, not by default to the biggest model

| Task | Agent | Model |
|---|---|---|
| Planning a new service, a shared-library change, or anything crossing service boundaries | `sakhaerp-planner` | Opus |
| Implementing a decided change — handlers, EF Core, validators, endpoints | `sakhaerp-developer` | Sonnet |
| Reviewing a diff/PR against the architecture rules and test coverage | `sakhaerp-qa-reviewer` | Sonnet |
| Small mechanical changes — renames, boilerplate, typos, config values | `sakhaerp-quick-task` | Haiku |

Definitions in [`.claude/agents/`](.claude/agents/). Full guidance — including token-efficiency
and memory practices for a fast start in a new session — in
[`docs/development/claude-code-workflow.md`](docs/development/claude-code-workflow.md).

## Testing is not optional

TDD: failing test first, then the minimum code to pass it (`test-driven-development` skill).
`dotnet test SakhaERP.sln` passing and `dotnet build SakhaERP.sln` at 0 warnings are the bar
before calling anything done — see `verification-before-completion` skill, don't claim a fix
works without having run it.

## Skills available

`.claude/skills/` — invoke the one that matches the task instead of ad hoc exploration:
`test-driven-development`, `systematic-debugging`, `verification-before-completion`,
`architecture-decision-record`, `using-git-worktrees`, `writing-plans`, `executing-plans`,
`find-bugs`, `finishing-a-development-branch`, `web-design-guidelines`, `react-best-practices`,
`brainstorming`, `webapp-testing`, `dispatching-parallel-agents`.

## Scaffolding a new service

```bash
dotnet new install ./templates/sakhaerp-service
dotnet new sakhaerp-service -n YourService -o services/your-service
```

See [`templates/sakhaerp-service/README.md`](templates/sakhaerp-service/README.md) for what to do
next.

---

## GitNexus (optional — only if you've configured it for this clone)

Everything above is the source of truth regardless of whether GitNexus is set up. If you *have*
run `npx gitnexus analyze` against this repo, the block below (auto-managed by that tool) adds
call-graph-aware impact analysis and safe renames on top. If you haven't, ignore it — nothing else
in this repo depends on it.

<!-- gitnexus:start -->
# GitNexus — Code Intelligence

This project is indexed by GitNexus as **erp-mono-repo** (1267 symbols, 2170 relationships, 7 execution flows). Use the GitNexus MCP tools to understand code, assess impact, and navigate safely.

> If any GitNexus tool warns the index is stale, run `npx gitnexus analyze` in terminal first.

## Always Do

- **MUST run impact analysis before editing any symbol.** Before modifying a function, class, or method, run `gitnexus_impact({target: "symbolName", direction: "upstream"})` and report the blast radius (direct callers, affected processes, risk level) to the user.
- **MUST run `gitnexus_detect_changes()` before committing** to verify your changes only affect expected symbols and execution flows.
- **MUST warn the user** if impact analysis returns HIGH or CRITICAL risk before proceeding with edits.
- When exploring unfamiliar code, use `gitnexus_query({query: "concept"})` to find execution flows instead of grepping. It returns process-grouped results ranked by relevance.
- When you need full context on a specific symbol — callers, callees, which execution flows it participates in — use `gitnexus_context({name: "symbolName"})`.

## Never Do

- NEVER edit a function, class, or method without first running `gitnexus_impact` on it.
- NEVER ignore HIGH or CRITICAL risk warnings from impact analysis.
- NEVER rename symbols with find-and-replace — use `gitnexus_rename` which understands the call graph.
- NEVER commit changes without running `gitnexus_detect_changes()` to check affected scope.

## Resources

| Resource | Use for |
|----------|---------|
| `gitnexus://repo/erp-mono-repo/context` | Codebase overview, check index freshness |
| `gitnexus://repo/erp-mono-repo/clusters` | All functional areas |
| `gitnexus://repo/erp-mono-repo/processes` | All execution flows |
| `gitnexus://repo/erp-mono-repo/process/{name}` | Step-by-step execution trace |

## CLI

| Task | Read this skill file |
|------|---------------------|
| Understand architecture / "How does X work?" | `.claude/skills/gitnexus/gitnexus-exploring/SKILL.md` |
| Blast radius / "What breaks if I change X?" | `.claude/skills/gitnexus/gitnexus-impact-analysis/SKILL.md` |
| Trace bugs / "Why is X failing?" | `.claude/skills/gitnexus/gitnexus-debugging/SKILL.md` |
| Rename / extract / split / refactor | `.claude/skills/gitnexus/gitnexus-refactoring/SKILL.md` |
| Tools, resources, schema reference | `.claude/skills/gitnexus/gitnexus-guide/SKILL.md` |
| Index, status, clean, wiki CLI commands | `.claude/skills/gitnexus/gitnexus-cli/SKILL.md` |

<!-- gitnexus:end -->
