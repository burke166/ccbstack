# 04 — Shared cross-cutting `CLAUDE.md` sections for `init-repo` and `adopt-repo`

Status: Implemented

## Problem

`docs/design/03_shared-design-doc-guidance.md` fixed one specific gap: both
skills wrote only a one-line design-first mention into a generated project's
`CLAUDE.md`, while `docs/examples/ai_instructions/` contains real,
hand-written `CLAUDE.md` files with much fuller guidance for that same
topic. The fix was a shared `skills/references/design-docs.md` both skills
load and adapt.

Comparing all four example files (`karl_claude-md.md`, a .NET email
framework; `computercodeblue-csv_claude-md.md`, a small .NET utility
library; `megamekversionmanager_claude-md.md`, a Windows WPF app;
`lcmeg_hugo_claude-md.md`, a Hugo static website) shows several more
sections recurring across most or all of them. Some of that recurrence is
genuinely useful — the same *concern* applies broadly and the concrete
content differs meaningfully per project (e.g. integration testing: Karl's
concern is not hitting a live SMTP server, MegaMekVersionManager's is not
hitting live GitHub/Adoptium downloads — same shape, different specifics
worth stating explicitly per project).

But not all of the recurrence is like that. Several sections are
near-verbatim across three of the four examples (Karl, Csv,
MegaMekVersionManager) not because each project independently arrived at
the same conclusion, but because all three were authored by the same
person reusing a personal template. Sections like "measure first, optimize
second" or "reliability over features" read as generic software-engineering
sentiment rather than information specific to the project — the kind of
content ccbstack's own `CLAUDE.md` explicitly warns against for generated
projects: *"Avoid boilerplate that does not provide Claude useful
information"* and *"Repository-specific instructions should reflect
decisions actually made for that repository rather than generic ccbstack
preferences that do not apply."* Auto-generating that content into every
`init-repo`/`adopt-repo` project would reproduce a habit, not encode a
decision — diluting the genuinely load-bearing, project-specific parts of a
generated `CLAUDE.md` with text a competent engineer (or Claude, by
default) would already follow without being told.

This revision narrows the original proposal to the sections that pass a
concrete test: does this content change what gets proposed, written, or
tested for *this* project, or would the same paragraph fit unchanged into
any project regardless of what it actually does?

## Goals

* Extract the sections that are genuinely reusable *and* concretely
  actionable — content that changes a specific decision or action, not
  general values — into shared resources, following
  `skills/references/design-docs.md`'s existing shape: both skills load and
  adapt them, content lives in exactly one place.
* Preserve the "same shape, different specifics" pattern for Integration
  Testing and the other genuinely conditional sections — the shared
  resource should teach the *pattern* (identify this project's own
  external/remote dependency; write guidance around *that*), not hand back
  one fixed example list.
* Explicitly exclude generic engineering-values boilerplate that doesn't
  meet that bar, even though it appears in most of the real examples —
  its presence there reflects one author's reused personal template, not
  evidence that auto-generating it adds value.

## Non-Goals

* Rewriting the six existing per-family `references/<family>.md` files'
  scaffolding, detection, or family-specific `.gitignore`/verification
  content. This only touches their `CLAUDE.md guidance` subsections.
* Forcing every generated `CLAUDE.md` to contain every section discussed
  here. Applicability varies by project, exactly as it does across the four
  real examples.
* Reproducing every section found in the four examples. Several are
  deliberately left out — see "Sections considered and excluded" below.
* A general-purpose "`CLAUDE.md` template engine" or new tooling.
* Changing ccbstack's own root `CLAUDE.md`. This only affects content
  written into *generated* projects.

## Current State

Section-by-section comparison of the four examples (headings as they
actually appear; content described, not quoted in full).

### Sections kept — concrete and actionable

* **Security (baseline)** — "treat external/user input as untrusted,
  validate before processing, least privilege, never hardcode secrets" —
  present in all four, each adapted to what "input" and "secrets" mean for
  that project (CSV input / AWS and SMTP credentials / API keys). Concrete:
  it prescribes specific handling of a specific recurring risk.
