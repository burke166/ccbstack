# External integrations — shared reference

Shared by `init-repo` and `adopt-repo`. Unlike
`skills/references/engineering-values.md`, this file is **conditional**:
load and apply it only when the project actually talks to an
external/remote/networked system (a third-party API, a cloud service, an
SMTP server, a subprocess, a filesystem boundary worth calling out, a
database), or has meaningful runtime configuration or logging. Skip it
entirely for a project with none of that — a small local-computation
library with no I/O beyond ordinary function calls should not get an
Automation, Integration Testing, Logging, or Configuration section just
because this file exists. Determine applicability from the family and
questions already gathered (`init-repo`) or from what inspection actually
found (`adopt-repo`) — don't guess.

Adapt this content to the target project's actual external dependency
rather than copying any example verbatim.

## Automation

State plainly that the project talks to external systems, and that calls
to them can fail: APIs error, networks disconnect, authentication expires,
rate limits apply. Code that depends on these systems should degrade
gracefully rather than assuming success.

## Integration testing

This is a pattern to apply, not a fixed list to copy. Two real projects
that both depend on external systems can need entirely different
integration-test guidance: a project that sends email over SMTP needs tests
that don't depend on a live mail server; a project that downloads and
installs software from GitHub releases needs tests that don't depend on
live downloads, archive extraction, and process launching actually
succeeding end-to-end. Same shape, different specifics.

To apply it:

1. Identify this project's own external/remote dependency — the actual
   thing it talks to (a network API, an SMTP server, a filesystem, a
   subprocess, a cloud download, a database).
2. Write the section around *that* dependency specifically: what are the
   two or three highest-value scenarios worth testing without hitting the
   live system (e.g. "downloading a release archive from a test HTTP
   server," "extracting an archive into a temporary directory," "detecting
   installed versions from the filesystem" for a download-and-install tool;
   "composing and handing off a message to a fake transport" for an email
   library)?
3. State the general rules that apply regardless of the specific
   dependency:
   * Favor fakes and in-memory implementations for unit tests.
   * Reserve tests against the live external system for integration,
     manual, or end-to-end tests, explicitly marked as such — not the
     default automated test run.
   * For workflows that span multiple components, prefer an integration
     test over more unit tests that only cover pieces in isolation.
   * Integration tests should use temporary directories/resources, avoid
     touching real user data or installations, and clean up after
     themselves.

For projects with enough automated testing to warrant it, a three-tier
structure works well: pure unit tests (parsing, calculations, pure logic)
→ integration tests (multi-component workflows against fakes or local
test doubles) → a small number of end-to-end tests (the real thing,
run rarely).

## Logging

State what's normally worth logging for *this* project — meaningful state
transitions, external call failures, retries, startup/shutdown, or
whatever categories of event actually help diagnose a production issue for
this specific system. Never log secrets or authentication tokens.

## Configuration

* Configuration belongs outside code.
* Use strongly typed options where the language/framework supports it.
* Validate configuration at startup.
* Fail fast on invalid configuration rather than proceeding with defaults
  that mask the problem.
