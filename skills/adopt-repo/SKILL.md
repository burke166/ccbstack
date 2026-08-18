---
name: adopt-repo
description: Brings an existing software repository into the ccbstack workflow — inspects it first, then creates or improves CLAUDE.md, README.md, CHANGELOG.md, docs/design/, and .gitignore without restructuring the project or overwriting what's already there. Use when the user asks to adopt a repo, onboard an existing project into ccbstack, add a CLAUDE.md to an existing project, or bring an existing .NET/Go/Hugo/Svelte/WordPress-theme/static-HTML5 (or other) repository under the ccbstack workflow.
user-invocable: true
---

# adopt-repo

Brings an existing repository into the ccbstack workflow, following ccbstack's
own `CLAUDE.md`: inspect before changing, preserve intentional existing
decisions, and never force an existing project into `init-repo`'s scaffold
choices merely because that's what a new project would have gotten.

Unlike `init-repo`, this skill mostly *reads* before it *writes* — there's a
real repository to learn from instead of a blank slate to ask about. It never
commits on its own; report what changed and leave staging/committing to the
user.

## Reference table

Once the project family is identified (step 4), load the matching reference
file for detection signals, `.gitignore` content, `CLAUDE.md` guidance, and a
safe verification command. These are **adoption**-focused references, not
`init-repo`'s scaffolding-focused ones of the same name — do not substitute
one for the other.

| Family | Reference |
|---|---|
| C#/.NET | `references/dotnet.md` |
| Go | `references/go.md` |
| Hugo | `references/hugo.md` |
| Svelte | `references/svelte.md` |
| WordPress theme | `references/wordpress.md` |
| Static HTML5 | `references/html5.md` |
| Other/generic | none — use the minimal fallback in "Other/generic family" below |

A repository may match more than one family — see "Multi-language
repositories" below.

## Workflow

### 1. Confirm the target repository

Default to the current working directory. If it doesn't look like a
repository root (no recognizable project files) or the user names a different
path, ask for the correct location rather than guessing.

### 2. Initialize Git only if genuinely absent

If `.git` doesn't exist, run `git init` so the repository can participate in
the ccbstack workflow at all. If it already exists, never touch Git history —
no resetting, rebasing, or branch changes. This is the one `init-repo`-like
action in this workflow, needed only because `.gitignore` and the final report
(via `git status`) assume Git is present.

### 3. Inspect the repository before changing anything

Read the existing file tree, package/build manifests, existing documentation
(`README.md`, `CHANGELOG.md`, `CLAUDE.md`, `.gitignore`, `docs/`), test setup,
and CI configuration if present. Build a picture of:

* what the project is for;
* what language(s)/framework(s) it uses;
* how it's actually built/tested/formatted — from real scripts, Makefiles, CI
  config, or README instructions, not assumed defaults;
* what conventions already exist (structure, naming, documentation style).

Do not write anything in this step. This is the step that makes the rest of
the workflow safe — everything downstream depends on what's actually found
here, not on assumptions.

### 4. Match against a project family, and confirm rather than guess

Compare what step 3 found against the six families in the reference table,
using each `references/<family>.md`'s "Detection signals" section, plus
"other/generic" if nothing matches. Propose the detected family (or families —
see "Multi-language repositories") to the user via `AskUserQuestion` for
confirmation. Unlike `init-repo`, which has no evidence and must ask cold,
`adopt-repo` has real signal to present — confirm it rather than silently
acting on a guess, and correct the detection if the user says it's wrong.

### 5. Create or improve `.gitignore`

If missing, create one from the confirmed family's typical entries (per
`references/<family>.md`). If present, merge in missing stack-appropriate
entries without touching existing custom entries, and report exactly what was
added. Never overwrite an existing `.gitignore`.

### 6. Create `docs/design/` if missing

Seed it with a short `docs/design/README.md` pointing back at `CLAUDE.md`'s
design-first workflow. Git does not track empty directories — creating an
empty `docs/design/` would silently vanish the moment anything is committed,
so always seed it with a real file. If `docs/design/` already exists (with or
without documents in it), leave it untouched.

### 7. Create or improve `CLAUDE.md`

If none exists, generate one using the same content checklist `init-repo`
uses — purpose, language/framework, architecture, important dependencies,
build/test commands, development principles, repository conventions,
documentation expectations, Git practices, testing expectations,
project-specific constraints, and the design-first workflow — grounded in
what step 3 actually found, not fresh choices. Use `references/<family>.md`'s
"CLAUDE.md guidance" section for what that stack's document normally covers.

For the design-first workflow section, load `skills/references/design-docs.md`
(shared with `init-repo`) and adapt its guidance — when a design document is
required, the Proposed → Accepted → Implemented lifecycle, and the standard
document structure — into the generated `CLAUDE.md`, rather than reducing it
to a one-line mention.

