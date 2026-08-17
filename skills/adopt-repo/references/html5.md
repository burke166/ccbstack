# Static HTML5 — adopt-repo reference

## Detection signals

* One or more `.html` files with no framework markers: no `package.json`
  referencing a JS framework, no Hugo/other SSG config, no WordPress theme
  header.
* Plain `css/`/`styles/` and `js/`/`scripts/` directories, or CSS/JS linked
  directly from the HTML with no build step.
* A `package.json`, if present at all, is limited to something like a local
  dev server or formatter — not a framework or bundler. If it *does* drive a
  real build (bundler, framework), this isn't static HTML5 — reconsider
  against the other families before falling back here.

## `.gitignore` content

If missing, use:

```gitignore
.DS_Store
Thumbs.db
*.swp
```

Add `/dist/` or `/build/` only if some build tooling is actually present. If
a `.gitignore` already exists, only add entries from this list that are
missing.

## `CLAUDE.md` guidance

Document what's actually there:

* **Project purpose** — what the site is for, from its content and any
  README.
* **Technology** — confirm it's genuinely plain HTML5/CSS/JS with no
  framework or build step; state this explicitly so future changes don't
  casually introduce one without it being a deliberate decision.
* **Structure** — where pages, styles, and scripts actually live.
* **Conventions** — note what's actually followed: semantic HTML, alt text,
  responsive CSS — or note gaps if the existing markup clearly lacks basic
  accessibility/responsiveness and that seems worth flagging (as a report
  note, not a code fix — see this skill's "documentation, not code changes"
  scope).
* **Browser support** — note any target stated anywhere in the repo;
  otherwise leave unstated rather than inventing one.
* **Dependencies** — note the real dependency count (usually zero); don't
  assume it should stay zero if the repository has already added something.
* **Design-first workflow** — add this section if the repository doesn't
  already describe an equivalent process.

## Verification

There is no build step for this family. Instead:

* Confirm each `.html` file has a `<!DOCTYPE html>` declaration, a `<title>`,
  and appears well-formed (a structural read is sufficient).
* If Node.js is available, optionally run `npx html-validate *.html` for a
  stricter check — treat this as informational, not blocking, consistent
  with this family having no official tooling of its own to defer to.

Report a malformed page (missing doctype, unclosed root tags) as a finding;
don't silently fix the markup — that's a code change outside this skill's
scope (see "Non-Goals" in the design).
