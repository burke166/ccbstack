# 03 — Shared design-doc guidance for `init-repo` and `adopt-repo`

Status: Implemented

## Problem

`docs/examples/ai_instructions/lcmeg_hugo_claude-md.md` — a real, previously
written `CLAUDE.md` used as a calibration example by both `init-repo` and
`adopt-repo` — contains a detailed, largely project-agnostic description of
ccbstack's own design-first workflow: when a design document is required,
what one must contain, and how it moves through
Proposed → Accepted → Implemented. Its "Documentation", "When a Design
Document Is Required", "Design Document Structure", and "Design and
Implementation" sections cover this in real depth (roughly 170 lines).

Today, the design-first section that `init-repo` (`SKILL.md` step 8) and
`adopt-repo` (`SKILL.md` step 7) write into a *generated* project's
`CLAUDE.md` is a single line: "design docs live in `docs/design/`, status
Proposed → Accepted → Implemented, per ccbstack's own convention." Projects
scaffolded or adopted by ccbstack therefore get a much thinner version of
this guidance than the lcmeg example demonstrates is useful in practice.

Both skills need the same fuller guidance. Neither skill currently has a
place to put shared, non-technology-specific content: each skill's
`references/` directory is scoped to that skill and, for the six project
families, deliberately holds *different* content between `init-repo` and
`adopt-repo` (scaffolding-focused vs. adoption-focused) even when the files
share a name. There is no precedent yet for a resource both skills should
read identically.

## Goals

* Give both `init-repo` and `adopt-repo` access to the same detailed
  design-doc workflow guidance when generating a project's `CLAUDE.md`.
* Keep the guidance in exactly one place, so editing it once updates both
  skills' output and it cannot silently drift out of sync between them.
* Keep the two skills independently usable — neither should need to invoke
  or know about the other to do its job (`CLAUDE.md`'s "Skill Independence").
* Make the dependency on the shared content explicit in each `SKILL.md`,
  not hidden inside prose that happens to produce the same output.

## Non-Goals

* Changing ccbstack's own design-first workflow (this repository's own
  `CLAUDE.md` process). This only affects what gets written into *generated*
  projects' `CLAUDE.md` files.
* Reworking the six existing per-family `references/` directories, or the
  scaffolding-vs-adoption distinction between `init-repo`'s and
  `adopt-repo`'s same-named reference files. That distinction is intentional
  and unaffected by this change.
