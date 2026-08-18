# Design-doc workflow — shared reference

Shared by `init-repo` and `adopt-repo`. Both skills load this file when
writing or improving a generated project's `CLAUDE.md`, to produce a fuller
design-first section than a one-line mention. Adapt this content to the
target project — its terminology, technology, and concrete examples — rather
than copying it verbatim. This file is not itself a design document; it is
guidance for what a *generated* project's `CLAUDE.md` should say about its
own design-first workflow.

## When a design document is required

A design document is normally appropriate before work that involves:

* significant architecture, module, or structural changes;
* a new external service, infrastructure component, or integration;
* build, deploy, or CI/CD pipeline changes;
* new or materially changed data models;
* security- or privacy-sensitive functionality;
* changes with substantial backward-compatibility impact (data formats,
  APIs, URLs, file formats, on-disk layouts);
* introducing a new dependency, framework, or build tool;
* changes that affect multiple major areas of the project;
* decisions a future maintainer would reasonably need to understand.

Routine changes normally do **not** require a design document. Examples:

* content or copy edits;
* typo fixes;
* small styling or formatting adjustments;
* small, self-contained bug fixes;
* correcting broken links or references;
* replacing an asset (image, icon, etc.) with an equivalent;
* straightforward dependency version updates;
* maintenance that does not change an established design decision.

When it's unclear whether a change needs one, prefer writing a short design
proposal over making an undocumented architectural decision.

## Status lifecycle

Design documents use one of three statuses:

* **Proposed** — initial design awaiting human review.
* **Accepted** — explicitly reviewed and approved for implementation.
* **Implemented** — implementation and verification are complete and the
  document reflects the resulting system.

The normal workflow:

1. Inspect the existing implementation.
2. Create or update a design document with status `Proposed`.
3. Stop and request review.
4. Incorporate requested changes into the design.
5. Change the status to `Accepted` only after the design has been approved
   by a human.
6. Implement the accepted design.
7. Verify the implementation.
8. Update the design document to reflect implementation decisions that
   differ from the original proposal.
9. Change the status to `Implemented`.

Do not implement a `Proposed` design unless explicitly instructed to
proceed. Do not mark a design `Accepted` merely because it has been written
— only a human approves that transition. Do not mark a design `Implemented`
until the corresponding implementation and verification are actually
complete.

Documents with status `Implemented` should collectively describe the
project's current architecture and significant design decisions. Keep them
accurate: if later work materially changes a documented decision, update the
existing document or create a superseding one rather than letting it go
stale.

## Design document structure

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

Not every section requires extensive content, and sections that genuinely
don't apply may be omitted rather than filled with boilerplate.

Design documents should explain **why** a decision is being made — they
should not merely list the files that will change.

* `Current State` should be based on inspection of the repository, not
  assumptions.
* `Alternatives Considered` should record meaningful alternatives and why
  they were rejected.
* `Implementation Plan` should be specific enough that implementation can
  proceed from the accepted design without rediscovering major decisions.
* `Validation` should describe how the completed implementation will be
  verified.

## Designing and implementing

When designing a significant change, focus on understanding: the problem,
the existing implementation, constraints, goals and non-goals, reasonable
alternatives, security/privacy implications, the proposed implementation,
and validation requirements. Do not begin modifying production code unless
implementation is also explicitly requested.

When implementing an accepted design, treat the design document as the
primary specification, but verify its assumptions against the current
repository before making changes. If the accepted design conflicts with the
current repository, stop and explain the conflict rather than silently
changing the design or the implementation. If implementation reveals that
the accepted design needs a significant change, update the design and
request review before proceeding with that deviation. Small implementation
details that don't materially change the accepted design may be resolved
during implementation and documented afterward, without another review
cycle.

The purpose of this process is to make significant decisions explicit and
reviewable — not to add ceremony to routine maintenance.
