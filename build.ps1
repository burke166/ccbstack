#Requires -Version 7.0

<#
.SYNOPSIS
    Restores, builds, tests, and packs ccbstack, placing the resulting NuGet package in a
    local feed directory.

.PARAMETER Configuration
    The build configuration to use. Defaults to Release.

.PARAMETER LocalFeed
    The directory to place the packed NuGet package in. Defaults to .\artifacts\local-feed.
    Created if it does not already exist. Safe to run repeatedly; existing package files for
    the same version are simply overwritten.

.EXAMPLE
    .\build.ps1

.EXAMPLE
    .\build.ps1 -Configuration Release -LocalFeed .\artifacts\local-feed
#>

[CmdletBinding()]
param(
    [string]$Configuration = "Release",
    [string]$LocalFeed = (Join-Path $PSScriptRoot "artifacts" "local-feed")
)

$ErrorActionPreference = "Stop"

$solutionPath = Join-Path $PSScriptRoot "CcbStack.slnx"
$cliProjectPath = Join-Path $PSScriptRoot "src" "CcbStack.Cli" "CcbStack.Cli.csproj"

function Invoke-DotNet {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    Write-Host "dotnet $($Arguments -join ' ')" -ForegroundColor Cyan
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE"
    }
}

if (-not (Test-Path -LiteralPath $LocalFeed)) {
    New-Item -ItemType Directory -Path $LocalFeed -Force | Out-Null
}

Invoke-DotNet -Arguments @("restore", $solutionPath)
Invoke-DotNet -Arguments @("build", $solutionPath, "--configuration", $Configuration, "--no-restore")
Invoke-DotNet -Arguments @("test", $solutionPath, "--configuration", $Configuration, "--no-build")
Invoke-DotNet -Arguments @("pack", $cliProjectPath, "--configuration", $Configuration, "--no-build", "--output", $LocalFeed)

$package = Get-ChildItem -LiteralPath $LocalFeed -Filter "CcbStack.Cli.*.nupkg" |
    Sort-Object -Property LastWriteTime -Descending |
    Select-Object -First 1

if (-not $package) {
    throw "Pack completed but no CcbStack.Cli.*.nupkg was found in '$LocalFeed'."
}

Write-Host ""
Write-Host "Build succeeded." -ForegroundColor Green
Write-Host "Package    : $($package.FullName)"
Write-Host "Local feed : $LocalFeed"
