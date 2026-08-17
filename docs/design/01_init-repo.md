# 01 — `init-repo`

Status: Implemented

## Problem

ccbstack needs a repeatable, low-friction way to start a new software repository that already has Git, standard documentation, and a working scaffold in place — without forcing every project into one architecture.

Starting a project today means separately remembering to run `git init`, write a `.gitignore` that actually matches the stack, write a useful `CLAUDE.md` instead of a generic one, pick the right official scaffolding tool, and confirm the result actually builds. Each of these steps is easy to skip or do inconsistently, especially across different languages and frameworks. `init-repo` should do all of it in one consistent workflow, while still respecting that "a .NET repository" isn't one thing — it might be a class library, a console app, a Web API, a worker service, or several related projects (per `CLAUDE.md`'s "Templates and Scaffolding" section). The generated structure must depend on what is actually being built rather than applying a single template to every repository.

## Goals

* One consistent workflow for starting a repository, regardless of language or framework.
* Support the six initial project families: C#/.NET, Go, Hugo, Svelte, WordPress themes, static HTML5.
* Produce a project-specific `CLAUDE.md`, not a copy of ccbstack's own.
* Use each ecosystem's official scaffolding tooling rather than hand-rolled templates.
* Validate the generated project builds/runs before considering the job done.
* Support both "run from a parent directory" and "run inside an already-created directory" invocation styles.

## Non-Goals

* Deciding *which* .NET/Svelte/etc. sub-variant is right for a project — that's the user's call; the skill asks, it doesn't guess.
* Building a standalone CLI or application. This stays a Claude Code skill per `CLAUDE.md`'s "Skills Before Tooling" principle.
* Supporting every possible project family. Six is the initial set; more can be added later as separate `references/` files without changing the shared workflow.
* Publishing ccbstack as an installable plugin. That's a repository-level prerequisite this design calls out (see "Distribution prerequisite" below) but does not implement.

## Current State

ccbstack currently contains only `CLAUDE.md`, `README.md`, `CHANGELOG.md`, `LICENSE`, and `docs/design/` (holding this document). There is no `skills/` directory yet — `init-repo` would be the first skill implemented in the repository, so there is no existing skill to pattern-match against. There is also no `.claude-plugin/plugin.json` manifest at the repository root yet.

`CLAUDE.md`'s "Skill Resources" section already defines the expected internal layout for a skill (`SKILL.md`, `references/`, `templates/`, `scripts/`, `examples/`, created only as needed), and `README.md`'s "Repository Structure" section already documents `skills/init-repo/` and `skills/adopt-repo/` as the intended top-level layout. This design does not need to establish either convention — only apply them to `init-repo`.

## Proposed Design

### Workflow

`init-repo` runs as a linear flow with a few branch points.

1. **Determine location mode.** Check whether the current working directory already looks like the target project root (already named for the project, or already contains files) versus a parent/projects directory such as `Projects/`. Ask the repository name, then confirm (or infer, when unambiguous) whether to create a new subdirectory or initialize in place. This matches the README's example of running from `Projects/` and producing `Projects/ExampleProject/`, as well as running directly inside an already-created empty directory.

2. **Determine project family.** Ask which of the six families applies (or "other/generic"), using `AskUserQuestion`. Do not try to infer the family from the repository name — names are frequently ambiguous ("api", "site", "tools") and a wrong guess is more expensive than one extra question.

3. **Gather family-specific inputs.** Delegate to the relevant `references/<family>.md` file for what to ask next (see "Skill structure" below). Examples: .NET needs a project type (console / Web API / class library / worker service / Razor Pages, or multiple related projects); Go needs a module path; Hugo needs a theme choice; Svelte needs SvelteKit vs. plain Vite+Svelte; WordPress needs a starter theme baseline; static HTML5 needs essentially nothing beyond the name.

4. **Initialize Git.** Run `git init` only if `.git` doesn't already exist. Never re-initialize an existing repository.

5. **Create `.gitignore`.** Use family-appropriate content. If a `.gitignore` already exists, merge missing entries into it rather than overwriting, and report what was added.

6. **Create standard ccbstack folders.** At minimum, create `docs/design/` in the new repository so the design-first workflow (per `CLAUDE.md`) is available there from the start.

7. **Scaffold the project** using the family's official tooling — `dotnet new`, `go mod init` (+ `go build` to prime `go.sum` if dependencies are added), `hugo new site`, `npx sv create` / `npm create vite@latest -- --template svelte`, a WordPress starter theme, or a minimal hand-built structure for static HTML5 (the one family without an official scaffolder). Never hand-roll what an official tool already produces reliably — per `CLAUDE.md`'s "prefer official scaffolding tools" instruction.

