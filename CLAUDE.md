# ccbstack Development Instructions

## Project purpose

ccbstack is a Windows-first, AI-assisted software development framework built primarily for Claude Code.

It consists of:

1. A .NET command-line tool invoked as:

   ```powershell
   dotnet ccbstack <command>
   ```

2. A collection of reusable AI development skills, including specification, planning, review, testing, investigation, QA, safety, release, and learning workflows.

3. Deterministic repository tooling that supplies facts, performs checks, enforces safety policies, and produces structured results for AI agents and human developers.

ccbstack must support development of:

* .NET command-line applications
* ASP.NET Core web applications
* Windows GUI applications
* Go command-line applications
* PowerShell tooling used by those projects

The first external project expected to use ccbstack is MegaMek Version Manager, a .NET Windows application.

## Current development stage

The repository is in its initial bootstrap stage.

Do not attempt to implement the entire planned ccbstack feature set in one pass. Work in small, testable vertical slices.

The initial command roadmap includes:

```text
dotnet ccbstack version
dotnet ccbstack repo inspect
dotnet ccbstack doctor
dotnet ccbstack skills validate
dotnet ccbstack skills render
dotnet ccbstack safety check
dotnet ccbstack freeze
dotnet ccbstack unfreeze
dotnet ccbstack check
dotnet ccbstack affected
dotnet ccbstack config
dotnet ccbstack docs check
dotnet ccbstack learn
dotnet ccbstack release
```

The planned skills, in approximate priority order, are:

```text
/careful
/freeze
/guard
/spec
/plan-eng-review
/review
/investigate
/dotnet-review
/dotnet-test
/powershell-review
/qa
/qa-only
/ship
/go-review
/go-test
/database-review
/api-review
/cli-review
/document-release
/learn
```

Additional strategic, security, browser, and deployment skills may be added later.

## Architecture

The initial production projects are:

```text
src/
├── CcbStack.Cli/
└── CcbStack.Core/
```

The initial test projects are:

```text
tests/
├── CcbStack.Core.Tests/
└── CcbStack.Cli.Tests/
```

### CcbStack.Cli

`CcbStack.Cli` owns the executable boundary:

* Spectre.Console.Cli command definitions
* command settings and argument parsing
* dependency registration
* terminal presentation
* JSON presentation
* exit-code mapping
* top-level exception handling

Keep this project thin.

Command classes should coordinate work. They should not contain substantial business logic, repository inspection logic, safety rules, rendering logic, or configuration logic.

### CcbStack.Core

`CcbStack.Core` owns reusable application behavior.

Organize it using feature folders as capabilities are implemented. Likely feature areas include:

```text
Checks/
Configuration/
Diagnostics/
Execution/
Learning/
Releases/
Repositories/
Safety/
Skills/
```

Do not create a separate project or assembly for each feature area unless a concrete need emerges.

Valid reasons for a future project split include:

* a real platform boundary
* an independently consumed library
* incompatible dependency requirements
* a plugin or process boundary
* demonstrated coupling or build problems

Speculative future reuse is not sufficient reason to create another assembly.

## Design principles

### Build vertical slices

Implement one complete, usable behavior at a time.

A vertical slice normally includes:

* command registration
* settings and input validation
* core behavior
* human-readable output
* structured output where applicable
* exit-code behavior
* unit tests
* CLI-level tests
* documentation updates

Avoid creating large unused abstraction layers in anticipation of later commands.

### Deterministic tooling over prompt logic

Anything that can be implemented deterministically should normally live in the .NET tool rather than being repeated as prose in an AI skill.

Examples include:

* repository discovery
* Git status inspection
* project detection
* configuration loading
* safety-rule matching
* skill validation
* template rendering
* affected-file calculation
* exit-code selection

AI skills should use deterministic results and apply judgment where judgment is actually required.

### Keep the CLI thin

Commands should generally:

1. Accept and validate command-line input.
2. Call a core service or use case.
3. Render the returned result.
4. Return a documented exit code.

Avoid placing filesystem traversal, process execution, Git parsing, or policy decisions directly in command classes.

### Prefer concrete code over premature abstraction

Do not introduce an interface solely because a class exists.

Interfaces are appropriate when:

* multiple implementations are required
* a platform or external-service boundary exists
* the behavior must be substituted in tests
* dependency inversion provides a clear benefit

