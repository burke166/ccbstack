# Go — init-repo reference

## Questions to ask

Ask, do not guess:

* **Module path** — e.g. `github.com/<user>/<repo>`. Needed for `go mod init`
  even if the module is never published.
* **Library or application?**
  * *Library* — package code lives at the module root (or in named packages
    under it); no `main` package.
  * *Application* — one or more commands under `cmd/<name>/main.go`, following
    the common Go project layout.
* If application, whether it needs more than one binary (multiple `cmd/`
  subdirectories) or just one.

## Scaffolding

```text
go mod init <module-path>
```

Then create the minimal source layout based on the answers above:

* **Library**: a root-level `.go` file (or a small package) with the module's
  primary type/function, named for what it does — not a placeholder
  `main.go`.
* **Application**: `cmd/<name>/main.go` containing a minimal `package main`
  with `func main()`.

If any dependencies are known up front, add them with `go get` and then run
`go build ./...` once to prime `go.sum` before the validation step.

Do not hand-roll `go.mod`/`go.sum` — always generate them via the `go` tool.

## `.gitignore` content

```gitignore
/bin/
/dist/
*.exe
*.test
*.out
.env
```

Add `vendor/` only if the project will *not* vendor dependencies (i.e. relies
on the module cache instead). If the user wants vendored dependencies, omit
that line so `vendor/` is tracked.

## `CLAUDE.md` guidance

Cover, as relevant:

* **Project purpose** — what the module/application does.
* **Design philosophy** — priority order (typically Simplicity, Correctness,
  Maintainability, Performance), idiomatic Go over cleverness.
* **Architecture** — package layout; for applications, the `cmd/` structure
  and where shared logic lives.
* **Go conventions** — `gofmt`-formatted code, explicit error handling
  (returned errors, not panics, except for truly unrecoverable states),
  `context.Context` propagation for cancellation/timeouts, small interfaces
  defined at the point of use.
* **Build/test/lint commands** — `go build ./...`, `go test ./...`,
  `go vet ./...`.
* **Testing expectations** — table-driven tests, testing observable behavior.
* **Dependencies** — prefer the standard library when sufficient (see
  `skills/references/engineering-values.md` for the general
  before-adding-a-package checklist).
* **Design-first workflow** — see `skills/references/design-docs.md`.

Security, Git, and code-documentation guidance come from
`skills/references/engineering-values.md`, loaded and adapted regardless of
family — no need to restate them here.

## Validation

Run:

```text
go build ./...
```

If the module has any tests already (unlikely immediately after scaffolding,
but possible if the user asked for an initial test), also run `go test ./...`.
Treat any failure as blocking.