* **Dependencies** — a three-question checklist in Karl/Csv/MMVM ("is it
  necessary? is it maintained? can we reasonably build it ourselves?") and
  an explicit avoid-list in lcmeg. Concrete: it's a decision procedure that
  changes whether a package gets proposed.
* **Git** — "keep commits focused, don't mix formatting with functional
  changes, avoid mass rewrites, preserve file history" — word-for-word in
  Karl, Csv, and MMVM, and the core of a longer Git section in lcmeg.
  Concrete: it's a specific commit-hygiene rule, not a value statement.
* **Documentation (code-level)** — "public APIs should be documented,
  complex code deserves comments, simple code shouldn't, update docs when
  architecture changes" — present in Karl/Csv/MMVM, and lcmeg expresses the
  same idea through its own content-specific documentation rules. Concrete
  enough (a specific rule about *when* to comment) to be worth stating, and
  distinct from `design-docs.md`'s process-level documentation guidance —
  both live under a "Documentation" heading in the real examples but are
  different concerns.
* **Repository Preferences / Technology Stack** — every example states
  preferred technologies and explicitly calls out what to avoid introducing
  and why. The content is entirely project-specific, but the *instruction*
  to always include such a block, using facts already gathered during
  family selection/inspection, is reusable and worth stating explicitly.

### Sections considered and excluded — generic, not project-specific

These appear close to verbatim across Karl, Csv, and MMVM, which is
evidence of a shared personal template rather than evidence that
auto-generating them adds value. Each reads as advice any competent
engineer (or Claude, unprompted) already follows, and none of it changes
based on what the project actually is:

* **Design Philosophy** (a numbered priority list — Simplicity, Correctness,
  Maintainability, Performance) — the *pattern* of stating a priority order
  only has value when it reflects a real, deliberated tradeoff for that
  project (lcmeg's own ordering, e.g. putting Accessibility second because
  it's a public-sector site, is a real decision); a default order applied
  uniformly is not.
* **Development/Maintainability Philosophy** ("assume ten years of service,
  a future engineer should understand it in five minutes") — sentiment
  already implied by ccbstack's own root `CLAUDE.md` Core Principles; adds
  no new information per project.
* **AI Expectations / Agent Guidance** ("understand existing code first,
  extend rather than invent, ask when uncertain, search before assuming")
  — substantially overlaps with ccbstack's own "Inspect Before Changing"
  principle and Claude Code's own default behavior; repeating it per
  project is redundant, not incremental.
* **Coding Style** (small classes/functions, avoid boolean-flag parameters,
  avoid deep inheritance, favor composition, avoid static mutable state) —
  general OOP taste that doesn't naturally generalize to Go, Svelte,
  WordPress, or static HTML5, and isn't grounded in what the project's
  actual code looks like when auto-generated.
* **Refactoring** ("improve code when touching it, avoid drive-by
  refactors, propose large refactors before implementing") — procedural
  etiquette, not project-specific information.
* **Performance** ("measure first, optimize second, document significant
  optimizations") — same: true of nearly every project, changes nothing
  about how work on *this* project should proceed differently.
* **Project Priorities** ("reliability over features, correctness over
  speed...") — an inspirational value statement, verbatim identical across
  Karl/Csv/MMVM; too abstract to change a specific decision.
* **Testing (core)** ("every bug fix should include a regression test when
  practical, test observable behavior") and **Error Handling (core)**
  ("prefer explicit failures, don't silently swallow exceptions") — closer
  to the line than the rest of this list, but still describe practice a
  competent engineer already follows by default rather than a project
  ‑specific decision. Left out for consistency with the rest of this
  category; nothing prevents `adopt-repo` from documenting either as a real
  section if inspection finds the project actually has an explicit,
  non-default policy here (e.g. a documented flaky-test quarantine process,
  or a specific error-handling convention visible in the code).

### Sections kept — conditional on the project actually having the concern

These are present only where the project genuinely has the underlying
concern, with specifics that differ enough between projects that a shared,
adaptable pattern is worth more than silence:

* **Automation** — "this project talks to external systems; assume APIs
  fail, networks disconnect, rate limits exist; degrade gracefully" — in
  Karl (SMTP/email providers) and MMVM (GitHub/Adoptium downloads), absent
  from Csv (no network I/O).
* **Integration Testing** — the section the user specifically raised: Karl
  folds it into "Testing" ("avoid tests that depend on external SMTP
  servers... unless explicitly marked as integration tests"). MMVM gives it
  a full standalone section with a three-tier testing pyramid (unit →
  integration → end-to-end) and a concrete list of high-value integration
  tests specific to *its* external dependency. Csv has no such section — it
  has no remote dependency to test against. The shape is reusable; the
  concrete bullet list is inherently project-specific.
* **Logging** — present in Karl and MMVM, each with a project-specific list
  of what to log; absent from Csv (no runtime logging in a pure utility
  library).
* **Configuration** — present in Karl and MMVM ("configuration belongs
  outside code, strongly typed options, validate at startup, fail fast"),
  absent from Csv (no runtime configuration).

The existing per-family `references/<family>.md` "CLAUDE.md guidance"
sections (checked directly in `skills/init-repo/references/dotnet.md` and
`skills/adopt-repo/references/dotnet.md`) already mention Security,
Dependencies, and Git, but only as single bullet points — the same
thinness `03` fixed for the design-first mention, not yet fixed here for
the sections kept in this design.

## Proposed Design

Add two new shared resources under `skills/references/` (sibling to the
existing `design-docs.md`):

### `skills/references/engineering-values.md` — narrow, universal, concrete

Covers only the five sections kept above:

* Security baseline (untrusted input, least privilege, never hardcode
  secrets) — generic enough to adapt regardless of what "input" or
  "secrets" mean for a given project.
* Dependencies (the three-question checklist).
* Git (the four-line hygiene block).
* Documentation (code-level: public APIs documented, comments proportional
  to complexity) — explicitly distinguished from `design-docs.md`'s
  process-level documentation guidance.
* Technology Stack / Repository Preferences — not boilerplate text, but an
  explicit instruction: state the project's preferred technologies and
  call out anything intentionally avoided (and why), using facts already
  gathered in the family-selection/inspection steps rather than inventing
  new ones.

Deliberately excludes Design Philosophy, Development/Maintainability
Philosophy, AI Expectations, Coding Style, Refactoring, Performance,
Project Priorities, and the "core" halves of Testing/Error Handling, per
"Sections considered and excluded" above. `adopt-repo` may still document
any of these if inspection finds the repository has a real, specific,
non-default policy in one of those areas — that's ordinary "document what's
actually there," not this shared file's job.

### `skills/references/external-integrations.md` — conditional

Unchanged from the original proposal. States its own applicability test up
front: include this content only when the project talks to an
external/remote/networked system, or has meaningful runtime configuration
or logging — skip entirely for a project like Csv with none of that.

* Automation (generic enough to adapt to whatever the project's actual
  external dependency is).
* Integration Testing — teach the *shape* (favor fakes/in-memory for unit
  tests; reserve live external systems for integration/manual/end-to-end
  tests; a unit → integration → end-to-end testing-pyramid structure) plus
  an explicit instruction to identify *this* project's own external/remote
  dependency and write the concrete high-value integration-test bullet
  list around that dependency specifically — the way Karl's list is about
  email delivery and MMVM's is about downloads and Java runtime
  management — not a fixed example list to copy.
* Logging (what categories of event are normally worth logging; never log
  secrets or tokens) — a pattern ("log meaningful state transitions and
  external failures relevant to *this* project"), not one fixed list.
* Configuration (config outside code, strongly typed options, validate at
  startup, fail fast) — for projects with runtime configuration at all.

### Both `SKILL.md` files updated

* `init-repo` step 8 and `adopt-repo` step 7 each gain instructions to load
  `skills/references/engineering-values.md` (small and close to always
  applicable — adapt into the generated `CLAUDE.md`) and to load
  `skills/references/external-integrations.md` only when the project
  actually integrates with external/remote systems or has meaningful
  runtime configuration/logging, determined from the family and questions
  already gathered (`init-repo`) or from what inspection actually found
  (`adopt-repo`) — not guessed.
* `adopt-repo`'s "Improving an existing `CLAUDE.md`" rules apply here the
  same way they already apply to the design-first section: a thin or
  missing section is a gap to fill using the shared reference; an existing,
  clearly intentional section is left alone even if it differs from what
  the shared reference would produce.
* The `dotnet.md` (and other family) `references/*.md` files' `CLAUDE.md
  guidance` sections are trimmed of the bullets now covered by
  `engineering-values.md` (Security, Dependencies, Git), replaced with a
  pointer to the shared file. Family-specific content that doesn't
  generalize (target framework conventions, build commands, detection
  signals) stays exactly where it is.

## Alternatives Considered

* **Include the full engineering-values set as originally drafted**
  (Design Philosophy, Development Philosophy, AI Expectations, Coding
  Style, Refactoring, Performance, Project Priorities, Testing/Error
  Handling cores). Rejected on reflection — none of it changes a specific
  decision per project, it substantially overlaps ccbstack's own root
  `CLAUDE.md` and Claude's default behavior, and its near-verbatim presence
  across three examples reflects one author's reused template rather than
  independently-arrived-at, project-specific value. Generating it into
  every project would be exactly the "boilerplate that does not provide
  Claude useful information" ccbstack's own `CLAUDE.md` warns against.
* **One combined shared file instead of two.** Rejected — universal and
  conditional content would be interleaved, and a reader (or the skill
  itself, deciding what to include) would have to re-derive which parts
  apply to a given project instead of the file structure making that
  obvious at a glance.
* **One file per individual section.** Rejected as excessive fragmentation
  for content that's always considered together — `03` established one
  file per genuinely distinct *topic*; the narrowed
  `engineering-values.md` is small enough that further splitting would add
  file-juggling overhead without benefit.
* **Fold this content into each family's `references/<family>.md` instead
  of a shared file.** Rejected for the same reason `03` rejected it for the
  design-first section: it would duplicate identical content across six
  families × two skills, with nothing to keep the copies in sync.

## Implementation Plan

1. Create `skills/references/engineering-values.md` with the five sections
   described under "Proposed Design" above, generalized from the kept
   examples, written to be adapted per project rather than copied verbatim
   — matching `design-docs.md`'s existing style and intro framing. Keep it
   short; this file should stay noticeably smaller than
   `design-docs.md`.
2. Create `skills/references/external-integrations.md` with the sections
   described under "conditional" above, opening with an explicit
   applicability test, and with the Integration Testing section written as
   a pattern-plus-instruction rather than Karl's or MMVM's fixed example
   list.
3. Edit `skills/init-repo/SKILL.md` step 8 and `skills/adopt-repo/SKILL.md`
   step 7 to load and apply both new files per the rules above, alongside
   the existing `design-docs.md` instruction added in `03`.
4. Trim the `CLAUDE.md guidance` sections of each `references/<family>.md`
   file (both skills, all six families) to remove the now-duplicated
   one-line Security/Dependencies/Git bullets, replacing them with a
   pointer to `engineering-values.md`; leave family-specific bullets
   (target framework, build commands, detection signals) in place.
5. Update `CHANGELOG.md` documenting the two new shared files and the
   trimmed per-family references.

## Implementation Notes

Step 4 of the plan called for trimming `references/<family>.md` files in
both skills. On inspection, `adopt-repo`'s six family reference files
turned out to have no Security or Git bullets to trim at all, and their
`Dependencies` bullets are inspection-focused ("document what's actually
there") rather than restating the generic before-adding-a-package
checklist — genuinely different content from what `engineering-values.md`
now covers, not duplication. Only `init-repo`'s six family reference files
needed trimming; `adopt-repo`'s were left unchanged.

`init-repo/references/wordpress.md`'s WordPress-specific security guidance
(escaping output, using nonces) and its plugin-dependency guidance were
kept, trimmed only of the generic "never hardcode credentials" clause now
covered by `engineering-values.md` — genuinely family-specific content, not
duplication.

## Validation

* Re-read both edited `SKILL.md` files end-to-end for consistency with the
  rest of each skill's workflow, the same check performed for `03`.
* Manually walk `init-repo` and/or `adopt-repo` against disposable test
  repositories again, choosing at least one project that genuinely touches
  an external/remote system (to confirm `external-integrations.md` gets
  pulled in and adapted, not copied verbatim) and one that doesn't (to
  confirm it's correctly omitted, the way it's absent from the real Csv
  example).
* Confirm the generated `CLAUDE.md`'s `engineering-values.md`-derived
  content stays short and doesn't reintroduce any of the excluded generic
  sections.
* Confirm the generated `CLAUDE.md`'s Integration Testing content (where
  applicable) is genuinely specific to that test project's own external
  dependency, not a restatement of Karl's SMTP example or MMVM's download
  example.
* `git status`/`git diff` review before considering the change complete.
