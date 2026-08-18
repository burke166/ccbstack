# CLAUDE.md

## ccbstack

ccbstack is a collection of reusable Claude Code skills for software
development.

Its purpose is to establish a consistent AI-assisted development workflow across
different repositories, languages, frameworks, and project types without
imposing unnecessary architecture or tooling on those projects.

ccbstack is Windows-first but should avoid unnecessary platform dependencies
when a skill can operate through Claude Code itself.

---

## Core Principles

When developing ccbstack, follow these priorities in order:

1. Simplicity
2. Correctness
3. Maintainability
4. Portability
5. Automation

Do not introduce infrastructure until demonstrated use cases justify it.

Prefer Claude Code's existing capabilities over custom tooling when Claude can
perform the task reliably.

Add deterministic scripts or command-line tools when repeated experience
demonstrates that deterministic behavior is preferable to AI reasoning.

Do not build tooling merely because it may become useful later.

Inspect before assuming.

Design significant changes before implementing them.

Verify work before reporting it as complete.

---

## Skills Are the Product

The primary product of ccbstack is its collection of Claude Code skills.

Skills should:

* encode reusable development workflows;
* provide enough structure to produce consistent results;
* allow Claude to exercise judgment where projects differ;
* use existing language and framework tooling when appropriate;
* avoid unnecessary dependencies;
* remain understandable and maintainable;
* be testable against real repositories;
* remain independently understandable and usable whenever practical.

Supporting scripts, templates, reference material, examples, and command-line
utilities exist to support skills rather than define the architecture of
ccbstack.

Do not introduce a ccbstack application or command-line tool unless concrete
skill requirements demonstrate a need for one.

---

## Development Workflow

### Inspect Before Changing

Do not make assumptions about a repository when they can be verified by
inspection.

Before proposing significant changes:

* inspect the relevant files and directories;
* identify existing languages, frameworks, tooling, and dependencies;
* identify existing repository conventions;
* inspect existing documentation and configuration;
* determine how the project is currently built and tested;
* understand the current implementation relevant to the proposed change;
* preserve intentional existing decisions unless there is a reason to change
  them.

Base design documents and implementation decisions on the repository's actual
current state rather than assumptions.

### Design First

Significant ccbstack changes should be designed before implementation.

A design document is normally required when work:

* adds a new skill;
* materially changes an existing skill's behavior;
* changes repository structure or conventions;
* introduces a new dependency or supporting tool;
* changes generated repository structure;
* changes behavior shared by multiple skills;
* introduces a significant architectural decision.

Small bug fixes, documentation corrections, formatting changes, and
implementation work already covered by an Accepted design normally do not
require a new design document.

Design documents belong in:

```text
docs/design/
```

For significant work:

1. Inspect the existing repository and relevant implementation.
2. Create or update a design document under `docs/design/`.
3. Give the design the status `Proposed`.
4. Describe the problem, goals, proposed behavior, important decisions,
   alternatives, failure cases, and validation.
5. Present the design for review.
6. Stop and wait for approval before implementation.
7. Incorporate requested changes into the design.
8. Change the status to `Accepted` only after explicit user approval.
9. Implement according to the Accepted design.
10. Verify the implementation.
11. Update the design to reflect significant implementation decisions or
    necessary deviations.
12. Change the status to `Implemented` when implementation and verification are
    complete.

Only the user may approve a Proposed design.

Claude may recommend that a design is ready for acceptance but must not change
its status to `Accepted` without explicit user approval.

Do not implement a `Proposed` design unless explicitly instructed to proceed
with implementation.

If implementation reveals a significant flaw or missing requirement, update the
design rather than silently diverging from it.

### Implement Accepted Designs

Once a design has been Accepted, treat it as the authoritative description of
the intended behavior.

Implementation should follow the Accepted design unless:

* the user changes the requirements;
* repository inspection reveals that an assumption in the design was incorrect;
* implementation reveals a significant technical problem;
* a safer or substantially simpler approach becomes apparent.

