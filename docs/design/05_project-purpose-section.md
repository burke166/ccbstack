# 05 — Project purpose section for generated `CLAUDE.md` files

Status: Implemented

## Problem

Every `references/<family>.md` file in both `init-repo` and `adopt-repo`
lists "Project purpose" as the first thing a generated `CLAUDE.md` should
cover (e.g. `init-repo/references/dotnet.md`: *"one or two sentences on
what the project is and isn't responsible for"*). Neither skill's workflow
has a step that actually gathers that input, though.

`init-repo`'s questions (steps 1–3) cover location, project family, and
family-specific technical choices (target framework, module path, theme,
etc.) — nothing about what the project is *for*. For a brand-new repository
with no code yet, there is no other source of truth: without an explicit
question, that bullet gets filled from a guess based on the repository
name and chosen family, which conflicts with ccbstack's own "do not invent
facts"/"inspect before assuming" principles — there's nothing to inspect
yet for a project's intent.

Notably, ccbstack's own root `CLAUDE.md` already anticipated this. Its
`init-repo` section states the conceptual sequence as:

```text
location
    ↓
repository
    ↓
project intent
    ↓
technology
    ↓
application type and architecture
    ↓
scaffold
    ↓
verification
```

"project intent" sits explicitly between "repository" and "technology" —
but `init-repo/SKILL.md`'s actual step sequence jumps from location (step
1) straight to project family (step 2), skipping the stage the conceptual
model already called for.

`adopt-repo` is in better shape, since it derives purpose from an existing
README, package manifests, and code — but has no explicit step for it, and
no fallback when inspection comes up thin (no README, cryptic naming, no
doc comments). More importantly, a purpose paragraph is qualitatively
different from `adopt-repo`'s other generated content: build commands,
`.gitignore` entries, and detected dependencies are objectively checkable
against the repository, but a paragraph characterizing what a project *is
for* is `adopt-repo`'s own interpretation of what it read, and easy for a
human to skim past in a `git diff` review without noticing it's subtly
wrong — the read-only inspection step could reasonably infer the wrong
thing (an audience, a purpose, a scope boundary) with no error message
alerting anyone.

## Goals

* Give both skills an explicit, defined source for the "Project purpose"
  content their family reference files already ask for.
* For `init-repo`, get this directly from the user — there's no existing
  artifact to derive it from, and the user is the only source of truth for
  a project's intent before it exists.
* For `adopt-repo`, draft the purpose section from inspection, then let the
  user review and correct it through an iterative draft/revise loop before
  it's finalized — rather than asking the user to write the whole thing
  themselves, or silently trusting a possibly-wrong inference.
* Match the user's stated collaboration preference: interactive,
  checkpointed review of interpretively risky content, not
  generate-then-only-review-the-final-diff.

## Non-Goals

* Extending the same interactive draft/review loop to every other
  `CLAUDE.md` section. The rest of `adopt-repo`'s output stays
  generate-once, reviewed by the user afterward via `git status`/`git
  diff`, per its existing "never commits on its own" model — that model
  works fine for objectively-checkable content (build commands,
  `.gitignore` entries). This design only adds interactivity where
  inspection is inherently interpretive.
* Changing how `init-repo` handles any other question — it already asks
  the user directly for everything else; this adds one more question of
  the same kind.
