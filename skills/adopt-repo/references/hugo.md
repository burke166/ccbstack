# Hugo — adopt-repo reference

## Detection signals

* A Hugo config file at the root (`hugo.toml`, `hugo.yaml`, `hugo.json`, or
  the older `config.toml`/`config.yaml`/`config.json`), typically containing
  a `baseURL` key.
* `content/` and `layouts/` directories together (the combination is a much
  stronger signal than either alone, since other tools also use directories
  named `content` or `layouts`).
* `themes/<name>/` (Git submodule) or a `module.imports` entry referencing a
  theme (Hugo Modules) in the config file.
* `archetypes/` directory (optional but a good confirming signal).

## `.gitignore` content

If missing, use:

```gitignore
/public/
/resources/_gen/
.hugo_build.lock
node_modules/
```

Only include `node_modules/` if a CSS/JS build pipeline is actually present
(check for `package.json` before adding it). If a `.gitignore` already
exists, only add entries from this list that are missing.

## `CLAUDE.md` guidance

Document what's actually there:

* **Project purpose** — what the site is, from the README and content.
* **Technology stack** — Hugo, the actual theme in use (or "no theme, custom
  layouts" if `layouts/` is hand-built), any CSS/JS pipeline actually
  configured, and the actual deployment target if it's discoverable (CI
  config, a documented hosting target) — don't invent one if it isn't stated
  anywhere.
* **Site structure** — the real `content/`/`layouts/`/`static/`-or-`assets/`
  layout and what the config file actually sets.
* **Content conventions** — front matter fields actually in use, Markdown
  conventions already followed.
* **Build/dev commands** — `hugo server`, `hugo` (or `hugo --minify` if that's
  what CI/deploy scripts actually use).
* **Design-first workflow** — add this section if the repository doesn't
  already describe an equivalent process.

Do not add scope-creep warnings (e.g. "don't introduce a JS framework") unless
the existing repository already establishes that constraint — `adopt-repo`
documents what's there, it doesn't impose `init-repo`'s defaults onto an
existing project.

## Verification

```text
hugo
```

Treat build errors as worth surfacing in the report. Warnings about missing
layouts/shortcodes are common in partially-built sites and don't need to
block anything — just note them if they look like a real gap rather than
expected in-progress content.