When a significant deviation is necessary, update the design and explain the
reason rather than silently changing direction.

Implementation should not rediscover architectural decisions that should already
have been resolved by the design.

### Verify the Work

Do not report work as complete merely because files were created or modified.

Use the project's normal tooling to verify changes whenever practical.

Depending on the change, verification may include:

* inspecting generated files;
* building the project;
* running automated tests;
* running formatters or linters;
* validating configuration;
* executing the generated application;
* checking generated documentation;
* inspecting Git diff and Git status.

Report the verification performed and any verification that could not be
performed.

Claude may change an Accepted design's status to `Implemented` after the
authorized implementation has been completed and successfully verified.

---

## Skill Architecture

### Skill Naming

Prefer lowercase verb-noun names for workflow skills.

Names should describe the action being performed and, when appropriate, its
target.

Examples:

* `init-repo`
* `adopt-repo`
* `review-code`
* `test-dotnet`

Prefer names that resemble PowerShell's Verb-Noun convention when practical.

State or mode skills may use a concise noun or adjective when a verb-noun name
would be unnatural.

### Skill Independence

Skills should be independently understandable and usable whenever practical.

Do not require one skill to invoke another merely to reuse instructions.

Shared behavior may be extracted into supporting resources when multiple skills
genuinely require the same rules.

Avoid hidden dependencies between skills.

When a skill depends on another skill, script, external tool, or resource, make
that dependency explicit.

### Skill Resources

Keep the primary skill instructions focused on the workflow Claude must follow.

A skill may contain supporting resources when they make the skill clearer, more
maintainable, or more reliable.

Use resources according to their purpose:

```text
SKILL.md      workflow and decision-making instructions
references/   information Claude consults when needed
templates/    files or structures intended to be instantiated
scripts/      deterministic operations where code is preferable to reasoning
examples/     illustrative outputs rather than mandatory templates
```

These names describe preferred organization rather than mandatory empty
directories.

Create a resource directory only when the skill actually requires it.

Move detailed technology-specific guidance into supporting resources when doing
so keeps the primary skill understandable.

Do not duplicate large amounts of technology-specific guidance in the primary
skill when it can be isolated cleanly.

### Deterministic Tooling

Prefer Claude Code's existing capabilities when Claude can perform an operation
reliably.

Prefer deterministic scripts or tools when an operation:

* must produce repeatable machine-readable results;
* is error-prone when performed through natural-language reasoning;
* must enforce an invariant;
* performs a safety-sensitive check;
* requires behavior that is difficult to express reliably as skill instructions;
  or
* has demonstrated through repeated use that deterministic execution is
  preferable.

Do not introduce repository-wide runtime dependencies merely to replace
capabilities Claude Code already provides adequately.

When deterministic tooling becomes necessary, design it from demonstrated
requirements rather than anticipated future needs.

---

## Initial Repository Skills

The first two repository-management skills are `init-repo` and `adopt-repo`.

### init-repo

`init-repo` creates a new repository from an empty starting point.

At a minimum, it is responsible for:

* asking for the repository name;
* asking whether a new repository directory should be created;
* supporting execution from a parent directory such as `Projects`;
* supporting execution from an already-created project directory;
* initializing Git;
* creating an appropriate `.gitignore`;
* creating a project-specific `CLAUDE.md`;
* creating an initial `README.md`;
* creating an initial `CHANGELOG.md`;
* creating the standard ccbstack repository folders;
* determining the type of project being created;
* gathering the information necessary to scaffold that project appropriately;
* scaffolding a minimal initial solution or application;
* verifying the generated project using the project's normal tooling.

`init-repo` should gather enough information to choose an appropriate scaffold
before creating project-specific application structure.

The conceptual sequence is:

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

Do not assume that selecting a language or framework provides enough information
to choose a scaffold.

