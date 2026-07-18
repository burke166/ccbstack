# ccbstack

A Windows-first, agentic software-development framework for .NET, Go, PowerShell, and related tools.

This project is being built independently, using gstack as a design reference.

## Status

ccbstack is in its initial bootstrap stage. The only implemented command today is
`dotnet ccbstack version`. See `CLAUDE.md` for the full planned command roadmap and
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

## Installing locally as a .NET tool

ccbstack is packaged as a .NET tool (`ToolCommandName` = `dotnet-ccbstack`), which lets it be
invoked as `dotnet ccbstack <command>` once installed. Install it as a repository-local tool:

```powershell
# Only needed once per repository clone:
dotnet new tool-manifest

# --prerelease is required while ccbstack ships prerelease versions (e.g. 0.1.0-alpha.1)
dotnet tool install --local CcbStack.Cli --add-source .\artifacts\nupkg --prerelease
```

## Invoking ccbstack

```powershell
dotnet ccbstack version
dotnet ccbstack version --format json
```

## Uninstalling the local tool

```powershell
dotnet tool uninstall --local CcbStack.Cli
```
