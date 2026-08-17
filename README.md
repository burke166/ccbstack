# ccbstack

ccbstack is a collection of reusable Claude Code skills for software development.

It provides structured workflows for creating, adopting, designing, developing, reviewing, testing, and maintaining software repositories with Claude Code.

ccbstack is **skills-first**. It relies on Claude Code's existing capabilities and standard development tools wherever practical rather than introducing a separate framework or runtime.

## Goals

ccbstack is intended to:

* establish a consistent Claude-assisted development workflow across projects;
* create useful project-specific `CLAUDE.md` instructions;
* encourage design before implementation;
* automate repetitive repository setup;
* provide reusable development, review, and testing workflows;
* support multiple languages and application types;
* preserve project-specific architecture and conventions;
* add deterministic tooling only where it provides demonstrated value.

The goal is not to impose one architecture on every project.

A .NET web application, Go command-line utility, Hugo site, and WordPress theme should each receive structures and instructions appropriate to what they are.

## Status

ccbstack is in early development.

Two skills are implemented and tested against all six initial project families
(C#/.NET, Go, Hugo, Svelte, WordPress themes, static HTML5):

* `init-repo` — creates and scaffolds a new repository.
* `adopt-repo` — examines an existing repository and brings it into the ccbstack workflow.

Additional skills will be added as the development workflow evolves.

## Repository Structure

```text
ccbstack/
├── CLAUDE.md
├── README.md
├── CHANGELOG.md
├── .gitignore
├── .claude-plugin/
│   ├── plugin.json
│   └── marketplace.json
├── docs/
│   └── design/
└── skills/
    ├── init-repo/
    └── adopt-repo/
```

### `skills/`

Contains reusable Claude Code skills.

Skills may include supporting reference material, templates, scripts, or other resources when needed.

### `docs/design/`

Contains design documents for skills and other significant ccbstack features.

Non-trivial functionality should be designed and reviewed before implementation.

### `.claude-plugin/`

Contains the plugin manifest (`plugin.json`) and marketplace catalog
(`marketplace.json`) that let ccbstack be installed as a Claude Code plugin —
see [Installation](#installation) below.

## Installation

ccbstack is distributed as a Claude Code plugin. `marketplace.json` points
back at this same repository, so no separate hosting or publishing is
required — install directly from a local clone:

```text
/plugin marketplace add /path/to/ccbstack
/plugin install ccbstack@ccbstack
```

If the install summary says `Run /reload-plugins to activate.`, run that too.
Choosing the `user` install scope makes `init-repo`/`adopt-repo` available
across all of your repositories; `project`/`local` scope ties them to a
single repository.

## Repository Workflow

ccbstack follows a design-first development process.

For significant changes:

1. Inspect the existing repository and relevant skills.
2. Create or update a design document under `docs/design/`.
3. Define the desired behavior, decisions, edge cases, and acceptance criteria.
4. Review and revise the design.
5. Implement the approved design.
6. Test the implementation against realistic repositories.
7. Update relevant documentation.

If implementation reveals a significant problem with the design, update the design rather than silently diverging from it.

## Initial Skills

### `init-repo`

`init-repo` starts a new software repository.

The skill asks for the repository name and whether it should create a new repository directory or initialize the current directory.

This allows workflows such as running the skill from a common projects directory:

```text
Projects/
```

and having it create:

```text
Projects/
└── ExampleProject/
```

or entering an already-created directory and initializing the project there.

The skill is responsible for establishing the initial repository, including:

* Git initialization;
* an appropriate `.gitignore`;
* a project-specific `CLAUDE.md`;
* `README.md`;
* `CHANGELOG.md`;
* the standard documentation structure;
* an appropriate initial project scaffold;
* validation that the generated project works.

Initial target project families include:

* C#/.NET;
* Go;
* Hugo;
* Svelte;
* WordPress themes;
* static HTML5 websites.

The generated structure should depend on what is actually being built rather than applying a single template to every repository.

### `adopt-repo`

`adopt-repo` brings an existing software repository into the ccbstack workflow.

Rather than scaffolding a new application, it first examines the repository to understand:

* its purpose;
* languages and frameworks;
* application type;
* build system;
* existing architecture;
* documentation;
* testing approach;
* repository conventions.

It then adds missing ccbstack conventions where appropriate, including:

* a useful project-specific `CLAUDE.md`;
* standard documentation files;
* `docs/design/`;
* an appropriate `.gitignore`.

Existing project conventions should be preserved unless there is a specific reason to change them.

`adopt-repo` should adapt ccbstack to the project rather than forcing the project into a predefined structure.

## Project-Specific CLAUDE.md

A central goal of ccbstack is creating useful repository-specific instructions for Claude Code.

A generated `CLAUDE.md` should describe the actual project rather than simply copying a generic template.

Depending on the project, it may document:

* project purpose;
* language and framework;
* architecture;
* important dependencies;
* build commands;
* test commands;
* development principles;
* coding conventions;
* documentation practices;
* Git practices;
* project-specific constraints;
* design and implementation workflow.

Only information useful to Claude while working in that repository should be included.

## Skills Before Tooling

ccbstack intentionally begins as a collection of Claude Code skills rather than a standalone application.

Claude Code can already inspect repositories, manipulate files, execute development tools, and reason about project structure. ccbstack should build on those capabilities.

Supporting scripts or command-line utilities may be introduced when actual skill usage demonstrates that deterministic tooling would improve reliability, safety, or maintainability.

The intended progression is:

```text
Skill
  │
  ├── Claude reasoning and orchestration
  │
  └── standard development tools
          │
          ▼
   repeated need discovered
          │
          ▼
  small deterministic utility
          │
          ▼
  shared tooling if justified
```

Tooling should emerge from demonstrated requirements rather than anticipated ones.

## Supported Platforms

ccbstack is developed with a **Windows-first** workflow, particularly Windows 11 and PowerShell 7.

Skills should nevertheless avoid unnecessary platform dependencies.

Project-specific tools naturally depend on the technology being used. For example, initializing a .NET project requires the .NET SDK, while creating a Hugo site requires Hugo.

Cross-platform behavior should be preserved where doing so does not significantly complicate the skill.

## Future Development

Potential future skills include workflows for:

* specification and design;
* engineering planning;
* code review;
* investigation and debugging;
* .NET development;
* Go development;
* PowerShell development;
* database review;
* API review;
* CLI review;
* testing and QA;
* release documentation;
* repository safety;
* learning from completed development work.

These are directions rather than commitments.

ccbstack will grow based on workflows that prove useful in actual development.

## License

MIT. See [LICENSE](LICENSE).