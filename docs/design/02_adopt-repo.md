# 02 — `adopt-repo`

Status: Implemented

## Problem

Most repositories ccbstack will touch already exist — they weren't created with
`init-repo` and don't yet have a project-specific `CLAUDE.md`, `docs/design/`, or
a `.gitignore` that reflects ccbstack's conventions. Bringing such a repository
into the ccbstack workflow today means manually inspecting it, writing a
`CLAUDE.md` by hand, and remembering which standard files are missing — the same
kind of easy-to-skip, easy-to-do-inconsistently work `init-repo` solves for new
repositories, but harder, because an existing repository already has real
conventions, real history, and possibly work in progress that must not be
disturbed.

`adopt-repo` needs to produce the same useful outcome as `init-repo` —
an accurate project-specific `CLAUDE.md`, `docs/design/`, and sane `.gitignore` —
without assuming a blank slate. It must inspect before changing anything (per
`CLAUDE.md`'s "Inspect Before Changing" principle), preserve intentional existing
decisions, and never force an existing project into `init-repo`'s scaffold
choices merely because that's what a new project would have gotten.

## Goals

* Inspect an existing repository's purpose, languages, frameworks, build
  system, application type, structure, and conventions before making any
  change.
* Create a project-specific `CLAUDE.md` when none exists, or *improve* one that
  already exists — filling real gaps and correcting inaccuracies without
  discarding intentional existing content.
* Create missing standard files (`README.md`, `CHANGELOG.md`, `docs/design/`)
  only where genuinely missing; never require or overwrite what's already
  there.
* Create or improve `.gitignore` by merging in missing, stack-appropriate
  entries — never overwriting existing custom entries.
* Verify the build/test commands recorded in the generated or updated
  `CLAUDE.md` actually work, where doing so is quick and safe.
* Report exactly what changed, what was added, and what was intentionally left
  alone — and why.
* Reuse the same six project families `init-repo` understands (C#/.NET, Go,
  Hugo, Svelte, WordPress themes, static HTML5) as the primary detection
  vocabulary, with a generic fallback for anything else, so the two skills
  describe the ccbstack-supported world consistently.

## Non-Goals

* Restructuring an existing repository's file layout, build configuration, or
  architecture to match what `init-repo` would have scaffolded for a new
  project. Working conventions are preserved even when they differ from
  `init-repo`'s defaults.
* Fixing bugs, refactoring code, or making functional changes. `adopt-repo`
  touches documentation and repository-workflow files
  (`CLAUDE.md`/`README.md`/`CHANGELOG.md`/`.gitignore`/`docs/design/`) — not
  application code.
* Reconstructing historical `CHANGELOG.md` entries from Git history. A missing
  `CHANGELOG.md` gets a fresh "Unreleased" seed, not a backfilled history.
* Committing changes automatically. Unlike `init-repo` (which is establishing a
  brand-new repository's first commit), `adopt-repo` runs against a repository
  that already has history — and possibly uncommitted work — so it follows
  ccbstack's normal "don't commit unless asked" practice instead of
  `init-repo`'s scoped exception.
* Fully modeling or redesigning an arbitrarily complex monorepo's build
  orchestration. `adopt-repo` detects and documents what languages/frameworks
  are present; it doesn't decide how a multi-project repository *should* be
  organized.
* Supporting project families beyond the six `init-repo` already understands
  plus a generic fallback. More families can be added to both skills together
  later, following the same pattern.

## Current State

`skills/init-repo/` now exists and establishes the pattern this design follows:
a `SKILL.md` holding the shared workflow, plus one `references/<family>.md` per
project family holding technology-specific detail. `.claude-plugin/plugin.json`
already exists at the repository root, so `adopt-repo` has no distribution
prerequisite of its own to add.

There is no `skills/adopt-repo/` yet, and no shared, cross-skill resource
location currently exists in ccbstack — each skill's `references/` lives inside
that skill's own directory (`CLAUDE.md`'s "Skill Resources" convention).

ccbstack's own `CLAUDE.md` (root) and `README.md` already describe `adopt-repo`
at a high level: inspect purpose/languages/frameworks/build system/application
type/structure/conventions; create or improve `CLAUDE.md`; create missing
standard documentation and ccbstack folders; create or improve `.gitignore`;
avoid unnecessary restructuring; verify where possible; report what changed and
why. This design turns that into a concrete workflow, the same way
`docs/design/01_init-repo.md` did for `init-repo`.

## Proposed Design

### Workflow

`adopt-repo` runs as a linear flow, but unlike `init-repo` most steps *read*
before they *write* — the whole point is acting on evidence instead of asking
cold.

1. **Confirm the target repository.** Default to the current working
   directory. If it doesn't look like a repository root (no recognizable
   project files, or the user names a different path), ask for the correct
   location rather than guessing.

