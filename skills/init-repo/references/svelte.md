# Svelte — init-repo reference

## Questions to ask

Ask, do not guess:

* **SvelteKit or plain Vite + Svelte?** SvelteKit if the project needs
  routing, server-side rendering, or is a full application; plain Vite +
  Svelte if it's a component library or a small client-only app.
* **TypeScript or JavaScript?**
* Whether to include commonly-paired tooling: ESLint, Prettier, Vitest
  (unit tests), Playwright (e2e tests). Accept the official scaffolder's
  defaults unless the user has a preference — do not silently omit or add
  tooling the scaffolder would normally offer.

## Scaffolding

**SvelteKit** (via the official `sv` CLI):

```text
npx sv create <name> --template minimal --types ts
```

Adjust `--template` and `--types` flags to match the user's answers (e.g.
`--types jsdoc` or omit typing entirely for JavaScript). Pass `--no-add-ons`
or select add-ons explicitly via flags if the CLI supports non-interactive
add-on selection; otherwise accept the interactive defaults and report what
was included. Consult `npx sv create --help` for the current flag set, since
the official CLI evolves — do not assume the flags above are exhaustive or
permanently stable.

**Plain Vite + Svelte**:

```text
npm create vite@latest <name> -- --template svelte
```

(or `svelte-ts` for TypeScript).

After scaffolding, run `npm install` inside the project directory before
validation.

## `.gitignore` content

The scaffolder normally generates an appropriate `.gitignore` already; verify
it includes at least:

```gitignore
node_modules/
.svelte-kit/
/build
/dist
.env
.env.*
!.env.example
.DS_Store
```

Merge in anything missing rather than replacing what the scaffolder produced.

## `CLAUDE.md` guidance

Cover, as relevant:

* **Project purpose** — what the app/library does.
* **Architecture** — SvelteKit routing conventions (`src/routes/`) or
  component library structure; where shared state lives.
* **Language** — TypeScript vs. JavaScript, and how strictly types are
  enforced.
* **Build/dev/test commands** — `npm run dev`, `npm run build`,
  `npm run check` (svelte-check, if present), `npm run test` (if Vitest was
  included).
* **Styling conventions** — component-scoped styles vs. a global stylesheet
  or utility framework, if one was chosen.
* **Testing expectations** — what Vitest/Playwright (if included) are used
  for.
* **Design-first workflow** — see `skills/references/design-docs.md`.

Security, Dependencies, Git, and code-documentation guidance come from
`skills/references/engineering-values.md`, loaded and adapted regardless of
family — no need to restate them here.

## Validation

Run:

```text
npm install
npm run build
```

If `npm run check` exists (SvelteKit projects normally include it), run that
too. Treat any failure as blocking.