Prefer small, cohesive classes and records over generic frameworks.

### Make results explicit

Core operations should return explicit result models rather than writing directly to the console.

A result should make it possible for the CLI to produce both:

* readable terminal output
* stable machine-readable output

Do not require an AI agent to parse decorative terminal text when structured data can be returned.

### Preserve machine-readable operation

Commands that produce meaningful data should be designed to support structured output, normally JSON.

Human-readable and JSON output must be generated from the same underlying result model.

Do not embed Spectre rendering objects or ANSI formatting in core result types.

### Be Windows-first without being needlessly Windows-only

The primary development environment is:

* Windows
* PowerShell 7
* the .NET CLI
* Claude Code
* VS Code or Visual Studio

Use Windows-safe path handling and test paths containing spaces.

Use:

* `Path.Combine`
* `Path.GetFullPath`
* `Path.GetRelativePath`
* `Environment.GetFolderPath`
* `FileInfo`
* `DirectoryInfo`

Do not build paths using manual slash concatenation.

Where practical, avoid unnecessary operating-system assumptions so that .NET and Go projects targeting Linux remain supported.

### Use PowerShell 7 for repository scripts

Repository automation intended for Windows should be written in PowerShell 7.

Use:

```powershell
#Requires -Version 7.0
```

Prefer:

* `$PSScriptRoot`
* `Join-Path`
* `-LiteralPath`
* explicit parameter declarations
* `$LASTEXITCODE` checks for native commands
* terminating errors for failed required operations

Do not require Git Bash for normal ccbstack development or installation.

A Bash script may be added later when it serves a real non-Windows use case. Do not create Bash translations automatically.

## Coding conventions

Follow the repository's existing SDK, target framework, nullable settings, analyzers, and formatting rules.

Unless the existing code establishes a different convention:

* enable nullable reference types
* use file-scoped namespaces
* use implicit global usings where appropriate
* prefer records for immutable result models
* use `sealed` for classes not designed for inheritance
* use asynchronous APIs for actual asynchronous I/O
* accept `CancellationToken` for operations that may block or perform I/O
* avoid unnecessary `Async` suffixes on methods that are not asynchronous
* avoid blocking on tasks with `.Result` or `.Wait()`
* dispose owned resources correctly
* do not catch `Exception` unless adding meaningful context or handling it at an application boundary

Do not add XML documentation to every private or obvious member. Document public APIs and behavior where documentation adds useful information.

## Spectre.Console.Cli conventions

Use Spectre.Console.Cli for the command hierarchy and parsing.

Commands should be organized so the intended invocation remains:

```powershell
dotnet ccbstack <command>
```

Nested commands should follow the planned hierarchy, for example:

```powershell
dotnet ccbstack repo inspect
dotnet ccbstack skills validate
dotnet ccbstack safety check
```

Command settings should:

* use clear names
* include useful descriptions
* validate incompatible or missing values
* avoid aliases that create ambiguity
* preserve non-interactive operation

Do not couple core services to Spectre.Console or Spectre.Console.Cli types.

Use Spectre.Console for terminal presentation only at the CLI boundary.

## Output conventions

Terminal output should be readable but restrained.

Do not use large banners, excessive panels, or decorative formatting for routine commands.

Errors should identify:

* what failed
* why it failed, when known
* what the user can do next

Do not expose stack traces during normal operation unless verbose or diagnostic output is explicitly requested.

JSON output should:

* be valid JSON
* contain no ANSI formatting
* remain stable enough for scripts and AI agents
* distinguish successful execution from findings or failures
* include diagnostics as structured objects

## Exit codes

Use consistent, documented exit codes.

The initial convention is:

```text
0  Success
1  Check completed but reported a failure or blocking finding
2  Invalid command-line input
3  Invalid configuration
4  Unsupported or unrecognized repository
5  Required dependency unavailable
6  Operation blocked by a safety policy
7  Unexpected internal error
```

Do not call `Environment.Exit` from core code.

Return exit codes through the CLI framework and keep exit-code mapping centralized where practical.

## Testing

Use automated tests for all deterministic behavior.

The preferred test stack is:

* xUnit
* FluentAssertions
* Moq only where a mock is genuinely useful

Prefer simple fakes, temporary directories, and real value objects over extensive mocking.

