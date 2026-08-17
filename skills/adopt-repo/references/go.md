# Go — adopt-repo reference

## Detection signals

* `go.mod` at the repository root (or, for a multi-module repo, at one or
  more subdirectory roots — note this rather than assuming a single module).
* `cmd/<name>/main.go` layout → application, possibly with multiple binaries.
* A root-level package with no `main` → library.
* `go.sum` presence indicates the module already has dependencies.

## `.gitignore` content

If missing, use:

```gitignore
/bin/
/dist/
*.exe
*.test
*.out
.env
```

Only add `vendor/` if the repository doesn't already vendor dependencies
(check whether a `vendor/` directory is already tracked before assuming it
should be ignored). If a `.gitignore` already exists, only add entries from
this list that are missing.

## `CLAUDE.md` guidance

Document what's actually there:

* **Project purpose** — from the README and what the module/commands
  actually do.
* **Module path** — read from `go.mod`, don't invent one.
* **Architecture** — real package layout; for applications, the actual
  `cmd/` structure and where shared logic lives, from inspection, not the
  idealized layout `init-repo` would scaffold fresh.
* **Existing conventions** — error-handling style, use of
  `context.Context`, testing patterns already in use — describe what's
  there.
* **Build/test/lint commands** — the real commands, including any
  Makefile/script wrappers already present, in addition to or instead of
  plain `go build`/`go test`/`go vet` if the repository has its own.
* **Dependencies** — note whether the module is intentionally
  dependency-light or already pulls in a significant third-party surface.
* **Design-first workflow** — add this section if the repository doesn't
  already describe an equivalent process.

## Verification

```text
go build ./...
```

If tests already exist, also run:

```text
go test ./...
```

Skip either command and report why if the module requires network access to
a private module proxy or credentials to build.
