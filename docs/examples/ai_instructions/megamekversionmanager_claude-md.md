# CLAUDE.md

## MegaMekVersionManager

MegaMekVersionManager is a Windows WPF application to manage installations
of MegaMek, user data, and associated Java runtimes on a Windows PC. It is
intended to make it easier to install and use MegaMek for less technical
Windows users.

MegaMek is a Java program. What we tend to refer to as "MegaMek" is a collection
of three programs packaged as a single downloadable archive as MekHQ:

- MegaMek: the adaptation of the BattleTech tabletop game as a graphical
application.
- MegaMekLab: the graphical application used to create or modify units for use
in the MegaMek game.
- MekHQ: the graphical application that manages multi-mission or multi-scenario
campaigns of MegaMek. Individual games or scenarios are played using the 
MegaMek application and results are tracked using MekHQ.

MegaMek allows users to play networked multi-player games, either hosted by
one of the players or hosted by an online server. All players and the server
must use the same version.

To use MegaMek on a Windows PC, users must download the MegaMek application
package, unzip the archive, and also maintain a Java runtime on their PC that
matches the requirements for any particular MegaMek version.

MegaMekVersionManager was created to make that process easier for less 
technical Windows users. It allows users to download MekHQ archives that 
contain all three programs in the MegaMek suite as well as Java runtimes 
necessary to run any particular version. These MegaMek-specific Java runtimes
are saved with MegaMekVersionManager where they can be independently used for
appropriate versions of MegaMek.

MegaMekVersionManager's responsibilities are:

- Downloading MekHQ releases from `https://github.com/MegaMek/mekhq/releases`
- Extracting each downloaded archive in its own folder
- Downloading the Adoptium Temurin JRE from `https://adoptium.net/temurin/releases`
- Extracting the JRE archive into its own folder
- Launching the appropriate MegaMek program from the MekHQ folder using the
required JRE. 

MegaMekVersionManager is designed for users who do not want to manage their own
installation folders, create shortcuts to specific folders, and manage installing
the required Java runtime.

---

## Design Philosophy

When making changes, follow these priorities in order:

1. Simplicity
2. Correctness
3. Maintainability
4. Performance

Do not introduce complexity unless there is measurable benefit.

Avoid premature optimization.

Favor explicit code over magic.

---

## Non-Goals

MegaMekVersionManager does not:

- modify MegaMek game files
- edit campaign data
- replace MegaMekLab
- replace MekHQ
- manage multiplayer servers

---

## Architecture

MegaMekVersionManager is a MVVM WPF app using `Microsoft.Extensions.Hosting`
for DI/config and Serilog for logging.

MegaMekVersionManager is designed as a thin application that has two
assemblies:

- MegaMekVersionManager: The WPF GUI application.
- MegaMekVersionManager.Test: The test suite.

MegaMekVersionManager is a MVVM application with the following dependency
direction:

Views
↓
ViewModels
↓
Services
↓
Infrastructure

ViewModels should coordinate application behavior through services.
Filesystem, network, and process-launching logic belong in services,
not view models.

MegaMekVersionManager coordinates existing tools rather than replacing them.
Business logic should remain focused on installation, version management,
Java management, and application launching. Avoid turning this application
into a MegaMek launcher ecosystem or campaign manager.

---

## External Dependencies

External Dependencies

MegaMekVersionManager depends on:

- GitHub Releases
- Adoptium downloads
- Windows filesystem
- Java runtimes
- Windows process launching

All external systems should be abstracted behind services.

Network operations should support retries where appropriate.

Filesystem operations should be recoverable whenever practical.

---

## Version Management Principles

Different MegaMek versions must coexist.

Installing one version must never modify another.

Each installation should be self-contained.

Java runtimes should be reusable but never overwrite incompatible versions.

Launching should always use the correct runtime for the selected version.

Installed versions should remain reproducible.

Given the same archive and Java runtime,
MegaMekVersionManager should produce the same installation layout.

---

## User Experience

MegaMekVersionManager is intended for users with little or no Java
experience.

Prefer designs that reduce manual configuration.

Prefer automatic detection over requiring user input.

Errors should clearly explain what happened, why it happened,
and how the user can resolve the problem.

---

## Development Philosophy

Assume this repository will exist for ten years.

Write code that another engineer can understand in five minutes.

Avoid creating abstractions for future requirements.

Only build what is currently needed.

---

## AI Expectations

Before making changes:

- Understand the surrounding code.
- Identify existing patterns.
- Extend existing patterns before creating new ones.

If a proposed design differs from existing conventions, explain why.

Never rewrite large portions of the project without justification.

Prefer incremental improvements.

---

## Coding Style

Prefer small classes.

Prefer small functions.

Functions should usually fit on one screen.

Avoid boolean flag parameters.

Avoid deep inheritance.

Prefer composition.

Use dependency injection where appropriate.