### Core tests

Core tests should directly exercise:

* result models
* validators
* configuration behavior
* path handling
* repository inspection
* safety rules
* rendering behavior
* affected-file calculations
* exit-status decisions

### CLI tests

CLI tests should verify:

* command registration
* argument parsing
* settings validation
* command selection
* exit codes
* human-readable output
* JSON output
* top-level error handling

Do not make tests depend on the developer's global ccbstack installation.

Use temporary directories for filesystem tests. Clean them up reliably.

Include Windows path cases, especially:

* paths containing spaces
* mixed path separators when relevant
* relative paths
* paths outside the repository
* missing files and directories

### Test behavior, not implementation details

Tests should verify observable behavior and meaningful contracts.

Avoid tests that merely duplicate the implementation or make harmless refactoring unnecessarily difficult.

## Dependency management

Before adding a package:

1. Determine whether the .NET base class library already provides the required behavior.
2. Confirm that the package solves a current requirement.
3. Keep the dependency in the project that actually uses it.
4. Avoid adding broad frameworks for a small feature.

`CcbStack.Core` should not reference Spectre.Console.Cli.

Do not add logging, dependency injection, serialization, Git, templating, or validation packages until the current slice needs them.

## Process execution

All external process execution must eventually flow through a controlled core abstraction.

The process layer should support:

* executable and argument separation
* correct quoting
* working-directory selection
* stdout capture
* stderr capture
* exit-code capture
* cancellation
* timeouts where appropriate
* no shell invocation unless shell behavior is explicitly required

Do not build a complete process framework before a command requires external execution.

Never place untrusted text directly into a shell command string.

## Safety

ccbstack will eventually implement `/careful`, `/freeze`, and `/guard` and a deterministic safety engine.

Until that system is implemented:

* inspect paths before destructive filesystem operations
* avoid broad recursive deletion
* do not discard uncommitted work
* do not use `git reset --hard`
* do not use broad `git restore` or `git checkout` operations
* do not force-push
* do not publish packages
* do not create or push release tags
* do not modify user-level Claude Code installation directories unless the task explicitly requires it
* do not overwrite existing configuration without preserving or explicitly replacing it

When cleaning generated output, remove only known generated paths inside the repository.

## Git behavior

Before making broad changes, inspect:

```powershell
git status --short
```

Do not revert unrelated user changes.

Do not commit, push, tag, rebase, merge, or rewrite history unless explicitly instructed.

Keep changes focused on the current task.

Do not mix unrelated cleanup or broad renaming into a feature implementation.

## Documentation

Update documentation when behavior, commands, installation, configuration, or architecture changes.

Command examples for Windows should use PowerShell syntax.

Use fenced code blocks labeled `powershell`, `csharp`, `json`, `yaml`, or another accurate language.

Do not claim a feature is implemented merely because a placeholder, interface, or command stub exists.

Clearly distinguish:

* implemented behavior
* planned behavior
* known limitations

## Workflow for implementation tasks

For nontrivial changes:

1. Inspect the repository and existing conventions.
2. Check Git status.
3. Restate the bounded implementation goal.
4. Identify the projects and files likely to change.
5. Implement the smallest coherent vertical slice.
6. Add or update tests.
7. Run targeted tests.
8. Run the full test suite when practical.
9. Build the solution.
10. Review the final diff.
11. Summarize:

    * behavior implemented
    * important design decisions
    * tests executed
    * known limitations
    * suggested next vertical slice

Do not continue into the next roadmap item without being asked.

## Initial bootstrap boundary

The first bootstrap task should establish:

* a working Spectre.Console.Cli application
* the root `ccbstack` command
* `dotnet ccbstack version`
* a reusable core version result
* human-readable version output
* a basic structured-output path if included in the task
* predictable exit codes
* tests
* .NET tool package metadata
* local packaging and installation documentation

The first bootstrap task should not implement:

* repository inspection
* configuration loading
* doctor checks
* safety rules
* freeze state
* skill manifests
* skill rendering
* process execution
* Git integration
* release automation
* dependency injection frameworks unless currently needed

## Definition of done

A task is complete only when:

* the requested behavior is implemented
* the relevant projects build
* tests pass
* public behavior is documented
* error handling is reasonable
* no unrelated changes were introduced
* the final response accurately reports what was and was not completed