Also load `skills/references/engineering-values.md` (shared with
`init-repo`) and adapt its Security, Dependencies, Git, Documentation, and
Technology-Stack guidance — this applies regardless of family. If step 3's
inspection shows the project actually talks to an external/remote system (a
network API, cloud service, SMTP server, subprocess, etc.) or has
meaningful runtime configuration or logging, also load
`skills/references/external-integrations.md` and adapt its Automation,
Integration Testing, Logging, and Configuration guidance to what was
actually found — not guessed. Skip it entirely for a project with no such
dependency, the way it would be absent from a dependency-free utility
library.

If a `CLAUDE.md` already exists, *improve* it rather than replace it — see
"Improving an existing CLAUDE.md" below. Treat a thin one-line mention, or a
missing section, as a gap to fill using the appropriate shared reference
above, following the same "fill in gaps, don't reorganize intentional
content" rule as any other section.

### 8. Create missing `README.md`/`CHANGELOG.md`

Only if genuinely absent. Write a minimal, project-specific version the same
way `init-repo` does (a README describing what the project is and how to
build/run it; a CHANGELOG seeded with an "Unreleased" entry in [Keep a
Changelog](https://keepachangelog.com/) format). Never rewrite an existing
`README.md` or `CHANGELOG.md` uninvited — at most, note in the final report if
either looks significantly out of date relative to what step 3 found.

### 9. Verify where it's quick and safe

Run the build/test command recorded in the (new or updated) `CLAUDE.md`,
using `references/<family>.md`'s verification command, if doing so is fast
and has no destructive or networked side effects beyond a normal local build
(e.g. `dotnet build`, `go build ./...`, `npm run build`). If verification
isn't safe or practical to run automatically — it needs infrastructure,
credentials, or a long-running process — say so explicitly in the report
instead of silently skipping it.

### 10. Report

Summarize, file by file: what was created, what was changed and why, and what
was found but intentionally left alone. **Do not stage or commit anything** —
that decision belongs to the user, since this repository already has history
(and possibly uncommitted work) to protect. This is a deliberate difference
from `init-repo`, which always commits its own fresh scaffold.

## Improving an existing `CLAUDE.md`

This step has no `init-repo` equivalent and is the one most likely to go
wrong, so follow these rules explicitly:

* Treat a section that already states a clear decision as intentional — leave
  it, even if `init-repo` would have chosen differently for a new project.
* Treat a section that is entirely absent, where inspection found an
  unambiguous fact (e.g. no "Build" section, but a `Makefile` with an obvious
  `build` target), as a gap — fill it in.
* Treat a section that appears to contradict what inspection found (e.g.
  `CLAUDE.md` says "no external dependencies" but a lockfile lists a dozen) as
  a discrepancy — do not silently resolve it in either direction. Flag it in
  the report and ask which is authoritative if it's unclear.
* Never delete existing content to make room for the generated structure.
  Add and correct; don't reorganize prose choices the user already made.

## Multi-language repositories

An adopted repository may genuinely span more than one family (e.g. a Go
backend with a Svelte frontend) — unlike a freshly scaffolded `init-repo`
project, which is always exactly one. Step 4 should surface every family it
detects, not force a single choice. The resulting `CLAUDE.md` should document
each detected part of the repository (its own build/test commands,
conventions) rather than assuming one language describes the whole project.
Do not decide how such a repository *should* be organized — only document
what's actually there.

## Other/generic family

If nothing in step 4 matches a known family (or the user says none apply),
skip family-specific `.gitignore`/verification guidance. Still run every
other step: Git init if absent, a minimal `.gitignore` merge (OS/editor noise
only — `.DS_Store`, `Thumbs.db`, editor swap files — plus anything obviously
implied by what step 3 found), `docs/design/`, a `CLAUDE.md` built from
whatever step 3 actually discovered, and missing `README.md`/`CHANGELOG.md`.
There is no family-specific verification command; note in the report that
build/test verification wasn't performed and why.

## Failure and edge cases

* **Target doesn't look like a repository at all** (empty directory, or
  clearly not a software project) — stop and ask, rather than generating
  speculative documentation for nothing.
* **No family matches and the repository is ambiguous** — fall back to
  "other/generic" rather than forcing a wrong family; document only what
  inspection actually established.
* **Existing `CLAUDE.md` contradicts what inspection found** — flag it per
  "Improving an existing `CLAUDE.md`" above; never silently overwrite.
* **Verification would require credentials, infrastructure, or is
  long-running** — skip it and say so explicitly in the report; do not
  attempt it anyway.
* **Repository is genuinely multi-language** — handle per "Multi-language
  repositories" above; do not force a single family.
* **`.git` already exists but looks broken or mid-operation** (e.g. an
  in-progress rebase or merge) — stop and report rather than running `git`
  commands against it.
