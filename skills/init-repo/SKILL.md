---
name: init-repo
description: Starts a brand-new software repository — Git init, a project-specific CLAUDE.md, README, CHANGELOG, docs/design/, and a validated initial scaffold for the chosen technology. Use when the user asks to start a new repo, create a new project, init-repo, scaffold a new project, or create a new .NET/Go/Hugo/Svelte/WordPress-theme/static-HTML5 project.
user-invocable: true
---

# init-repo

Starts a new software repository from an empty starting point, following ccbstack's
design-first, inspect-before-changing principles from the repository's own `CLAUDE.md`.

This skill always ends with a validated, committed initial repository — never a
partial scaffold. If any step below fails, stop, report the failure, and do not
commit.

## Reference table

Once the project family is known (step 3), load the matching reference file for
scaffolding commands, family-specific questions, `.gitignore` content, `CLAUDE.md`
guidance, and the validation command. Do not guess this detail — it lives in the
reference file so this skill stays generic across families.

| Family | Reference |
|---|---|
| C#/.NET | `references/dotnet.md` |
| Go | `references/go.md` |
| Hugo | `references/hugo.md` |
| Svelte | `references/svelte.md` |
| WordPress theme | `references/wordpress.md` |
| Static HTML5 | `references/html5.md` |
| Other/generic | none — use the minimal fallback in "Other/generic family" below |

## Workflow

Work through these steps in order. Steps 1–4 gather information; steps 5 onward
change the filesystem and must not start until the family (and its reference file,
if any) is known.

### 1. Determine location mode

Check whether the current working directory already looks like the target project
root (already named for the project, or already contains files) versus a
parent/projects directory such as `Projects/`.

Ask the user for the repository name. If the current directory is empty or looks
like a general projects directory, confirm whether to create a new subdirectory
named for the repository or initialize in place. If the current directory already
appears to be the target project (e.g. its name matches, or it already has some
project files), initialize in place without asking to create a nested directory.

### 2. Determine project intent

Ask the user, in plain conversation (not `AskUserQuestion` — this is open-ended
prose the user composes themselves, not a choice among options), for a short
description of what the project is and who or what it's for. Take their answer
as given, doing no more than light formatting cleanup — this is their authorial
content, not a draft for Claude to improve or expand. If they don't have an
answer yet, write a plain placeholder (e.g. "Purpose not yet defined — update
this section once established") rather than blocking the rest of the workflow
on it.

This matches ccbstack's own root `CLAUDE.md` conceptual sequence for
`init-repo` (location → repository → project intent → technology → ...) —
without this step, the "Project purpose" content every
`references/<family>.md` file asks for later has no defined source.

### 3. Determine project family

Ask which project family applies, using `AskUserQuestion`: C#/.NET, Go, Hugo,
Svelte, WordPress theme, static HTML5, or other/generic.

Do not infer the family from the repository name — names are frequently ambiguous
("api", "site", "tools") and scaffolding the wrong stack is more expensive to
undo than asking one extra question.

### 4. Gather family-specific inputs

Load `references/<family>.md` for the chosen family and follow its "Questions to
ask" section (e.g. .NET project type, Go module path, Hugo theme, SvelteKit vs.
plain Vite+Svelte, WordPress starter theme baseline). Static HTML5 needs nothing
beyond the name. Skip this step entirely for "other/generic" — see "Other/generic
family" below.

### 5. Initialize Git

Run `git init` only if `.git` doesn't already exist in the target directory. Never
re-initialize an existing repository.

If the target directory is non-empty and already a *different* Git repository
(and the user did not ask to adopt it), stop and report rather than proceeding —
this skill is for starting new repositories, not merging into unrelated ones. Use
`adopt-repo` for bringing existing repositories into the ccbstack workflow instead.

### 6. Create `.gitignore`

Write the family-appropriate `.gitignore` content from `references/<family>.md`
(or the minimal OS/editor-noise content for other/generic). If a `.gitignore`
already exists, merge in missing entries rather than overwriting, and report what
was added.

Do this *before* scaffolding or committing — it's what keeps stray secrets or
build output out of the very first commit.

### 7. Create standard ccbstack folders

