# Working with Claude Code on SakhaERP

This is the "how to drive Claude Code efficiently on this repo" doc. `CLAUDE.md` and `AGENTS.md`
at the repo root point here rather than inlining this — per Anthropic's own guidance, those files
load into every session unconditionally, so they stay short and this stays detailed.

## Route work to the right subagent, not the biggest model by default

Four subagents are defined in [`.claude/agents/`](../../.claude/agents/), each pinned to the model
that fits its risk/complexity, not the most capable one available:

| Agent | Model | Use for |
|---|---|---|
| `sakhaerp-planner` | **Opus** | New services, changes to any `SakhaERP.Shared.*` library, multi-tenancy behavior, anything crossing service boundaries. A wrong architectural call here is expensive to unwind later — worth the strongest model. |
| `sakhaerp-developer` | **Sonnet** | Implementing a decided plan: CQRS handlers, EF Core configs, validators, endpoints. The bulk of feature work lands here. |
| `sakhaerp-qa-reviewer` | **Sonnet** | Reviewing a diff/PR against the framework's architecture rules, tenant isolation, and test coverage before it merges. |
| `sakhaerp-quick-task` | **Haiku** | Renames, boilerplate scaffolding from an existing pattern, typo fixes, config value updates — genuinely small, low-risk work. |

The rule of thumb: **planning and irreversible architectural decisions get the most capable
model; well-specified implementation gets a cheaper one; mechanical work gets the cheapest one
that can still do it correctly.** If a "quick task" turns out to touch multi-tenancy or a shared
library once you're in it, that's a signal to hand it to `sakhaerp-developer` or
`sakhaerp-planner`, not to push through on a cheaper model.

## Token efficiency — context engineering, not prompt engineering

The framing that matters more than any individual trick: **treat the context window as a scarce,
curated resource, not a scratchpad.** Concretely, on this repo:

- **Delegate research to subagents.** If you need to understand how tenant resolution flows
  through `SakhaERP.Shared.Security` before making a change elsewhere, dispatch a subagent to read
  it and report back a summary — don't pull five files into the main conversation to answer a
  question that only needs the conclusion. Subagents run in their own context window; only their
  final report lands in yours.
- **Read the specific doc, not the whole `docs/architecture/` folder.** `docs/architecture/README.md`
  is an index for a reason — go to `multi-tenancy.md` for tenancy questions, not all nine files.
- **Scope tasks narrowly and start fresh sessions for unrelated work.** A session that drifts from
  "fix this validator" to "also let's discuss the shared library versioning strategy" accumulates
  context that's irrelevant to both topics by the end. Long sessions don't just cost more tokens —
  quality degrades as earlier instructions lose weight against everything piled on top (context
  rot). If you're switching topics, switch sessions.
- **Don't re-read what's already in context.** If you just wrote or edited a file, you know what's
  in it — re-reading it to "double check" before the next edit is wasted tokens most of the time
  (exceptions: after a tool reports the file changed on disk since you last read it).
- **Measure what matters.** Token cost per completed task or PR is a more useful signal than total
  session spend — a session that burns more tokens but ships a correct, tested change in one pass
  is more efficient than a cheap one that needs three follow-up fixes.

## Memory for a fast start in a new session

Claude Code supports persistent, cross-session memory. For a framework repo like this one — where
a contributor might not touch it again for weeks — that memory is the difference between Claude
re-deriving "don't use `<ProjectReference>` across service boundaries" from scratch each time and
just knowing it.

**Set memory up deliberately, not ad hoc.** Worth persisting, as you work on this repo:

- Architecture decisions made along the way that aren't yet written into `docs/architecture/`
  (if they should be, put them there instead — memory is for things that don't belong in a
  committed doc, like *why* a particular approach was chosen for a still-open question).
- Debugging insights on anything gnarly (a subtle EF Core query-filter interaction, a
  multi-tenancy edge case) that would otherwise get re-discovered the hard way next session.
- Conventions established but not yet formalized in `docs/` — e.g. a naming pattern used in a new
  service that should eventually get written up.

**Not worth persisting** (it's already recoverable from the repo itself, so storing it stales
almost immediately): file paths, current test-pass/fail state, in-progress task lists — use `git
log`/`git status`/the actual code for those, not memory.

**Never persist secrets or credentials** into memory files — same rule as everywhere else in this
codebase (see [`SECURITY.md`](../../SECURITY.md)).

## Session hygiene checklist

- [ ] One topic per session where reasonably possible.
- [ ] New/non-trivial work: plan first (`sakhaerp-planner` or the `writing-plans` skill), then
      implement, don't skip straight to code for anything crossing a boundary.
- [ ] TDD: failing test first (`test-driven-development` skill), then the minimum code to pass it.
- [ ] Before claiming something is fixed or done: actually run it
      (`verification-before-completion` skill) — a build or test result you haven't seen isn't
      evidence.
- [ ] Delegate exploratory reading to a subagent when the answer, not the raw file contents, is
      what the main conversation needs.
