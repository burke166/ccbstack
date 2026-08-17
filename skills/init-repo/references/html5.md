# Static HTML5 — init-repo reference

This is the one project family with no official scaffolding tool. Build the
minimal structure by hand, deliberately kept small — resist the urge to add
tooling this family doesn't need.

## Questions to ask

Ask, do not guess:

* **Single page or multiple pages?** Determines whether to scaffold just
  `index.html` or an initial small set of pages.
* Whether any build/dev tooling is wanted at all (e.g. a simple dev server,
  a CSS preprocessor). Default to **no build tooling** — plain HTML, CSS, and
  JS served as-is — unless the user asks for something specific. Do not
  introduce Node.js/npm merely to serve static files.

## Scaffolding

Hand-create a minimal, semantic structure:

```text
index.html
css/
└── style.css
js/
└── main.js
assets/
```

`index.html` should be a complete, valid HTML5 document (`<!DOCTYPE html>`,
`<html lang="...">`, `<head>` with `<meta charset>`, a `<title>`, and a
viewport meta tag, `<body>`), linking `css/style.css` and, if any JS is
needed, `js/main.js`. Do not add a `js/` directory or `<script>` tag if the
user has no scripting needs yet.

If multiple pages were requested, create one `.html` file per page at the
root (or in subdirectories that mirror the desired URL structure), each a
complete HTML5 document, sharing `css/style.css`.

## `.gitignore` content

```gitignore
.DS_Store
Thumbs.db
*.swp
```

Add `/dist/` or `/build/` only if the user opted into some build tooling in
the questions step.

## `CLAUDE.md` guidance

Cover, as relevant:

* **Project purpose** — what the site is for.
* **Technology** — plain HTML5/CSS/JS, explicitly no framework or build
  step (unless one was added) — state this so future changes don't
  casually introduce one.
* **Structure** — where pages, styles, and scripts live.
* **Conventions** — semantic HTML, accessible markup (alt text, heading
  order, label associations), mobile-responsive CSS.
* **Browser support** — note any specific target if the user states one;
  otherwise note "modern evergreen browsers" as the default assumption.
* **Dependencies** — keep at zero unless the user explicitly adds
  something; justify any addition.
* **Design-first workflow** — design docs for significant changes live in
  `docs/design/`, status Proposed → Accepted → Implemented.

## Validation

There is no build step to run. Instead:

* Confirm every generated `.html` file has a `<!DOCTYPE html>` declaration,
  a `<title>`, and correctly closed tags (a quick structural read is
  sufficient — this family has no official scaffolder or linter to defer
  to).
* If Node.js is available and the user wants stricter validation, offer to
  run `npx html-validate *.html`, but treat this as optional, not a
  blocking requirement, since it introduces a Node.js dependency this
  family otherwise avoids entirely.

Treat a malformed `index.html` (missing doctype, unclosed root tags) as
blocking; treat a missing optional validator as informational only.
