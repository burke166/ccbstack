# ccbstack

A Windows-first, agentic software-development framework for .NET, Go, PowerShell, and related tools.

This project is being built independently, using gstack as a design reference.

## Status

ccbstack is in its initial bootstrap stage. The implemented commands today are
`ccbstack version` and `ccbstack config`. See `CLAUDE.md` for the full planned command
roadmap and development conventions.

## Building the solution

```powershell
dotnet build .\CcbStack.slnx
```

## Running from source

```powershell
dotnet run --project .\src\CcbStack.Cli -- version
dotnet run --project .\src\CcbStack.Cli -- version --format json
dotnet run --project .\src\CcbStack.Cli -- config
dotnet run --project .\src\CcbStack.Cli -- config --json
```

## Running the tests

```powershell
dotnet test .\CcbStack.slnx
```

## Packing the tool locally

```powershell
dotnet pack .\src\CcbStack.Cli\CcbStack.Cli.csproj -c Release -o .\artifacts\nupkg
```

This produces `CcbStack.Cli.<version>.nupkg` in `.\artifacts\nupkg`.

### Using the build script

`build.ps1` runs restore, build, test, and pack in one step, placing the package in a local
NuGet feed directory (`.\artifacts\local-feed` by default):

```powershell
.\build.ps1
.\build.ps1 -Configuration Release -LocalFeed .\artifacts\local-feed
```

It fails immediately on the first failing step and is safe to run repeatedly.

## Installing as a global .NET tool

ccbstack is packaged as a .NET tool (`ToolCommandName` = `ccbstack`). Installing it globally
places a `ccbstack` shim on your `PATH`, so it can be invoked directly as `ccbstack <command>`
once installed:

```powershell
# --prerelease is required while ccbstack ships prerelease versions (e.g. 0.1.0-alpha.1)
dotnet tool install --global CcbStack.Cli --add-source .\artifacts\nupkg --prerelease
```

To upgrade after repacking a new version:

```powershell
dotnet tool update --global CcbStack.Cli --add-source .\artifacts\nupkg --prerelease
```

## Invoking ccbstack

```powershell
ccbstack version
ccbstack version --format json
ccbstack config
ccbstack config --json
```

### The `config` command

`ccbstack config` displays the effective configuration, merged from (lowest to highest
precedence): built-in defaults, `%USERPROFILE%\.ccbstack\config.json`,
`<project-root>\.ccbstack\config.json`, `CCBSTACK_*` environment variables, and command-line
overrides. A "Runtime Environment" section shows resolved `pwsh`/`git`/`claude` executable
paths — this is informational, not configuration.

Temporary, non-persisted overrides can be supplied on the command line (mutually exclusive):

```powershell
ccbstack config --config-json '{ "defaultModel": "sonnet" }'
ccbstack config --config-file .\temporary-config.json
```

`ccbstack config` does not yet support reading or writing individual keys (`config get`/
`config set`) — that is planned for a future milestone.

## Uninstalling the global tool

```powershell
dotnet tool uninstall --global CcbStack.Cli
```
