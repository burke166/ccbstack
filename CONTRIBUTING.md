# Contributing to ccbstack

Thank you for your interest in contributing to **ccbstack**!

## Project Philosophy

ccbstack is primarily a personal development framework created to support my own software engineering workflow. While the repository is public and outside contributions are welcome, the project is **not** attempting to satisfy every possible workflow or development style.

Design decisions are guided by a few core principles:

- Windows-first development
- .NET-first tooling
- PowerShell instead of Bash whenever practical
- Simplicity over configurability
- AI-assisted software engineering
- Safe-by-default automation

Not every suggestion will align with those goals, and that's okay.

## Before Contributing

If you're planning a significant feature or architectural change, please open an issue first so we can discuss whether it fits the direction of the project.

Small bug fixes and documentation improvements generally don't require prior discussion.

## Pull Requests

Please keep pull requests:

- Focused on a single logical change
- Small enough to review comfortably
- Accompanied by documentation updates when appropriate
- Compatible with the existing coding style

## Coding Standards

General expectations include:

- Use modern C# and current .NET features.
- Prefer readability over cleverness.
- Avoid unnecessary dependencies.
- Write PowerShell for Windows automation.
- Add XML documentation where it improves understanding.
- Include tests for new functionality when practical.

## Skill Development

When adding or modifying skills:

- Keep prompts concise.
- Avoid unnecessary token usage.
- Prefer deterministic behavior over "creative" prompting.
- Consider how the skill composes with other skills.
- Update documentation when behavior changes.

## Documentation

Good documentation is as valuable as code.

If you add functionality, please update any relevant:

- README
- CLAUDE.md
- Skill documentation
- Examples

## Feature Acceptance Criteria

New features should generally satisfy at least one of the following:

- Reduce repetitive developer work.
- Improve AI-assisted development.
- Increase safety or prevent mistakes.
- Improve cross-project reuse.
- Reduce setup or maintenance complexity.
- Improve Windows or .NET integration.

Features that primarily add configuration, abstraction, or support for unrelated ecosystems are unlikely to be accepted unless they provide substantial benefit.

## Licensing

By submitting a contribution, you agree that it may be distributed under the project's license.

## Code of Conduct

Please be respectful and constructive. Disagreement is expected and discussion encouraged; personal attacks are not acceptable.

## Final Note

This project exists because it solves problems I encounter while building software. If your workflow is different, that's perfectly fine—forking the repository may be a better fit than trying to generalize every feature.

Thank you for taking an interest in ccbstack.