For example, knowing that a repository will use .NET does not establish whether
it should contain a class library, console application, command-line tool, Razor
Pages application, Web API, worker service, or multiple projects.

Prefer the smallest scaffold that establishes a working foundation for the
requested project.

The authoritative list of supported project families and project-specific
behavior belongs in the `init-repo` skill and its supporting documentation and
resources rather than in this file.

Detailed behavior will be defined in a separate design document before
implementation.

### adopt-repo

`adopt-repo` brings an existing repository into the ccbstack development
workflow.

It must inspect the existing repository before making changes.

At a minimum, it is responsible for:

* understanding the repository's purpose;
* identifying its languages, frameworks, build systems, and application type;
* understanding the existing repository structure and conventions;
* identifying existing build, test, formatting, and development workflows;
* preserving appropriate existing conventions;
* creating or improving `CLAUDE.md`;
* creating missing standard documentation files when appropriate;
* creating missing ccbstack repository folders when appropriate;
* creating or improving `.gitignore` when appropriate;
* avoiding unnecessary restructuring of existing projects;
* verifying appropriate changes when possible;
* reporting what it changed and why.

`adopt-repo` should adapt ccbstack to the repository rather than forcing an
existing repository into a rigid template.

Do not replace working project conventions merely because `init-repo` would have
made different choices for a new repository.

Detailed behavior will be defined in a separate design document before
implementation.

---

## Repository Structure

The initial ccbstack repository should remain small.

Expected structure:

```text
ccbstack/
├── CLAUDE.md
├── README.md
├── CHANGELOG.md
├── .gitignore
├── docs/
│   └── design/
└── skills/
    ├── references/
    ├── init-repo/
    └── adopt-repo/
```

Additional directories should be introduced only when they have a concrete
purpose.

A skill may contain supporting references, templates, scripts, examples, or
other resources when they make the skill clearer or more reliable.

Do not create empty architectural layers or placeholder directories in
anticipation of future requirements.

---

## Repository Generation

### Project-Specific CLAUDE.md Files

One important responsibility of ccbstack is establishing useful
repository-specific Claude instructions.

Generated `CLAUDE.md` files must describe the actual project.

They should not simply copy the ccbstack `CLAUDE.md`.

A generated `CLAUDE.md` should capture relevant information such as:

* project purpose;
* language and framework;
* architecture;
* important dependencies;
* build and test commands;
* development principles;
* repository conventions;
* documentation expectations;
* Git practices;
* testing expectations;
* project-specific constraints;
* the design-first development workflow.

Include only sections relevant to the repository.

Avoid boilerplate that does not provide Claude useful information.

Repository-specific instructions should reflect decisions actually made for that
repository rather than generic ccbstack preferences that do not apply.

### Templates and Scaffolding

Avoid treating every project using a particular technology as having the same
architecture.

For example, a .NET repository may contain:

* a class library;
* a console application;
* a command-line tool;
* a Razor Pages application;
* a Web API;
* a worker service;
* multiple related projects.

Determine what is being built before selecting a scaffold.

Templates and reference material should provide good defaults without preventing
project-specific decisions.

Prefer official project scaffolding tools where they exist.

Examples include the appropriate tooling from the .NET, Go, Hugo, JavaScript, or
other relevant ecosystems.

Do not manually recreate generated project structures when an established
scaffolding tool provides the desired result.

Prefer the smallest scaffold that establishes a working foundation for the
requested project.

---

## Testing Skills

Skills should be tested against realistic repositories.

For repository initialization skills, prefer disposable test directories rather
than modifying ccbstack itself.

Test important variations independently.

Tests should verify both:

1. the structure, configuration, and documentation generated or modified by the
   skill; and
2. that the resulting project builds, tests, runs, or otherwise validates
   successfully using its normal tooling.

For `init-repo`, testing should eventually include representative projects from
supported project families.

For `adopt-repo`, testing should include repositories with different existing
structures and conventions.

