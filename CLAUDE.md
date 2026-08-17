# CLAUDE.md

## ccbstack

ccbstack is a collection of reusable Claude Code skills for software development.

Its purpose is to establish a consistent AI-assisted development workflow across different repositories, languages, frameworks, and project types without imposing unnecessary architecture or tooling on those projects.

ccbstack is Windows-first but should avoid unnecessary platform dependencies when a skill can operate through Claude Code itself.

---

## Core Principles

When developing ccbstack, follow these priorities in order:

1. Simplicity
2. Correctness
3. Maintainability
4. Portability
5. Automation

Do not introduce infrastructure until demonstrated use cases justify it.

Prefer Claude Code's existing capabilities over custom tooling when Claude can perform the task reliably.

Add deterministic scripts or command-line tools when repeated experience demonstrates that deterministic behavior is preferable to AI reasoning.

Do not build tooling merely because it may become useful later.

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
* be testable against real repositories.

Supporting scripts, templates, reference material, and command-line utilities exist to support skills rather than define the architecture of ccbstack.

Do not introduce a ccbstack application or command-line tool unless concrete skill requirements demonstrate a need for one.

---

## Skill Naming

Use lowercase verb-noun names for skills.

Names should describe an action and its target.

Examples:

* `init-repo`
* `adopt-repo`
* `review-code`
* `test-dotnet`

Prefer names that resemble PowerShell's Verb-Noun convention when practical.

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

Initial project families are expected to include:

* C#/.NET;
* Go;
* Hugo;
* WordPress themes;
* static HTML5 websites;
* Svelte applications.

Project-specific behavior belongs in the `init-repo` skill and its supporting resources rather than in this file.

### adopt-repo

`adopt-repo` brings an existing repository into the ccbstack development workflow.

It should inspect the existing repository before making changes.

At a minimum, it is responsible for:

* understanding the repository's purpose;
* identifying its languages, frameworks, build systems, and application type;
* understanding the existing repository structure and conventions;
* preserving appropriate existing conventions;
* creating or improving `CLAUDE.md`;
* creating missing standard documentation files when appropriate;
* creating missing ccbstack repository folders;
* creating or improving `.gitignore` when appropriate;
* avoiding unnecessary restructuring of existing projects;
* reporting what it changed and why.

`adopt-repo` should adapt ccbstack to the repository rather than forcing an existing repository into a rigid template.

Detailed behavior will be defined in a separate design document before implementation.

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
    ├── init-repo/
    └── adopt-repo/
```

Additional directories should be introduced only when they have a concrete purpose.

A skill may contain supporting references, templates, scripts, examples, or other resources when they make the skill clearer or more reliable.

Do not create empty architectural layers in anticipation of future requirements.

---

## Design-First Development

Non-trivial ccbstack features and skills should be designed before implementation.

Design documents belong in:

```text
docs/design/
```

For a new skill or substantial change:

1. Inspect the existing repository and relevant skills.
2. Create or update a design document under `docs/design/`.
3. Describe the problem being solved.
4. Define the proposed workflow and behavior.
5. Identify important decisions and alternatives.
6. Define expected inputs and outputs.
7. Define failure and edge cases.
8. Define acceptance criteria.
9. Present the design for review.
10. Do not begin implementation until the design has been approved.

The design document should describe behavior and intent rather than prematurely specifying implementation details.

Once a design has been approved, implement according to the approved design.

If implementation reveals a significant flaw or missing requirement, update the design rather than silently diverging from it.

---

## Skill Development

Keep the primary skill instructions focused on the workflow Claude must follow.

Move detailed technology-specific guidance into supporting resources when doing so keeps the primary skill understandable.

For example, `init-repo` may eventually contain references for:

```text
.NET
Go
Hugo
Svelte
WordPress
HTML5
```

Do not duplicate large amounts of technology-specific guidance in the primary skill when it can be isolated cleanly.

Prefer using official project scaffolding tools where they exist, such as the appropriate .NET, Go, Hugo, or JavaScript ecosystem tooling.

Do not manually recreate generated project structures when an established scaffolding tool provides the desired result.

---

## Project-Specific CLAUDE.md Files

One important responsibility of ccbstack is establishing useful repository-specific Claude instructions.

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

---

## Templates and Scaffolding

Avoid treating every project of a particular technology as having the same architecture.

For example, a .NET repository may contain:

* a class library;
* a console application;
* a command-line tool;
* a Razor Pages application;
* a Web API;
* a worker service;
* multiple related projects.

Determine what is being built before selecting a scaffold.

Templates and reference material should provide good defaults without preventing project-specific decisions.

Prefer the smallest scaffold that establishes a working foundation for the requested project.

---

## Testing Skills

Skills should be tested against realistic repositories.

For repository initialization skills, prefer disposable test directories rather than modifying ccbstack itself.

Test important variations independently.

For `init-repo`, this should eventually include representative projects from each supported project family.

Tests should verify both:

1. the structure and documentation generated by the skill; and
2. that the generated project builds or runs successfully using its normal tooling.

When a skill failure reveals a generalizable problem, improve the skill rather than adding instructions tailored only to the test repository.

---

## Git

Use Git for all ccbstack development.

Keep commits focused and understandable.

Do not commit generated files, temporary test repositories, build output, package caches, IDE state, credentials, or other machine-specific artifacts.

Do not commit secrets.

Do not rewrite published history unless explicitly instructed.

Do not create commits, push branches, create tags, or otherwise publish changes unless explicitly requested.

Before reporting implementation work as complete, inspect the Git diff and Git status.

---

## Documentation

Documentation is part of the implementation.

Update documentation when behavior, workflow, repository structure, or supported project types change.

Design documents describe why a feature should work a particular way.

Skill documentation describes how Claude should perform the workflow.

`README.md` describes ccbstack from the user's perspective.

`CHANGELOG.md` records meaningful user-visible changes.

Avoid duplicating the same detailed documentation in multiple locations.

---

## Dependencies

Keep ccbstack dependencies minimal.

A skill may rely on tools that are inherent to the project it is creating or managing.

For example, scaffolding a .NET project may reasonably require the .NET SDK.

Do not introduce repository-wide runtime dependencies merely to support functionality Claude Code can already perform.

If repeated workflows eventually require deterministic supporting software, design that software based on demonstrated requirements before adding it.

---

## Scope

ccbstack may eventually grow to include additional development, review, testing, safety, release, repository intelligence, or orchestration capabilities.

Those future possibilities are not current requirements.

Build the workflows that are useful now.

Allow the architecture to emerge from actual skill usage.
