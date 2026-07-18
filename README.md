# ccbstack

A Windows-first, agentic software-development framework for .NET, Go, PowerShell, and related tools.

This project is being built independently, using gstack as a design reference.

## Status

ccbstack is in its initial bootstrap stage. The only implemented command today is
`ccbstack version`. See `CLAUDE.md` for the full planned command roadmap and
development conventions.

## Building the solution

```powershell
dotnet build .\CcbStack.slnx
```

## Running from source

```powershell
dotnet run --project .\src\CcbStack.Cli -- version
dotnet run --project .\src\CcbStack.Cli -- version --format json
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
```

## Uninstalling the global tool

```powershell
dotnet tool uninstall --global CcbStack.Cli
```
