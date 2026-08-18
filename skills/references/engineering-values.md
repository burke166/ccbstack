# Engineering values — shared reference

Shared by `init-repo` and `adopt-repo`. Both skills load this file when
writing or improving a generated project's `CLAUDE.md`, regardless of
project family. Adapt this content to the target project rather than
copying it verbatim — this file is not itself a `CLAUDE.md`; it's a small
set of concrete, adaptable defaults.

This file is deliberately narrow. It covers only content that changes a
specific decision or action for a real project — not general
software-engineering sentiment ("measure first, optimize second," "avoid
premature abstraction") that any competent engineer, or Claude by default,
already follows regardless of what's being written into a `CLAUDE.md`.
`adopt-repo` may still document a project's real, specific, non-default
policy in any of those areas if inspection finds one — that's ordinary
"document what's actually there," not this file's job.

## Security baseline

* Treat external and user-supplied input as untrusted; validate before
  processing. Identify what "input" actually means for this project (HTTP
  request bodies, CLI arguments, file contents, form submissions, CSV/JSON
  payloads, etc.) rather than leaving the statement generic.
* Apply least privilege wherever the project grants access — file
  permissions, service accounts, deployment credentials, API scopes.
* Never hardcode secrets: passwords, API keys, connection strings, tokens,
  client secrets. State where they belong instead (environment variables,
  a secrets manager, CI/CD variables, local `.env` files excluded from Git)
  based on what the project actually uses.

## Dependencies

Before adding a third-party dependency, ask:

1. Is it necessary?
2. Is it maintained?
3. Can this reasonably be implemented without it?

State the project's own default posture toward third-party dependencies
(minimal vs. framework-heavy) based on what's actually true for it, rather
than asserting minimalism as a universal rule.

## Git

* Keep commits focused and understandable.
* Do not mix formatting changes with functional changes.
* Avoid mass file rewrites.
* Preserve file history whenever practical.

## Documentation (code-level)

* Public APIs should be documented.
* Complex algorithms or non-obvious decisions deserve a comment explaining
  *why*; simple, self-explanatory code should not be commented for its own
  sake.
* Update documentation when the architecture changes.

This is distinct from the design-doc *process* guidance in
`skills/references/design-docs.md` — that file covers when and how to write
a design document; this section covers ordinary code-level documentation.

## Technology stack and preferences

Every generated `CLAUDE.md` should state its preferred technologies
explicitly, using the facts already gathered during family
selection/inspection — and call out anything intentionally avoided, and
why, so a future contributor doesn't casually reintroduce it. This is an
instruction, not boilerplate text to copy: the actual list of preferred and
avoided technologies is different for every project and must come from
what was actually chosen (`init-repo`) or actually found (`adopt-repo`).
