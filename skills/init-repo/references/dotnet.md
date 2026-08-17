# C#/.NET — init-repo reference

## Questions to ask

Ask which project type this is — do not guess:

* Console application
* Class library
* Web API
* Worker service
* Razor Pages application
* Multiple related projects (e.g. a library plus a CLI, or an API plus a
  worker) — in a solution

Also ask (or confirm a sensible default and let the user override):

* Target framework — default to the latest supported .NET LTS unless the user
  specifies otherwise.
* The root namespace / project name, if it should differ from the repository
  name.

## Scaffolding

Use the .NET SDK's official templates. Never hand-build a `.csproj`.

| Project type | Command |
|---|---|
| Console application | `dotnet new console -n <Name>` |
| Class library | `dotnet new classlib -n <Name>` |
| Web API | `dotnet new webapi -n <Name>` |
| Worker service | `dotnet new worker -n <Name>` |
| Razor Pages | `dotnet new razor -n <Name>` |

For **multiple related projects**, create a solution and add each project to
it:

```text
dotnet new sln -n <RepoName>
dotnet new classlib -n <RepoName>.Core
dotnet new console -n <RepoName>.Cli
dotnet sln add **/*.csproj
```

Choose the actual project set based on what the user described — the example
above (`.Core` + `.Cli`) is illustrative, not prescriptive.

After scaffolding, edit each `.csproj` to treat warnings as errors where
practical:

```xml
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
</PropertyGroup>
```

Current SDK templates (verified with the .NET 10 SDK) already set
`<Nullable>enable</Nullable>` by default — check the generated `.csproj`
before adding it again rather than assuming it's missing.

## `.gitignore` content

Use the standard Visual Studio / .NET ignore set at minimum:

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

## `CLAUDE.md` guidance

Generate a `CLAUDE.md` in the style of a focused, principles-first .NET
project document (real examples of this style may exist under
`docs/examples/ai_instructions/` in this repository — use them for tone and
structure, not content). Cover, as relevant:

* **Project purpose** — one or two sentences on what the project is and isn't
  responsible for.
* **Design philosophy** — priority order (typically Simplicity, Correctness,
  Maintainability, Performance).
* **Architecture** — project/assembly layout, especially for multi-project
  solutions (dependency direction between projects).
* **C# conventions** — target framework, nullable reference types enabled,
  async/await conventions, cancellation token usage, preference for records
  for immutable models.
* **Build/test commands** — `dotnet build`, `dotnet test` (once tests exist),
  `dotnet run --project <Name>`.
* **Testing expectations** — regression tests for bug fixes, testing observed
  behavior rather than implementation details.
* **Security** — never hardcode secrets/connection strings/API keys; treat
  external input as untrusted.
* **Dependencies** — minimize third-party packages; prefer Microsoft/BCL
  libraries when sufficient.
* **Git practices** — focused commits, no mixing formatting with functional
  changes.
* **Design-first workflow** — design docs for significant changes live in
  `docs/design/`, status Proposed → Accepted → Implemented.

## Validation

Run:

```text
dotnet build
```

Treat any build failure as blocking — do not commit until it succeeds. If the
scaffold includes a solution file, build the solution rather than an
individual project so every project is validated together.