Tests should include important failure and edge cases where practical.

When a skill failure reveals a generalizable problem, improve the skill rather
than adding instructions tailored only to the test repository.

---

## Git

Use Git for all ccbstack development.

Keep commits focused and understandable.

Do not commit:

* generated files that should not be versioned;
* temporary test repositories;
* build output;
* package caches;
* IDE state;
* credentials;
* secrets;
* other machine-specific artifacts.

Do not rewrite published history unless explicitly instructed.

Do not create commits, push branches, create tags, or otherwise publish changes
unless explicitly requested.

Before reporting implementation work as complete:

* inspect the Git diff;
* inspect Git status;
* verify that unrelated changes have not been introduced;
* verify that files which should not be committed are appropriately ignored.

Do not discard unrelated user changes.

---

## Documentation

Documentation is part of the implementation.

Update documentation when behavior, workflow, repository structure,
dependencies, or supported capabilities change.

Each documentation artifact has a distinct purpose:

* `CLAUDE.md` defines repository-specific instructions and development
  expectations for Claude.
* `README.md` describes ccbstack from the user's perspective.
* `CHANGELOG.md` records meaningful user-visible changes.
* `docs/design/` records significant behavioral and architectural decisions.
* `SKILL.md` describes how Claude performs a particular skill workflow.
* supporting skill resources provide detailed information needed by that
  workflow.

Avoid duplicating the same detailed documentation in multiple locations.

Prefer one authoritative location for a decision or behavior and reference it
conceptually from other documentation when necessary.

### Design Documents

Design documents belong under:

```text
docs/design/
```

Design documents should use one of these statuses:

* **Proposed** — initial design awaiting human review.
* **Accepted** — explicitly reviewed and approved for implementation.
* **Implemented** — implementation and verification are complete and the
  document reflects the resulting system.

Do not mark a design `Accepted` merely because it has been written.

Do not mark a design `Implemented` until the corresponding implementation and
verification are complete.

Documents with status `Implemented` should collectively describe the current
architecture and significant design decisions of ccbstack.

Keep Implemented design documents accurate.

If later work materially changes a documented design decision, update the
existing document or create a superseding design document as appropriate.

### Design Document Structure

Design documents should normally contain:

```markdown
# Title

Status: Proposed

## Problem

## Goals

## Non-Goals

## Current State

## Proposed Design

## Alternatives Considered

## Security and Privacy Considerations

## Implementation Plan

## Validation
```

Not every section requires extensive content.

Omit sections that genuinely do not apply rather than filling them with
boilerplate.

Design documents should explain **why** a decision is being made; they should
not merely list the files that will be changed.

The `Current State` section should be based on inspection of the repository
rather than assumptions.

The `Proposed Design` section should describe behavior and intent clearly enough
to resolve the important design decisions before implementation.

The `Alternatives Considered` section should record meaningful alternatives and
why they were rejected.

The `Implementation Plan` should be specific enough that implementation can
proceed from the Accepted design without rediscovering major architectural
decisions.

The `Validation` section should describe how the completed implementation will
be verified.

---

## Dependencies

Keep ccbstack dependencies minimal.

A skill may rely on tools that are inherent to the project it is creating or
managing.

For example, scaffolding a .NET project may reasonably require the .NET SDK.

Do not introduce repository-wide runtime dependencies merely to support
functionality Claude Code can already perform.

Do not add dependencies solely because they might become useful later.

If repeated workflows eventually require deterministic supporting software,
design that software based on demonstrated requirements before adding it.

Make external dependencies required by a skill explicit.

---

## Scope

ccbstack may eventually grow to include additional development, review, testing,
safety, release, repository intelligence, or orchestration capabilities.

Those future possibilities are not current requirements.

Do not design current architecture around speculative future capabilities.

Build the workflows that are useful now.

Allow the architecture to emerge from actual skill usage and demonstrated
requirements.
