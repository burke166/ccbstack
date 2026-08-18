# Hugo — init-repo reference

## Questions to ask

Ask, do not guess:

* **Theme** — an existing theme (name or Git URL) added as a Hugo Module or
  Git submodule, or no theme (build layouts from scratch / start minimal).
* **Content type** — roughly what the site is (marketing site, blog,
  documentation, organizational/public site) — this shapes what starter
  content and archetypes are worth generating, not the scaffolding command
  itself.
* Whether a CSS/JS build pipeline is wanted (e.g. Hugo Pipes with a framework
  like Bootstrap or Tailwind) or plain CSS/vanilla JS is sufficient. Do not
  add Node.js tooling unless the user asks for it or the chosen theme
  requires it.

## Scaffolding

```text
hugo new site . --force
```

(Use `.` and `--force` when initializing in an already-created target
directory per step 1 of the shared workflow; omit `--force` and pass the
directory name instead when creating a new subdirectory from a parent
directory.)

If a theme was requested, add it as a Hugo Module (preferred, avoids
submodule friction) where the theme supports it:

```text
hugo mod init <module-path>
hugo mod get <theme-module>
```

and reference it in `hugo.toml`/`config.toml`'s `theme` or `module.imports`
key. Fall back to a Git submodule (`git submodule add <theme-repo>
themes/<name>`) only if the theme doesn't support Hugo Modules.

If no theme was requested, leave `themes/` empty and note in the generated
`CLAUDE.md` that layouts live directly under `layouts/`.

## `.gitignore` content

```gitignore
/public/
/resources/_gen/
.hugo_build.lock
node_modules/
```

Only include `node_modules/` if a CSS/JS build pipeline was set up.

## `CLAUDE.md` guidance

Cover, as relevant:

* **Project purpose** — what the site is and who it's for.
* **Technology stack** — Hugo, theme (or "no theme, custom layouts"), any CSS
  framework, deployment target if known (ask the user; leave a placeholder
  noting "to be determined" if not yet decided rather than inventing one).
* **Design philosophy** — priority order; for public-facing sites this often
  includes Accessibility and Performance alongside Simplicity and
  Maintainability — ask whether accessibility standards (e.g. WCAG) apply
  rather than assuming.
* **Site structure** — `content/`, `layouts/`, `static/` or `assets/`, and
  the config file's role.
* **Content conventions** — front matter conventions, Markdown usage.
* **Build/dev commands** — `hugo server` for local development, `hugo` (or
  `hugo --minify`) to build.
* **Avoid scope creep** — do not turn the site into a web application when
  static HTML can solve the problem; do not add a JS framework (React, Vue,
  Svelte) unless there's a compelling requirement Hugo and CSS can't satisfy.
* **Design-first workflow** — see `skills/references/design-docs.md`.

Security, Dependencies, Git, and code-documentation guidance come from
`skills/references/engineering-values.md`, loaded and adapted regardless of
family — no need to restate them here.

## Validation

Run:

```text
hugo
```

Treat any build error as blocking. Warnings (e.g. about missing images or
unresolved shortcodes in placeholder content) should be reported but do not
have to block the commit unless they indicate a broken scaffold.
