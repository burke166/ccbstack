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
* Test framework — default to xUnit (the .NET SDK's default test template)
  unless the user specifies otherwise (e.g. NUnit, MSTest).

## Standard layout

Every .NET repository created by this skill uses this layout, regardless of
project type or project count:

```text
<RepoName>.slnx
src/
  <ProjectName>/
    <ProjectName>.csproj
tests/
  <ProjectName>.Tests/
    <ProjectName>.Tests.csproj
```

* Application/library code always lives under `src/<ProjectName>/`, never at
  the repository root.
* Every `src` project gets a corresponding `tests/<ProjectName>.Tests/`
  project, referencing it via `<ProjectReference>`.
* The repository root holds a single `<RepoName>.slnx` (the XML solution
  format, not the legacy `.sln` format) referencing every `src` and `tests`
  project.
* For multiple related `src` projects (e.g. a library plus a CLI), add one
  `tests/<ProjectName>.Tests/` per project that has independently testable
  behavior — don't force unrelated projects to share one test project.

## Scaffolding

Use the .NET SDK's official templates. Never hand-build a `.csproj`.

| Project type | Command |
|---|---|
| Console application | `dotnet new console -n <ProjectName> -o src/<ProjectName>` |
| Class library | `dotnet new classlib -n <ProjectName> -o src/<ProjectName>` |
| Web API | `dotnet new webapi -n <ProjectName> -o src/<ProjectName>` |
| Worker service | `dotnet new worker -n <ProjectName> -o src/<ProjectName>` |
| Razor Pages | `dotnet new razor -n <ProjectName> -o src/<ProjectName>` |

Scaffold the test project the same way for every project type:

```text
dotnet new xunit -n <ProjectName>.Tests -o tests/<ProjectName>.Tests
```

(Substitute the SDK template for the chosen test framework if the user
requested something other than xUnit, e.g. `dotnet new nunit` or
`dotnet new mstest`.)

Add the project reference from the test project to the project it tests:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\<ProjectName>\<ProjectName>.csproj" />
</ItemGroup>
```

Then create the root solution and add every project to it:

```text
dotnet new sln -n <RepoName> -o . --format slnx
dotnet sln <RepoName>.slnx add src/<ProjectName>/<ProjectName>.csproj tests/<ProjectName>.Tests/<ProjectName>.Tests.csproj
```

For **multiple related projects**, repeat the `src`/`tests` scaffolding per
project and add every `.csproj` to the same root `.slnx`:

```text
dotnet new classlib -n <RepoName>.Core -o src/<RepoName>.Core
dotnet new console -n <RepoName>.Cli -o src/<RepoName>.Cli
dotnet new xunit -n <RepoName>.Core.Tests -o tests/<RepoName>.Core.Tests
dotnet new sln -n <RepoName> -o . --format slnx
dotnet sln <RepoName>.slnx add src/<RepoName>.Core/<RepoName>.Core.csproj src/<RepoName>.Cli/<RepoName>.Cli.csproj tests/<RepoName>.Core.Tests/<RepoName>.Core.Tests.csproj
```

Choose the actual project set based on what the user described — the example
above (`.Core` + `.Cli`) is illustrative, not prescriptive.

After scaffolding, edit each `.csproj` (both `src` and `tests` projects) to
treat warnings as errors where practical:

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

These patterns are unanchored, so they already match `bin/`/`obj/` under both
`src/<ProjectName>/` and `tests/<ProjectName>.Tests/` without modification.

## `CLAUDE.md` guidance

Generate a `CLAUDE.md` in the style of a focused, principles-first .NET
project document (real examples of this style may exist under
`docs/examples/ai_instructions/` in this repository — use them for tone and
structure, not content). Cover, as relevant:

* **Project purpose** — one or two sentences on what the project is and isn't
  responsible for.
* **Design philosophy** — priority order (typically Simplicity, Correctness,
  Maintainability, Performance).
* **Architecture** — state the `<RepoName>.slnx` / `src/` / `tests/` layout
  explicitly, and the project/assembly layout within `src/` (dependency
  direction between projects) for multi-project solutions.
* **C# conventions** — target framework, nullable reference types enabled,
  async/await conventions, cancellation token usage, preference for records
  for immutable models.
* **Build/test commands** — `dotnet build <RepoName>.slnx`,
  `dotnet test <RepoName>.slnx`, `dotnet run --project src/<ProjectName>`.
* **Testing expectations** — regression tests for bug fixes, testing observed
  behavior rather than implementation details.
* **Dependencies** — prefer Microsoft/BCL libraries when sufficient (see
  `skills/references/engineering-values.md` for the general
  before-adding-a-package checklist).
* **Design-first workflow** — see `skills/references/design-docs.md`.

Security, Git, and code-documentation guidance come from
`skills/references/engineering-values.md`, loaded and adapted regardless of
family — no need to restate them here.

## Validation

Run:

```text
dotnet build <RepoName>.slnx
dotnet test <RepoName>.slnx
```

Treat any build or test failure as blocking — do not commit until both
succeed. Always build and test the root `.slnx`, not an individual project,
so every project (including the test project) is validated together.