2. **Initialize Git only if genuinely absent.** If `.git` doesn't exist, run
   `git init` so the repository can participate in the ccbstack workflow at
   all; if it already exists, never touch Git history. This is the one
   `init-repo`-like action in the flow, and only because everything downstream
   (`.gitignore`, reporting what changed via `git status`) assumes Git is
   present.

3. **Inspect the repository before changing anything.** Read the existing
   file tree, package/build manifests, existing documentation
   (`README.md`, `CHANGELOG.md`, `CLAUDE.md`, `.gitignore`, `docs/`), test
   setup, and CI configuration if present. Build a picture of: what the
   project is for, what language(s)/framework(s) it uses, how it's normally
   built/tested/formatted (from actual scripts/Makefiles/CI config/README
   instructions — not assumed defaults), and what conventions already exist.
   Do not write anything in this step.

4. **Match against a project family, and confirm rather than guess.** Compare
   what was found in step 3 against the same six families `init-repo`
   understands (see "Reference table" below) plus "other/generic." Propose the
   detected family (or families, for a mixed-language repository — see
   "Multi-language repositories" below) to the user via `AskUserQuestion` for
   confirmation rather than asking cold like `init-repo` does, since there's
   real evidence to present. Correct the detection if the user says it's
   wrong.

5. **Create or improve `.gitignore`.** If missing, create one from the
   confirmed family's typical entries (per `references/<family>.md`). If
   present, merge in missing stack-appropriate entries without touching
   existing custom entries, and report exactly what was added.

6. **Create `docs/design/` if missing.** Seed it with a short
   `docs/design/README.md` pointing at the design-first workflow — Git doesn't
   track empty directories, a real gap `init-repo`'s testing already found and
   fixed the same way. If `docs/design/` already exists (with or without
   documents in it), leave it untouched.

7. **Create or improve `CLAUDE.md`.** If none exists, generate one using the
   same content checklist `init-repo` uses (purpose, language/framework,
   architecture, dependencies, build/test commands, principles, conventions,
   documentation expectations, Git practices, testing expectations,
   project-specific constraints, design-first workflow) — but grounded in
   what step 3 actually found, not fresh choices. If a `CLAUDE.md` already
   exists, *improve* it: keep everything that states an intentional decision,
   add sections that are genuinely missing and have an obvious answer from
   inspection, and flag (rather than silently rewrite) anything that looks
   inaccurate or contradicts what step 3 found — see "Improving an existing
   `CLAUDE.md`" below for how to tell "existing decision" from "gap."

8. **Create missing `README.md`/`CHANGELOG.md`.** Only if genuinely absent —
   write a minimal, project-specific version the same way `init-repo` does.
   Never rewrite an existing `README.md` or `CHANGELOG.md` uninvited; at most,
   note in the final report if either looks significantly out of date
   relative to what step 3 found.

9. **Verify where it's quick and safe.** Run the build/test command recorded
   in the (new or updated) `CLAUDE.md` if doing so is fast and has no
   destructive/networked side effects beyond a normal local build (e.g.
   `dotnet build`, `go build ./...`, `npm run build`). If verification isn't
   safe or practical to run automatically (e.g. it needs infrastructure,
   credentials, or a long-running process), say so explicitly in the report
   instead of silently skipping it.

10. **Report.** Summarize, file by file: what was created, what was changed
    and why, and what was found but intentionally left alone. Do not commit —
    leave staging and committing to the user, per ccbstack's normal Git
    practice (see "Non-Goals").

### Reference table