Avoid static mutable state.

---

## C#

Target the latest supported .NET LTS unless otherwise specified.

Enable nullable reference types.

Treat warnings as errors whenever practical.

Prefer async APIs.

Use cancellation tokens for long-running work.

Avoid synchronous blocking of async code.

Prefer records for immutable models.

---

## Testing

Every bug fix should include a regression test when practical.

Test observable behavior.

Do not test implementation details.

Favor fake external services and in-memory implementations
for unit tests.

Unit tests should not touch the filesystem unless filesystem behavior
is the subject under test.

Testing priorities:

1. Pure unit tests
   - parsing
   - version comparison
   - Java version selection
   - configuration

2. Integration tests
   - filesystem
   - ZIP extraction
   - download pipeline
   - installation
   - launching

3. End-to-end tests (few in number)
   - install a real MekHQ release
   - install a real JRE
   - launch MegaMek successfully

---

## Integration Testing

MegaMekVersionManager interacts heavily with the filesystem, network,
ZIP archives, and external processes. Many defects occur in the
interaction between these components rather than within individual
classes.

When practical, prefer integration tests over unit tests for workflows
that span multiple services.

High-value integration tests include:

- Downloading a release archive from a test HTTP server.
- Extracting a MekHQ archive into a temporary directory.
- Detecting installed MegaMek versions from the filesystem.
- Downloading and extracting a JRE.
- Selecting the correct JRE for a MegaMek version.
- Launching MegaMek with the expected Java executable and arguments.
- Recovering cleanly from interrupted downloads or extraction failures.

Integration tests should:

- Use temporary directories.
- Avoid modifying the user's real installation.
- Mock external network services whenever practical.
- Verify observable behavior rather than internal implementation.
- Clean up all temporary files after execution.

Do not write integration tests that depend on GitHub, Adoptium,
or other live Internet services unless they are explicitly marked
as manual or end-to-end tests.

Whenever fixing a bug, first determine whether the failure could have
been detected by an integration test. If so, prefer adding an
integration test before implementing the fix.

---

## Logging

Logging exists to diagnose production issues.

Log:

- download started
- download completed
- archive extracted
- JRE selected
- launch command
- GitHub API failures
- checksum validation
- extraction failures

Log user-visible failures with enough context to diagnose the cause.

Never log secrets.

Never log authentication tokens.

---

## Configuration

Configuration belongs outside code.

Use strongly typed options.

Validate configuration at startup.

Fail fast on invalid configuration.

---

## Security

Treat all external input as untrusted.

Validate before processing.

Escape before rendering.

Use least privilege.

---

## Dependencies

Minimize third-party dependencies.

Prefer Microsoft libraries when sufficient.

Before adding a package:

1. Is it necessary?
2. Is it maintained?
3. Can we reasonably implement the functionality ourselves?

---

## Documentation

Public APIs should be documented.

Complex algorithms deserve comments.

Simple code should not.

If the architecture changes, update documentation.

Significant architectural changes should begin with
a document in `docs/design` before implementation.

Use the `docs/design` folder to document proposed designs for application
features. The status of those designs should be `Proposed` for an initial
proposal, `Accepted` for a design that's been reviewed and pending
implementation, and `Implemented` for designs that have been implemented.
The set of documents in docs/design with a status of `Implemented`
should collectively describe the current architecture of the
application.

---

## Git

Keep commits focused.

Do not mix formatting with functional changes.

Avoid mass file rewrites.

Preserve file history whenever possible.

---

## Refactoring

Improve code when touching it.

Avoid drive-by refactors.

Large refactors should be proposed before implementation.

---

## Performance

Measure first.

Optimize second.

Document significant optimizations.

Readable code is preferred over micro-optimizations.

---

## Error Handling

Surface actionable errors to the user whenever practical.

Prefer explicit failures.

Prefer recovering from expected operational failures
(network, filesystem, missing files) while failing fast
for programming errors.

Do not silently swallow exceptions.

Return meaningful error information.

Retry only transient failures.

---

## Automation

MegaMekVersionManager interacts with external systems
to download MekHQ and JRE files (archives).

Assume:

- APIs fail.
- Websites go down.
- Networks disconnect.
- Rate limits exist.

Code should degrade gracefully.

---

## Agent Guidance

When uncertain:

- Ask questions.
- Do not invent APIs.
- Do not invent configuration.
- Do not assume database schemas.
- Search the repository first.

---

## Project Priorities

The project values:

- Reliability over features.
- Correctness over speed.
- Maintainability over cleverness.
- Consistency over novelty.
- Small improvements over rewrites.

Every contribution should leave the project slightly better than it was found.

---

## Repository Preferences

Preferred technologies

- C#
- .NET
- Microsoft.Extensions.*
- Dependency Injection
- Options Pattern
- Logging Abstractions

Avoid introducing

- ORM dependencies
- Database requirements