* A new shared `skills/references/` file. The actual purpose text is
  entirely project-specific prose with nothing to template, and the
  guidance on what makes a good purpose paragraph is a sentence or two —
  not enough to justify a third shared resource file (see "Alternatives
  Considered").

## Current State

* `init-repo/SKILL.md` steps 1–3: location, family, family-specific
  technical questions. No step asks what the project is or who it's for.
* `adopt-repo/SKILL.md` step 3 ("Inspect the repository before changing
  anything") already builds "what the project is for" as part of its
  picture-building, but that's folded into general inspection, not
  surfaced to the user for confirmation before it's written into
  `CLAUDE.md` in step 7.
* Every `references/<family>.md` file (both skills, all six families)
  already lists "Project purpose" first in its "`CLAUDE.md` guidance"
  section — this design doesn't add a new section to generate, it defines
  where that section's content actually comes from.
* The real examples in `docs/examples/ai_instructions/` show a consistent
  shape worth noting (not mandating): state what the project is, its
  primary responsibilities, and often what it deliberately does *not* own
  (Karl: *"Karl does not own: User management, Scheduling..."*; MMVM has a
  dedicated Non-Goals section). This is useful framing for whoever drafts
  the paragraph — the user directly (`init-repo`) or Claude from inspection
  (`adopt-repo`) — not new boilerplate to generate.

## Proposed Design

### `init-repo`: ask directly, once

Insert a new step between the existing step 1 (location) and step 2
(family), matching the root `CLAUDE.md` conceptual sequence exactly:

**New step 2 — Determine project intent.** Ask the user, in plain
conversation (not `AskUserQuestion` — this is open-ended prose the user
composes themselves, not a choice among options), for a short description
of what the project is and who or what it's for. Take their answer as
given, doing no more than light formatting cleanup — this is their
authorial content, not a draft to improve. If they don't have an answer
yet, write a plain placeholder (e.g. "Purpose not yet defined — update this
section once established") rather than blocking the rest of the workflow
on it.

Renumber the existing steps 2–11 to 3–12 accordingly, and update every
downstream reference to step numbers (e.g. "the facts gathered in steps
2–3" in the current `CLAUDE.md`-generation step becomes "steps 2–4").

### `adopt-repo`: draft from inspection, then review/revise until confirmed

Insert a new step after the existing step 3 (Inspect) and before step 4
(Match family) — purpose doesn't depend on which family is detected, and
settling it early means the user isn't asked to react to it a second time
once the rest of `CLAUDE.md` is drafted in what is currently step 7.

**New step 4 — Draft and confirm the project's purpose.**

1. Using what step 3's inspection found (README, package manifests, doc
   comments, code structure, existing `CLAUDE.md` if present), draft a
   short description of what the repository is, who or what it's for, and
   — if it's evident — what it deliberately doesn't do.
2. Present the draft to the user in plain conversation and ask whether
   it's accurate.
3. If the user proposes a change, revise the draft to incorporate their
   feedback and present it again — don't ask them to rewrite the whole
   thing; they should only ever need to describe what to change, not
   restate what's already right.
4. Repeat step 2–3 until the user affirmatively confirms the draft is
   accurate. There's no fixed iteration limit — continue until the user
   says so.
5. Use the confirmed text, unedited, as the purpose section when the rest
   of `CLAUDE.md` is generated in what is now step 8.

**Fallback when inspection finds nothing to go on:** if there's no README,
no package-manifest description, and nothing in the code that makes intent
evident, say so plainly and ask the user directly, the same way `init-repo`
does — presenting a fabricated guess as a "draft to react to" would violate
"do not invent facts" just as much as silently writing one into `CLAUDE.md`
would.

**Interaction with an existing `CLAUDE.md`:** apply the same rule this
skill already uses elsewhere (see "Improving an existing `CLAUDE.md`" in
`adopt-repo/SKILL.md`) — a purpose section that's already present and
clearly intentional is left alone, even if this step would have drafted it
differently. A missing purpose section, or one thin enough to be a gap
(e.g. just the repository name restated), gets the draft/review/revise
treatment above.

Renumber the existing steps 4–10 to 5–11 accordingly.

## Alternatives Considered

* **A shared `skills/references/project-purpose.md`.** Rejected — unlike
  `design-docs.md` or `engineering-values.md`, there's no reusable
  boilerplate text here: the purpose paragraph is entirely project-specific
  prose, and the one piece of transferable guidance (state what it is, who
  it's for, optionally what it doesn't do) is a sentence, not a file's worth
  of content. Putting it inline in each `SKILL.md` step is simpler and
  avoids a third shared file for something this small.
* **Use `AskUserQuestion` for the `init-repo` question or the `adopt-repo`
  review loop.** Rejected — that tool is designed for 2–4 discrete,
  mutually exclusive choices with an "Other" escape hatch, not open-ended
  prose composition or iterative free-text critique. Plain conversation is
  the right tool for both.
* **Apply the same draft/review loop to every generated `CLAUDE.md`
  section in `adopt-repo`, not just purpose.** Rejected as disproportionate
  — most other sections (build commands, `.gitignore` entries, detected
  dependencies) are objectively checkable against the repository and
  already get a normal `git diff` review; the interactive loop is
  justified specifically because a purpose paragraph is `adopt-repo`'s own
  interpretation and easy to skim past unnoticed if wrong, not because
  interactivity is free.
* **Only ask `adopt-repo` to draft and present once, without a revision
  loop, relying on `git diff` review like everything else.** Rejected per
  the user's explicit request and stated collaboration preference: they
  don't want to have to rewrite the section from scratch to fix it, only
  to describe what's wrong, and want the chance to do that iteratively
  rather than accept-or-redo-everything.

## Implementation Plan

1. `init-repo/SKILL.md`: insert the new "Determine project intent" step
   between current steps 1 and 2; renumber subsequent steps; update the
   "steps 2–3" reference inside the `CLAUDE.md`-generation step (and any
   other step-number cross-references) to match the new numbering.
2. `adopt-repo/SKILL.md`: insert the new "Draft and confirm the project's
   purpose" step between current steps 3 and 4; renumber subsequent steps;
   update the "Improving an existing `CLAUDE.md`" section to note that
   purpose-section gaps are filled via this new step's loop rather than
   silently; update any step-number cross-references.
3. No changes needed to the six `references/<family>.md` "Project purpose"
   bullets in either skill — they already describe the target content;
   this only defines where it comes from.
4. Update `CHANGELOG.md`.

## Validation

* Re-read both edited `SKILL.md` files end-to-end for consistency and
  correct step-number cross-references, the same check performed for `03`
  and `04`.
* Manually walk `init-repo` against a disposable test repository and
  confirm the new question is asked at the right point and its answer
  (verbatim, not paraphrased) ends up in the generated `CLAUDE.md`.
* Manually walk `adopt-repo` against a disposable test repository twice:
  once with enough evidence to draft from (a README with real content),
  confirming the draft/review loop runs and a deliberately "wrong" first
  draft gets corrected without requiring a full rewrite from the user; and
  once with no real evidence (no README, opaque code), confirming the
  fallback asks directly instead of presenting a fabricated guess.
* Confirm an existing, clearly intentional purpose section is left
  untouched when re-running `adopt-repo` against a repository that already
  has one.
* `git status`/`git diff` review before considering the change complete.
