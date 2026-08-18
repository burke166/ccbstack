# Changelog

All notable changes to this project are documented here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [Unreleased]

### Added

- `init-repo` skill: scaffolds a new repository for one of six project
  families (C#/.NET, Go, Hugo, Svelte, WordPress themes, static HTML5) or a
  generic fallback — Git init, a family-appropriate `.gitignore`, a
  project-specific `CLAUDE.md`, `README.md`, `CHANGELOG.md`, `docs/design/`,
  and a validated scaffold before the initial commit.
- `adopt-repo` skill: inspects an existing repository before changing
  anything, then creates or improves `CLAUDE.md`, `README.md`,
  `CHANGELOG.md`, `docs/design/`, and `.gitignore` — preserving intentional
  existing conventions and flagging discrepancies rather than silently
  overwriting them. Never commits on its own.
- `.claude-plugin/plugin.json` plugin manifest and `.claude-plugin/marketplace.json`
  catalog (self-referencing via `"source": "./"`), so ccbstack can be
  installed as a Claude Code plugin directly from a local clone via
  `/plugin marketplace add` + `/plugin install`, with no separate hosting or
  publishing required.
- `docs/design/01_init-repo.md` and `docs/design/02_adopt-repo.md` design
  documents.
- `docs/examples/ai_instructions/` reference `CLAUDE.md` examples used to
  calibrate the documentation both skills generate.
- `skills/references/design-docs.md`: shared design-doc workflow guidance
  used by both `init-repo` and `adopt-repo` when generating a project's
  `CLAUDE.md`, so the design-first section covers when a design document is
  required, its Proposed → Accepted → Implemented lifecycle, and its
  standard structure, instead of a one-line mention. Verified by manually
  walking both skills' `SKILL.md` workflows against disposable test
  repositories (a Go module for `adopt-repo`, a static HTML5 scaffold for
  `init-repo`) and confirming each generated `CLAUDE.md` adapted the shared
  guidance to that project rather than copying it verbatim.
- `docs/design/03_shared-design-doc-guidance.md` design document.