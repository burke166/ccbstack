# WordPress theme — adopt-repo reference

## Detection signals

* `style.css` at the theme root containing a WordPress theme header block
  (`Theme Name:`, `Theme URI:`, `Author:`, `Description:`, `Version:`,
  `Text Domain:`).
* `functions.php` at the theme root.
* Classic theme markers: `index.php`, `header.php`, `footer.php`, and other
  template-hierarchy files at the root.
* Block/FSE theme markers: `theme.json` plus `templates/*.html` and
  `parts/*.html` instead of PHP templates.
* If the repository is a full WordPress install rather than just a theme
  (`wp-content/`, `wp-admin/`, `wp-config.php` present), treat that as
  outside this reference's scope — note it in the report as a larger surface
  than a single theme and describe what's found without attempting to
  document the whole install as if it were a theme.

## `.gitignore` content

If missing, use:

```gitignore
node_modules/
vendor/
.DS_Store
*.log
```

Add `/build/` or `/dist/` only if a JS/CSS build pipeline (e.g.
`@wordpress/scripts`) is actually present (`package.json` with that
dependency). If a `.gitignore` already exists, only add entries from this
list that are missing.

## `CLAUDE.md` guidance

Document what's actually there:

* **Project purpose** — what the theme/site is for, from the `style.css`
  header's `Description` field and the README if present.
* **Theme type** — classic vs. block, from the detection signals above.
* **Structure** — the real template hierarchy in use (classic) or
  `theme.json` + block templates (block theme); where custom PHP logic
  actually lives.
* **Existing conventions** — whether output is consistently escaped
  (`esc_html`/`esc_attr`/`esc_url`), whether scripts/styles are enqueued
  properly vs. inlined, what text domain is actually used for i18n — describe
  what's there; flag clear security gaps (e.g. unescaped output of user
  input) in the report rather than silently fixing code, per this skill's
  "documentation, not code changes" scope.
* **Build commands**, if a JS/CSS pipeline exists — the real `package.json`
  scripts.
* **Dependencies** — note any hard plugin dependency the theme assumes,
  found via `functions.php` (e.g. calls to functions from a specific
  plugin).
* **Design-first workflow** — add this section if the repository doesn't
  already describe an equivalent process.

## Verification

If `php` is installed, syntax-check every PHP file:

```text
find . -name "*.php" -print0 | xargs -0 -n1 php -l
```

(On Windows, iterate the files with PowerShell and run `php -l` on each
instead of relying on `xargs`.)

**If `php` is not installed**, fall back to a structural check instead of
skipping verification entirely: confirm `style.css` still has all required
header fields, and that `functions.php` (and `index.php` for classic themes)
exist and are non-empty. Report that PHP syntax checking specifically wasn't
possible and why, so the gap is visible.

If a JS/CSS build pipeline exists, also run its build command (e.g.
`npm run build`).