* Making the generated guidance identical, word-for-word, in every project's
  `CLAUDE.md`. Both skills already adapt reference content to the specific
  project rather than copying it verbatim (e.g. `init-repo` step 8's
  existing instruction to use real `CLAUDE.md` examples "to calibrate tone,
  depth, and section structure — not to copy their content"); this design
  keeps that adaptation model.

## Current State

* `skills/init-repo/SKILL.md` step 8 and `skills/adopt-repo/SKILL.md` step 7
  are the two places a generated `CLAUDE.md`'s design-first content is
  produced. Both currently reduce it to one sentence.
* `skills/init-repo/references/` and `skills/adopt-repo/references/` each
  hold six family-specific files (`dotnet.md`, `go.md`, `hugo.md`,
  `svelte.md`, `wordpress.md`, `html5.md`). Both `SKILL.md` files explicitly
  warn that same-named files between the two skills are *not*
  interchangeable — they serve different purposes (scaffolding vs.
  adoption/detection).
* There is no directory today at the `skills/` level that isn't owned by a
  single skill. `skills/` currently contains only `init-repo/` and
  `adopt-repo/`, each self-contained per `CLAUDE.md`'s "Skill Resources"
  section.
* `docs/examples/ai_instructions/lcmeg_hugo_claude-md.md` is the concrete
  source of the guidance to reuse. Its design-doc sections are already
  written generically enough (they talk about "the site," "the project," and
  general engineering process, not Hugo internals) to generalize with light
  editing rather than a rewrite from scratch.
* ccbstack's own root `CLAUDE.md` ("Repository Structure" section) documents
  the expected top-level layout, including `skills/init-repo/` and
  `skills/adopt-repo/`, but has no entry for a shared, skill-independent
  resource directory.

## Proposed Design

Extract the design-doc-workflow guidance into one new shared resource file,
and have both skills read it explicitly.

### New shared resource

Add `skills/references/design-docs.md` — sibling to `skills/init-repo/` and
`skills/adopt-repo/`, owned by neither. This placement follows `CLAUDE.md`'s
"Skill Independence" section directly: "Shared behavior may be extracted
into supporting resources when multiple skills genuinely require the same
rules." It is deliberately *outside* either skill's own `references/`
directory so it reads as shared infrastructure, not as one skill's private
resource that the other happens to borrow.

The file's content is a generalized distillation of the lcmeg example's
design-doc sections, meant to be adapted (not copied verbatim) into a
generated project's `CLAUDE.md`:

* When a design document is normally required vs. when routine changes don't
  need one (the lcmeg example's own worked list of examples for each side is
  a useful, reusable pattern).
* The `Proposed` → `Accepted` → `Implemented` status lifecycle and the rule
  that only a human approves `Accepted`, and that `Implemented` requires
  both implementation and verification to be complete.
* The standard document section structure (`Problem`, `Goals`, `Non-Goals`,
  `Current State`, `Proposed Design`, `Alternatives Considered`, `Security
  and Privacy Considerations`, `Implementation Plan`, `Validation`) and what
  each section is for.
* The core process rules: don't implement a `Proposed` design uninstructed,
  don't mark `Accepted` merely because it's written, update the design
  rather than silently diverging from it if implementation reveals a flaw.

This mirrors ccbstack's own root `CLAUDE.md` sections of the same name
closely (unsurprising — the lcmeg example was itself calibrated against
ccbstack's process) but is written to be dropped into *any* generated
project, independent of ccbstack itself.

### Both `SKILL.md` files updated to point at it

* `skills/init-repo/SKILL.md` step 8 ("Generate a project-specific
  `CLAUDE.md`") gains an explicit instruction to load
  `skills/references/design-docs.md` and adapt its content for the
  generated `CLAUDE.md`'s design-first section, replacing the current
  one-line summary.
* `skills/adopt-repo/SKILL.md` step 7 ("Create or improve `CLAUDE.md`")
  gains the same instruction, used both when writing a design-first section
  from scratch and when checking an existing `CLAUDE.md` for a
  design-first section that's missing or thin (per that skill's existing
  "Improving an existing `CLAUDE.md`" rules on filling gaps without
  overwriting intentional content).

Neither skill invokes the other or needs to know the other exists. Each
`SKILL.md` names the shared file it depends on directly, so the dependency
is explicit and visible in the skill's own instructions rather than implied.

### Repository structure

Add `skills/references/` to ccbstack's own `CLAUDE.md` "Repository
Structure" expected-layout listing, since it is now a real, intentional part
of the repository layout rather than a placeholder.

## Alternatives Considered

* **One skill invokes the other to reuse instructions.** Rejected —
  `CLAUDE.md`'s "Skill Independence" section rules this out directly ("Do
  not require one skill to invoke another merely to reuse instructions"),
  and it would make `adopt-repo` (or `init-repo`) unusable standalone if the
  other skill were ever renamed, removed, or unavailable.
* **Duplicate the guidance into each skill's own `references/` directory**
  (e.g. `skills/init-repo/references/design-docs.md` and
  `skills/adopt-repo/references/design-docs.md`, kept in sync by hand).
  Rejected — this is exactly the kind of duplication `CLAUDE.md` warns
  against ("Prefer one authoritative location for a decision or behavior"),
  and nothing would catch the two copies drifting apart after the first
  edit made to only one of them.
* **Inline the full guidance into ccbstack's own root `CLAUDE.md`, and have
  both skills copy from there.** Rejected — ccbstack's root `CLAUDE.md`
  already contains its *own* design-first process (written for ccbstack
  itself, addressed to Claude working in this repository). Adding a second,
  parallel copy meant for *generated* projects in the same file would be
  confusing to a reader and would blur "instructions for working on
  ccbstack" with "content ccbstack generates for other repositories." A
  dedicated resource file under `skills/references/` keeps those cleanly
  separate while still being one authoritative source.

## Implementation Plan

1. Create `skills/references/design-docs.md` containing the generalized
   design-doc-workflow guidance described under "New shared resource" above,
   distilled from `docs/examples/ai_instructions/lcmeg_hugo_claude-md.md`'s
   "Documentation", "When a Design Document Is Required", "Design Document
   Structure", and "Design and Implementation" sections, generalized to any
   project rather than Hugo/Lake County MEG specifically.
2. Edit `skills/init-repo/SKILL.md` step 8 to reference
   `skills/references/design-docs.md` explicitly and instruct adapting its
   content into the generated `CLAUDE.md`, in place of the current one-line
   design-first mention.
3. Edit `skills/adopt-repo/SKILL.md` step 7 with the equivalent instruction,
   including how it interacts with that skill's existing gap-filling rules
   for an already-present `CLAUDE.md`.
4. Add `skills/references/` to the "Repository Structure" expected-layout
   listing in ccbstack's own root `CLAUDE.md`.
5. Note in `CHANGELOG.md` that both skills now embed fuller design-doc
   guidance in generated `CLAUDE.md` files, per this repository's existing
   changelog convention for user-visible skill behavior changes.

## Implementation Notes

Beyond the plan above, `README.md`'s own "Repository Structure" section
(which documents the same top-level layout as `CLAUDE.md`'s, for a human
audience) was also updated to list `skills/references/` and explain its
purpose, to keep it consistent with `CLAUDE.md`'s updated listing.

## Validation

* Inspect the generated `skills/references/design-docs.md` for accuracy
  against the lcmeg source sections and for the absence of
  Hugo/Lake-County-MEG-specific wording.
* Re-read both edited `SKILL.md` files end-to-end to confirm the new step-8
  / step-7 instructions read consistently with the rest of each skill's
  existing workflow and failure/edge-case sections.
* Run `init-repo` and `adopt-repo` against disposable test directories (per
  `CLAUDE.md`'s "Testing Skills" section) and inspect the resulting
  `CLAUDE.md` files' design-first sections for substance beyond the old
  one-liner, appropriately adapted to the test project rather than copied
  verbatim from the shared file.
* `git status`/`git diff` review before considering the change complete, to
  confirm only the intended files were touched.
