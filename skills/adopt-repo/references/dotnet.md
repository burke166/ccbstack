# C#/.NET — adopt-repo reference

## Detection signals

* One or more `*.csproj`/`*.fsproj`/`*.vbproj` files, or a `*.sln`/`*.slnx`.
* A `global.json` pinning an SDK version.
* `Directory.Build.props`/`Directory.Build.targets` at the repository root
  (shared MSBuild settings across multiple projects).

Determine the project type(s) actually present rather than assuming one:

* `<OutputType>Exe</OutputType>` with no web framework → console application.
* `Sdk="Microsoft.NET.Sdk.Web"` → Web API or Razor Pages (check for
  `Program.cs` minimal-API patterns vs. `Pages/`/`Views/` folders to tell
  which).
* `Sdk="Microsoft.NET.Sdk.Worker"` → worker service.
* No `<OutputType>` (defaults to library) → class library.
* Multiple `.csproj` files referenced by one `.sln` → a multi-project
  solution; identify each project's role rather than treating the solution as
  one undifferentiated thing.

## `.gitignore` content

If missing, use the standard Visual Studio / .NET set:

```gitignore
bin/
obj/
.vs/
*.user
*.suo
appsettings.*.local.json
*.local.json
.env
```

If a `.gitignore` already exists, only add entries from this list that are
missing — leave everything else as-is.

## `CLAUDE.md` guidance

Document what's actually there, not a generic .NET template:

* **Project purpose** — from the README, solution/project names, and what the
  code actually does.
* **Architecture** — the real project layout: one project, or a solution with
  named responsibilities (e.g. `*.Core`, `*.Cli`, `*.Api`) and their
  dependency direction (read project references, don't guess).
* **Target framework** — read from the `.csproj`/`global.json`, don't assume
  latest LTS.
* **Existing conventions** — nullable reference types enabled or not, whether
  warnings are treated as errors, whether records/DI/async patterns are
  already used consistently — describe what's there, don't prescribe what
  `init-repo` would choose for a fresh project.
* **Build/test commands** — the real commands (`dotnet build`, `dotnet test`,
  or wrapper scripts/Makefile targets if the repository uses one instead).
* **Dependencies** — list notable ones from the `.csproj`(s)/`packages.lock.json`
  if dependency philosophy is worth documenting (e.g. clearly minimal vs.
  clearly framework-heavy).
* **Design-first workflow** — add this section (design docs in `docs/design/`,
  Proposed → Accepted → Implemented) if the repository doesn't already
  describe an equivalent process; don't replace an existing one that differs.

## Verification

If a solution file exists, build the solution; otherwise build the
project(s) found:

```text
dotnet build
```

Only run this if it's a plain local build (no required external services or
credentials). If restoring packages would require private feeds needing
authentication, skip and report that build verification wasn't possible
rather than attempting it.
