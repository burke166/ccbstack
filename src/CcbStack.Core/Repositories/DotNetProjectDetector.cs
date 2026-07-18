using System.Xml;
using System.Xml.Linq;
using CcbStack.Core.IO;
using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

/// <summary>
/// The default <see cref="IDotNetProjectDetector"/>. Solution/project discovery is a plain
/// file-extension scan; project classification does a light, tolerant XML read of each
/// project file — never a full MSBuild evaluation, per CLAUDE.md — and never lets a malformed
/// project file fail the whole scan.
/// </summary>
public sealed class DotNetProjectDetector : IDotNetProjectDetector
{
    private static readonly string[] ProjectExtensions = [".csproj", ".fsproj", ".vbproj"];
    private static readonly string[] CliPackageIds =
        ["Spectre.Console.Cli", "System.CommandLine", "CommandLineParser", "McMaster.Extensions.CommandLineUtils"];
    private static readonly string[] TestPackageIds = ["Microsoft.NET.Test.Sdk", "xunit", "nunit", "MSTest"];
    private static readonly string[] BlazorPackageIds = ["Microsoft.AspNetCore.Components"];

    private readonly IFileSystem _fileSystem;

    public DotNetProjectDetector(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<DotNetInfo> DetectAsync(string repositoryRoot, IReadOnlyList<string> relativeFilePaths, CancellationToken cancellationToken)
    {
        var solutionFiles = relativeFilePaths
            .Where(p => string.Equals(Path.GetExtension(p), ".sln", StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var projectRelativePaths = relativeFilePaths
            .Where(p => ProjectExtensions.Contains(Path.GetExtension(p), StringComparer.OrdinalIgnoreCase))
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var projects = new List<DotNetProjectInfo>(projectRelativePaths.Count);
        foreach (var relativePath in projectRelativePaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            projects.Add(await ParseProjectAsync(repositoryRoot, relativePath, relativeFilePaths, cancellationToken).ConfigureAwait(false));
        }

        var hasGlobalJson = HasRootFile(relativeFilePaths, "global.json");
        var hasDirectoryBuildProps = HasRootFile(relativeFilePaths, "Directory.Build.props");
        var hasDirectoryPackagesProps = HasRootFile(relativeFilePaths, "Directory.Packages.props");

        return new DotNetInfo(solutionFiles, projects, hasGlobalJson, hasDirectoryBuildProps, hasDirectoryPackagesProps);
    }

    private async Task<DotNetProjectInfo> ParseProjectAsync(
        string repositoryRoot, string relativePath, IReadOnlyList<string> relativeFilePaths, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(repositoryRoot, relativePath);

        if (!_fileSystem.FileExists(fullPath))
        {
            return new DotNetProjectInfo(relativePath, IsSdkStyle: false, TargetFrameworks: [], DotNetProjectKind.Unknown);
        }

        try
        {
            var xml = await _fileSystem.ReadAllTextAsync(fullPath, cancellationToken).ConfigureAwait(false);
            var project = XDocument.Parse(xml).Root;

            if (project is null)
            {
                return new DotNetProjectInfo(relativePath, IsSdkStyle: false, TargetFrameworks: [], DotNetProjectKind.Unknown);
            }

            var sdkValue = project.Attribute("Sdk")?.Value ?? string.Empty;
            var isSdkStyle = sdkValue.Length > 0;
            var targetFrameworks = ExtractTargetFrameworks(project);
            var kind = ClassifyKind(project, sdkValue, relativePath, relativeFilePaths);

            return new DotNetProjectInfo(relativePath, isSdkStyle, targetFrameworks, kind);
        }
        catch (XmlException)
        {
            return new DotNetProjectInfo(relativePath, IsSdkStyle: false, TargetFrameworks: [], DotNetProjectKind.Unknown);
        }
        catch (IOException)
        {
            return new DotNetProjectInfo(relativePath, IsSdkStyle: false, TargetFrameworks: [], DotNetProjectKind.Unknown);
        }
    }

    private static IReadOnlyList<string> ExtractTargetFrameworks(XElement project)
    {
        var multi = project.Descendants("TargetFrameworks").Select(e => e.Value).FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(multi))
        {
            return multi.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        var single = project.Descendants("TargetFramework").Select(e => e.Value).FirstOrDefault();
        return string.IsNullOrWhiteSpace(single) ? [] : [single.Trim()];
    }

    private static DotNetProjectKind ClassifyKind(
        XElement project, string sdkValue, string relativePath, IReadOnlyList<string> relativeFilePaths)
    {
        var packageIds = project.Descendants("PackageReference")
            .Select(e => e.Attribute("Include")?.Value ?? string.Empty)
            .ToList();

        if (IsTrue(project, "IsTestProject") || packageIds.Any(id => TestPackageIds.Any(testId => id.Contains(testId, StringComparison.OrdinalIgnoreCase))))
        {
            return DotNetProjectKind.Test;
        }

        if (IsTrue(project, "UseWPF") || IsTrue(project, "UseWindowsForms"))
        {
            return DotNetProjectKind.Desktop;
        }

        if (sdkValue.Contains("Sdk.Web", StringComparison.OrdinalIgnoreCase))
        {
            if (packageIds.Any(id => BlazorPackageIds.Any(blazorId => id.Contains(blazorId, StringComparison.OrdinalIgnoreCase))))
            {
                return DotNetProjectKind.Blazor;
            }

            return HasControllerFile(relativePath, relativeFilePaths)
                ? DotNetProjectKind.AspNetCoreWebApp
                : DotNetProjectKind.MinimalApi;
        }

        if (sdkValue.Contains("Sdk.Worker", StringComparison.OrdinalIgnoreCase))
        {
            return DotNetProjectKind.Worker;
        }

        var outputType = project.Descendants("OutputType").Select(e => e.Value).FirstOrDefault();
        var isExecutable = string.Equals(outputType, "Exe", StringComparison.OrdinalIgnoreCase)
            || string.Equals(outputType, "WinExe", StringComparison.OrdinalIgnoreCase);

        if (!isExecutable)
        {
            return DotNetProjectKind.Library;
        }

        return packageIds.Any(id => CliPackageIds.Any(cliId => id.Contains(cliId, StringComparison.OrdinalIgnoreCase)))
            ? DotNetProjectKind.Cli
            : DotNetProjectKind.Console;
    }

    private static bool HasControllerFile(string projectRelativePath, IReadOnlyList<string> relativeFilePaths)
    {
        var projectDirectory = Path.GetDirectoryName(projectRelativePath) ?? string.Empty;
        return relativeFilePaths.Any(p =>
            p.EndsWith("Controller.cs", StringComparison.OrdinalIgnoreCase) &&
            (projectDirectory.Length == 0 || p.StartsWith(projectDirectory, StringComparison.OrdinalIgnoreCase)));
    }

    private static bool IsTrue(XElement project, string elementName)
    {
        var value = project.Descendants(elementName).Select(e => e.Value).FirstOrDefault();
        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasRootFile(IReadOnlyList<string> relativeFilePaths, string fileName)
    {
        return relativeFilePaths.Any(p =>
            string.Equals(Path.GetFileName(p), fileName, StringComparison.OrdinalIgnoreCase) &&
            !p.Contains(Path.DirectorySeparatorChar) && !p.Contains(Path.AltDirectorySeparatorChar));
    }
}