Reuse the same family vocabulary as `init-repo`, with adoption-focused
reference content rather than scaffolding-focused content (see "Alternatives
Considered" for why these aren't the same files):

| Family | Reference |
|---|---|
| C#/.NET | `references/dotnet.md` |
| Go | `references/go.md` |
| Hugo | `references/hugo.md` |
| Svelte | `references/svelte.md` |
| WordPress theme | `references/wordpress.md` |
| Static HTML5 | `references/html5.md` |
| Other/generic | none — minimal fallback, same as `init-repo` |

Each reference file should cover, for that family: what files/structure
identify it during inspection (step 3); what its normal `.gitignore` entries
are; what a good `CLAUDE.md` for that stack typically documents; and what a
safe, quick verification command looks like (step 9).

### Improving an existing `CLAUDE.md`

This is the step with no `init-repo` equivalent and the one most likely to go
wrong, so it gets explicit rules:

* Treat a section that already states a clear decision as intentional — leave
  it, even if `init-repo` would have chosen differently for a new project.
* Treat a section that is entirely absent, where inspection found an
  unambiguous fact (e.g. no "Build" section, but a `Makefile` with an obvious
  `build` target), as a gap — fill it.
* Treat a section that appears to contradict what inspection found (e.g.
  `CLAUDE.md` says "no external dependencies" but a lockfile lists a dozen) as
  a discrepancy — do not silently resolve it either direction. Flag it in the
  report and ask if it's unclear which is authoritative.
* Never delete existing content to make room for the generated structure.
  `adopt-repo` adds and corrects; it doesn't reorganize prose style choices
  the user already made.

### Multi-language repositories

Unlike a freshly scaffolded `init-repo` project, an adopted repository may
genuinely span more than one family (e.g. a Go backend with a Svelte
frontend). Step 4 should surface every family it detects, not force a single
choice. The resulting `CLAUDE.md` should document each detected part of the
repository (build/test commands, conventions) rather than assuming one
language describes the whole project. `adopt-repo` does not attempt to decide
how such a repository *should* be organized (see "Non-Goals") — it documents
what's there.

## Alternatives Considered

* **Reuse `init-repo`'s existing `references/<family>.md` files directly**,
  instead of giving `adopt-repo` its own. Rejected: those files are
  scaffolding instructions ("run this command to create a new project"), not
  detection/adoption guidance ("here's how to recognize this stack and
  document what's already there"). Sharing them would create a hidden
  cross-skill dependency ccbstack's "Skill Independence" principle warns
  against, for content that's about half-overlapping at best. If real
  duplication pain shows up once both skills exist, extracting genuinely
  shared facts (e.g. "what does this family's `.gitignore` normally contain")
  into a common resource is a reasonable future step — but not a
  demonstrated need yet.
* **Ask the project family cold, the same way `init-repo` does**, instead of
  inspecting first and asking for confirmation. Rejected: `adopt-repo` has
  real evidence available before asking anything, and ignoring it would mean
  re-asking a question the repository can already answer for itself; a
  confirm-what-was-detected question is cheaper and more accurate than a
  cold one.
* **Auto-commit after changes, matching `init-repo`.** Rejected: `init-repo`'s
  auto-commit is a scoped exception justified by there being no prior history
  to protect. `adopt-repo` runs against repositories with real history and
  potentially uncommitted work, so ccbstack's normal "don't commit unless
  asked" practice applies instead.
* **Silently resolve `CLAUDE.md`/reality discrepancies** (e.g. always trust
  inspection over existing prose, or vice versa). Rejected: either default
  risks quietly erasing an intentional decision or quietly preserving stale,
  wrong documentation. Flagging the discrepancy for the user is slower but
  doesn't guess wrong in a way that's hard to notice later.

## Security and Privacy Considerations

* **Inspection reads repository content, including files that might contain
  secrets accidentally committed in the past** (e.g. a tracked `.env`).
  `adopt-repo` must not surface secret *values* in its report — it can note
  that a file like `.env` is tracked and probably shouldn't be, and suggest
  adding it to `.gitignore` (it won't stop being tracked just from a
  `.gitignore` change, which is worth saying in the report), without echoing
  its contents.
* **Verification (step 9) executes the repository's own build/test tooling.**
  This inherits whatever trust boundary already exists for that repository's
  build system (e.g. `npm install`/`go build` may fetch dependencies from
  package registries) — the same inherited trust boundary `init-repo`
  documents, not a new one `adopt-repo` introduces.
* **No credentials are requested, generated, or stored by this skill.**

## Implementation Plan

1. Implement `skills/adopt-repo/SKILL.md` per the "Workflow" section above,
   following the same structural pattern as `skills/init-repo/SKILL.md`
   (shared workflow, reference table, explicit failure/edge cases).
2. Implement the six `skills/adopt-repo/references/<family>.md` files
   (`dotnet.md`, `go.md`, `hugo.md`, `svelte.md`, `wordpress.md`, `html5.md`),
   each covering family detection signals, typical `.gitignore` entries,
   `CLAUDE.md` content guidance, and a safe verification command.
3. Cover the "other/generic" fallback directly in `SKILL.md` (no dedicated
   reference file), matching `init-repo`'s pattern.

This design is specific enough that implementation should not need to
re-decide the workflow, the family vocabulary, the no-auto-commit boundary, or
how `CLAUDE.md` improvement should behave — only fill in each family's
detection/documentation detail.

## Validation

**Acceptance criteria:**

* Running `adopt-repo` against a repository with no `CLAUDE.md` produces one
  that accurately reflects the actual detected language, build/test commands,
  and structure.
* Running `adopt-repo` against a repository with an existing, reasonable
  `CLAUDE.md` preserves its intentional content and only adds genuinely
  missing information.
* Running `adopt-repo` against a repository with an existing `.gitignore`
  merges in missing entries without disturbing existing ones.
* Running `adopt-repo` against a repository with no `docs/design/` creates it
  with a real (non-empty) seed file, not an empty directory that Git would
  drop.
* Running `adopt-repo` against a mixed-language repository documents each
  detected family rather than forcing a single choice.
* `adopt-repo` never stages or commits changes on its own.
* The final report accurately lists every file created or changed, and why.

**Testing approach:** Per `CLAUDE.md`'s "Testing Skills" section, test against
disposable repositories with different existing structures and conventions,
per each of the six families plus a deliberately messy/incomplete example and
a mixed-language example — verifying both the resulting files and that
nothing was overwritten that shouldn't have been. This is follow-up work to
perform once `skills/adopt-repo/` is implemented — it is out of scope for this
design document.
