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
- `.claude-plugin/plugin.json` plugin manifest, so ccbstack can be installed
  as a Claude Code plugin in other repositories.
- `docs/design/01_init-repo.md` and `docs/design/02_adopt-repo.md` design
  documents.
- `docs/examples/ai_instructions/` reference `CLAUDE.md` examples used to
  calibrate the documentation both skills generate.