# WordPress theme — init-repo reference

## Questions to ask

Ask, do not guess:

* **Classic (PHP template hierarchy) theme or block (full-site-editing)
  theme?** This determines the starter and the file layout.
* **Starter baseline**:
  * Classic — [Underscores](https://underscores.me/) (`_s`), the standard
    minimal starter theme, unless the user names a different starter.
  * Block — WP-CLI's `wp scaffold block-theme` (bundled with recent WP-CLI),
    or a specific existing block theme to use as a baseline if named.
* Whether a build pipeline is wanted for JS/CSS (e.g. `@wordpress/scripts`)
  — common for block themes, optional for classic themes.

## Scaffolding

Prefer WP-CLI when it's installed (`wp --info` to check):

```text
wp scaffold theme <slug> --activate=false
```

for a classic theme, or

```text
wp scaffold block-theme <slug>
```

for a block theme.

If WP-CLI is not installed and the user wants a classic theme, fall back to
downloading the Underscores starter (it's a static generator/zip download,
not an installable CLI) and renaming it in place — report clearly that this
step required network access and that WP-CLI would normally be the preferred
path. Do not hand-build a theme's PHP template hierarchy from scratch when
Underscores or WP-CLI can generate it.

Underscores has no stable download-by-URL endpoint — generate it by POSTing
the same form fields its generator page uses, to `https://underscores.me/`:

```text
curl -s -o theme.zip \
  -F "underscoresme_generate=1" \
  -F "underscoresme_name=<Theme Name>" \
  -F "underscoresme_slug=<slug>" \
  -F "underscoresme_author=<author>" \
  -F "underscoresme_author_uri=<author URI, may be blank>" \
  -F "underscoresme_description=<description>" \
  -F "underscoresme_generate_submit=Generate" \
  https://underscores.me/
```

The response is a zip whose single top-level entry is `<slug>/` — unzip it
and move that directory's *contents* up into the theme root; don't leave the
generated files nested inside an extra `<slug>/` subdirectory.

If `curl`/network access is also unavailable, hand-build the minimal classic
file set instead: `style.css` (with the required header block below),
`index.php`, and `functions.php`. Report clearly that this is a degraded
fallback, not the preferred Underscores or WP-CLI path.

Every generated theme, regardless of path, must have a valid `style.css`
header block (`Theme Name`, `Theme URI`, `Author`, `Description`, `Version`,
`Text Domain` at minimum) — WordPress requires this to recognize the theme.

## `.gitignore` content

```gitignore
node_modules/
vendor/
.DS_Store
*.log
```

Add `/build/` or `/dist/` if a JS/CSS build pipeline (e.g.
`@wordpress/scripts`) was set up.

## `CLAUDE.md` guidance

Cover, as relevant:

* **Project purpose** — what the theme/site is for.
* **Theme type** — classic vs. block, and the starter baseline used.
* **Structure** — template hierarchy (classic) or `theme.json` + block
  templates (block theme); where custom PHP logic (`functions.php`,
  `inc/`) lives.
* **WordPress conventions** — escape output (`esc_html`, `esc_attr`,
  `esc_url`) before printing; enqueue scripts/styles via
  `wp_enqueue_script`/`wp_enqueue_style` rather than inlining; use the
  theme's text domain consistently for i18n (`__()`, `_e()`).
* **Build commands**, if a JS/CSS pipeline exists — e.g. `npm run build`,
  `npm run start` for `@wordpress/scripts`.
* **WordPress-specific security** — never trust `$_GET`/`$_POST`/`$_REQUEST`
  without sanitizing; use nonces for form submissions (the general
  never-hardcode-credentials baseline comes from
  `skills/references/engineering-values.md`).
* **Dependencies** — minimize plugins/libraries the theme assumes exist;
  document any hard plugin dependency explicitly (see
  `skills/references/engineering-values.md` for the general
  before-adding-a-package checklist).
* **Design-first workflow** — see `skills/references/design-docs.md`.

Git and code-documentation guidance also come from
`skills/references/engineering-values.md`, loaded and adapted regardless of
family — no need to restate them here.

## Validation

If `php` is installed, syntax-check every PHP file:

```text
find . -name "*.php" -print0 | xargs -0 -n1 php -l
```

(On Windows, iterate the files with PowerShell and run `php -l` on each
instead of relying on `xargs`.)

**If `php` is not installed** (common outside a WordPress development
environment), fall back to a degraded structural check instead of skipping
validation entirely:

* Confirm `style.css`, `index.php`, and `functions.php` all exist and are
  non-empty.
* Confirm `style.css` contains every required header field (`Theme Name:`,
  `Theme URI:`, `Author:`, `Description:`, `Version:`, `Text Domain:`).

Report that PHP syntax checking was skipped and why, so the gap is visible
rather than silently treated as a full pass.

If a JS/CSS build pipeline was set up, also run its build command (e.g.
`npm run build`). If a local WordPress install with WP-CLI is available,
`wp theme activate <slug> --dry-run` (or an equivalent check) can confirm the
theme is recognized as valid — treat this as a bonus check, not a hard
requirement, since a scaffolded theme repository won't always have a full WP
install alongside it.

Treat PHP syntax errors (or, in the degraded path, missing/empty required
files) or a missing/invalid `style.css` header as blocking.