8. **Generate a project-specific `CLAUDE.md`.** Use the content checklist from `README.md`'s "Project-Specific CLAUDE.md" section — purpose, language/framework, architecture, important dependencies, build/test commands, development principles, repository conventions, documentation expectations, Git practices, testing expectations, project-specific constraints, and the design-first workflow — and fill in only the sections relevant to this family, using facts gathered in steps 2–3. This must describe the actual project, not restate ccbstack's own `CLAUDE.md`. Examples of recently used `CLAUDE.md` files include the file from this project and other examples in the `docs/examples/ai_instructions` folder.

9. **Generate `README.md` and `CHANGELOG.md`.** Minimal, project-specific starting points: a README describing what the new project is and how to build/run it, and a CHANGELOG seeded with an initial "Unreleased" entry.

10. **Validate.** Run the family's normal build or check command (`dotnet build`, `go build ./...`, `hugo`, `npm run build` or `npm run check`, a theme lint step where available, or a basic structural/HTML-validity check for static HTML5) and surface failures rather than silently continuing or committing a broken scaffold.

11. **Commit.** Stage everything and create the initial commit once validation succeeds. `git init` is skipped when a repository already existed, but the commit step always runs — this establishes a clean starting point in history regardless of whether Git was already present.

### Skill structure

```text
skills/
└── init-repo/
    ├── SKILL.md
    └── references/
        ├── dotnet.md
        ├── go.md
        ├── hugo.md
        ├── svelte.md
        ├── wordpress.md
        └── html5.md
```

* **`SKILL.md`** — YAML frontmatter with `name: init-repo` and a third-person `description` containing concrete trigger phrases (e.g. "start a new repo", "init-repo", "scaffold a new project", "create a new .NET/Go/Hugo/Svelte/WordPress/static site project"). The body holds the shared workflow above, written in imperative form, kept lean (target under ~2,000 words per the skill-authoring conventions used across Claude Code plugins), with a short table pointing to each `references/<family>.md` file and when to load it.
* **`references/<family>.md`** — one file per project family, each covering: which official scaffolding command(s) to use and how to choose among sub-variants; what family-specific questions to ask in step 3; what a minimal `.gitignore` needs; what the generated `CLAUDE.md` should say about that stack's build/test commands and conventions; and how to run the validation step for that family. This keeps `SKILL.md` focused on the shared workflow and isolates technology-specific detail, per `CLAUDE.md`'s instruction to move such guidance into supporting resources rather than duplicating it six times.

### Distribution prerequisite

The user has decided that ccbstack skills will be consumed by other repositories as a **Claude Code plugin**, not by copying or symlinking files per repository. Claude Code's plugin auto-discovery requires a `.claude-plugin/plugin.json` manifest at the repository root, with component directories (`skills/`, etc.) at the plugin root rather than nested inside `.claude-plugin/`. ccbstack does not yet have this manifest. Adding a minimal `.claude-plugin/plugin.json` (just a `name` field, plus recommended metadata such as `description` and `version`) is a **prerequisite for any skill — including `init-repo` — to be installable in another repository**, but it is a repository-level change, not part of this design or its implementation. It should be added once, alongside or just before `init-repo` ships, and is called out here so it isn't missed.

### Inputs / outputs

**Inputs**: repository name; target project family; family-specific answers gathered in step 3 (e.g. module path, project type, theme choice); current working directory state (parent directory vs. already-created target directory; existing Git repo or not).

**Outputs**: an initialized Git repository containing an initial commit with `.gitignore`, `CLAUDE.md`, `README.md`, `CHANGELOG.md`, `docs/design/`, and the scaffolded project files for the chosen family — validated as buildable/runnable using that family's normal tooling before the commit is made.

### Failure & edge cases

* **Target directory already exists and is non-empty**, or is already a different Git repository — stop and report rather than overwriting or merging blindly.
* **Required tooling for the chosen family isn't installed** (no `dotnet`, `go`, `hugo`, `node`, `wp-cli`, etc.) — report clearly and stop rather than partially scaffolding.
* **`.gitignore`, `CLAUDE.md`, `README.md`, or `CHANGELOG.md` already exist** — merge/preserve existing content where sensible (especially `.gitignore`) rather than overwriting, and report what changed.
* **Validation fails** (build/check error) — surface the error and do not commit a broken scaffold.
* **No Git identity configured** (`user.name` / `user.email`) — surface this clearly, since the commit step depends on it.
* **User picks "other/generic" family** — fall back to a minimal scaffold: Git, `docs/design/`, `README.md`, `CHANGELOG.md`, and a minimal `CLAUDE.md`, with no language-specific tooling or `.gitignore` content beyond OS/editor noise.

