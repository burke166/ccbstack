# Svelte — adopt-repo reference

## Detection signals

* `package.json` with `svelte` (and usually `@sveltejs/kit`) as a
  dependency/devDependency.
* `svelte.config.js` presence.
* `src/routes/` → SvelteKit (file-based routing). `src/App.svelte` with no
  `src/routes/` → plain Vite + Svelte.
* `vite.config.js`/`vite.config.ts` referencing the Svelte plugin.
* TypeScript in use: `tsconfig.json` present and/or `.ts`/`.svelte` files
  using `<script lang="ts">`.

## `.gitignore` content

The project almost always already has an appropriate `.gitignore` from its
original scaffolder. If missing, use:

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

If a `.gitignore` already exists, only add entries from this list that are
missing.

## `CLAUDE.md` guidance

Document what's actually there:

* **Project purpose** — from the README and what the app/library does.
* **Architecture** — real SvelteKit routing structure (`src/routes/`) or
  component library layout; where shared state actually lives.
* **Language** — TypeScript or JavaScript, and how strictly types are
  actually enforced (check `tsconfig.json`'s `strict` setting rather than
  assuming).
* **Build/dev/test commands** — the real `package.json` scripts
  (`npm run dev`, `npm run build`, `npm run check`, `npm run test`, or
  whatever the repository actually defines — script names vary).
* **Styling conventions** — component-scoped styles, a global stylesheet, or
  a utility framework, based on what's actually used.
* **Testing** — what's actually configured (Vitest, Playwright, neither) and
  what it currently covers, not what a fresh scaffold would include.
* **Dependencies** — note the real dependency surface; don't prescribe
  minimalism if the project already isn't minimal.
* **Design-first workflow** — add this section if the repository doesn't
  already describe an equivalent process.

## Verification

```text
npm install
npm run build
```

Run `npm run check` too if that script exists. If `package.json` defines a
different build script name, use that instead of assuming `build`. Skip
verification and report why if install requires a private registry needing
credentials.