Create `docs/design/` in the new repository so the design-first workflow is
available there from the start. Git does not track empty directories, so seed
it with a short `docs/design/README.md` (a sentence or two pointing back at
`CLAUDE.md`'s design-first workflow) rather than leaving the directory empty —
otherwise it silently disappears from `git add`/the initial commit and won't
exist for anyone who clones the repository.

### 8. Scaffold the project

Use the family's official scaffolding tool(s), per `references/<family>.md`.
Never hand-roll what an official tool already produces reliably. Static HTML5 is
the one family without an official scaffolder — build its minimal structure by
hand, as described in `references/html5.md`.

### 9. Generate a project-specific `CLAUDE.md`

Write a `CLAUDE.md` that describes the *actual* project — not a copy of
ccbstack's own `CLAUDE.md`. Cover only what's relevant: project purpose, language
and framework, architecture, important dependencies, build/test commands,
development principles, repository conventions, documentation expectations, Git
practices, testing expectations, project-specific constraints, and the
design-first workflow.

Use the project purpose gathered in step 2, verbatim, as the opening
description — don't paraphrase or expand on what the user already told you.

For the design-first workflow section, load `skills/references/design-docs.md`
(shared with `adopt-repo`) and adapt its guidance — when a design document is
required, the Proposed → Accepted → Implemented lifecycle, and the standard
document structure — into the generated `CLAUDE.md`, rather than reducing it to
a one-line mention.

Also load `skills/references/engineering-values.md` (shared with
`adopt-repo`) and adapt its Security, Dependencies, Git, Documentation, and
Technology-Stack guidance — this applies regardless of family. If the
project actually talks to an external/remote system (a network API, cloud
service, SMTP server, subprocess, etc.) or has meaningful runtime
configuration or logging — determined from the stated intent (step 2) and
the family/family-specific answers (steps 3–4), not guessed — also load
`skills/references/external-integrations.md` and adapt its Automation,
Integration Testing, Logging, and Configuration guidance to that project's
specific dependency. Skip it entirely for a project with no such dependency
(e.g. a static HTML5 site with no serverless component, or a
dependency-free library).

Use the facts gathered in steps 2–4, plus `references/<family>.md`'s "CLAUDE.md
guidance" section for what that stack's build/test commands and conventions
normally look like. If real, previously written `CLAUDE.md` examples are
available in this repository (e.g. under `docs/examples/ai_instructions/`, if
present), use them to calibrate tone, depth, and section structure — not to copy
their content, which is specific to those other projects.

### 10. Generate `README.md` and `CHANGELOG.md`

Check whether the family's scaffolding tool already wrote a `README.md` in
step 8 (some do, e.g. a WordPress starter's own usage notes, or a JS
scaffolder's placeholder). If so, rewrite it to be project-specific rather
than blindly overwriting it with a generic template — fold in anything from
the scaffold-provided version worth keeping (e.g. license/support notes),
the same "don't silently clobber what's already there" rule step 6 applies to
`.gitignore`. Otherwise, write a minimal, project-specific `README.md`
describing what the project is and how to build/run it.

Seed `CHANGELOG.md` with an "Unreleased" entry, following the
[Keep a Changelog](https://keepachangelog.com/) format. Scaffolders don't
normally generate one, but apply the same merge-not-overwrite rule if one is
already present.

### 11. Validate

Run the family's normal build or check command from `references/<family>.md`
(e.g. `dotnet build`, `go build ./...`, `hugo`, `npm run build`/`npm run check`).
If it fails, surface the error and stop — do not continue to the commit step with
a broken scaffold.

### 12. Commit

Stage everything and create the initial commit once validation succeeds.

This is an intentional exception to ccbstack's usual "don't commit unless asked"
practice: `init-repo`'s entire job is to produce a clean starting commit for a
*new* repository. It does not change how Claude behaves in ccbstack itself or in
other already-established repositories.

If no Git identity (`user.name`/`user.email`) is configured, report this clearly
before attempting the commit — the commit will fail without it.

## Other/generic family

If the user picks "other/generic" in step 3, skip step 4 (no family-specific
questions) and step 8 (no scaffolding tool). Still run every other step: project
intent, Git init, minimal `.gitignore` (OS/editor noise only — `.DS_Store`,
`Thumbs.db`, editor swap files), `docs/design/`, a minimal `CLAUDE.md`,
`README.md`, `CHANGELOG.md`. There is no build/validate step beyond confirming
the files were written correctly, and no language-specific `.gitignore` content.

## Failure and edge cases

* **Target directory already exists and is non-empty**, or is already a
  different Git repository — stop and report rather than overwriting or merging
  blindly.
* **Required tooling for the chosen family isn't installed** (no `dotnet`, `go`,
  `hugo`, `node`, `wp-cli`, etc.) — report clearly and stop rather than partially
  scaffolding.
* **`.gitignore`, `CLAUDE.md`, `README.md`, or `CHANGELOG.md` already exist** —
  merge/preserve existing content where sensible (especially `.gitignore`)
  rather than overwriting, and report what changed.
* **User has no project intent yet** — use a plain placeholder (step 2) rather
  than blocking the workflow or inventing one.
* **Validation fails** — surface the error and do not commit.
* **No Git identity configured** — surface this clearly before attempting to
  commit.