## Alternatives Considered

* **Infer the project family from the repository name or directory contents**, instead of always asking. Rejected: names are frequently ambiguous ("api", "site", "tools"), and scaffolding the wrong stack is more expensive to undo than asking one extra question.
* **A separate skill per project family** (six skills), instead of one shared `SKILL.md` with six `references/<family>.md` files. Rejected: it would fragment a single conceptual action ("start a new repo") across six overlapping triggers and duplicate the "ask name → ask family → scaffold → validate → commit" flow six times.
* **Hand-written templates for every family**, instead of official scaffolding tools. Rejected for all families except static HTML5 (which has no standard scaffolder and gets a small hand-built minimal structure instead) — hand-rolled templates drift from what the ecosystem's own tooling produces and add maintenance burden ccbstack doesn't need.
* **Copy or symlink skill files into each consuming repository**, instead of distributing ccbstack as a Claude Code plugin. Rejected per explicit user decision; the accepted trade-off is that ccbstack needs the additional `.claude-plugin/plugin.json` manifest described above.
* **Leave the generated repository uncommitted**, matching ccbstack's own general practice of not committing unless asked. Rejected specifically for `init-repo`: its job is to produce a clean starting point for a *new* repository, so committing after successful validation is the intended behavior. This does not change how Claude behaves inside ccbstack itself or other already-established repositories.

## Security and Privacy Considerations

* **Initial commit could capture secrets dropped into the target directory before `init-repo` runs** (e.g. a stray `.env` or local credentials file) if `.gitignore` isn't in place first. The workflow mitigates this by creating/merging `.gitignore` (step 5) before scaffolding and committing (steps 7–11), and each family's `references/<family>.md` must include that ecosystem's standard secret/credential-adjacent ignore entries (e.g. `.env`, `appsettings.*.local.json`).
* **Scaffolding pulls from external package/theme registries** (NuGet, npm, Hugo theme repos, WordPress starter themes) via each ecosystem's official tooling. `init-repo` introduces no custom download or network logic of its own — it only invokes standard, already-trusted tooling — so this is an inherited trust boundary rather than a new one.
* **No credentials are requested or stored by the skill itself.** Family-specific questions (module path, project type, theme choice) are all non-sensitive configuration values.

## Implementation Plan

1. Add `.claude-plugin/plugin.json` to ccbstack's repository root (see "Distribution prerequisite" above). This is a small, one-time repository-level change and can land alongside or just before `init-repo`.
2. Implement `skills/init-repo/SKILL.md` per the "Skill structure" and "Workflow" sections above.
3. Implement the six `skills/init-repo/references/<family>.md` files (`dotnet.md`, `go.md`, `hugo.md`, `svelte.md`, `wordpress.md`, `html5.md`), each covering scaffolding command(s), family-specific questions, `.gitignore` content, `CLAUDE.md` content, and the validation command for that family.
4. Cover the "other/generic" fallback path (no `references/` file needed — it's the minimal scaffold described under "Failure & edge cases").

This design is specific enough that implementation should not need to re-decide the shared workflow, skill layout, or distribution model — only fill in each family's technical detail.

## Validation

**Acceptance criteria:**

* Running `init-repo` from a parent directory creates the named subdirectory with the full standard structure and an initial commit.
* Running `init-repo` inside an already-created empty directory initializes in place instead of nesting another directory.
* The generated `CLAUDE.md` describes the actual project (language, build/test commands, conventions) and is not a copy of ccbstack's own `CLAUDE.md`.
* The scaffolded project builds/validates successfully using that family's normal tooling before the commit is made.
* Re-running against a directory that already has `.git` does not attempt to re-initialize it.
* All six project families (plus "other/generic") can be scaffolded through the same shared workflow, each using its own `references/<family>.md`.

**Testing approach:** Per `CLAUDE.md`'s "Testing Skills" section, test each of the six families against disposable directories rather than modifying ccbstack itself, verifying both the generated structure/documentation and that the scaffold builds or runs using its normal tooling. This is follow-up work to perform once `skills/init-repo/` is implemented — it is out of scope for this design document.
