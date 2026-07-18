using CcbStack.Core.IO;
using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

/// <summary>
/// The default <see cref="IGoProjectDetector"/>: looks for a root-level <c>go.mod</c> (falling
/// back to the shallowest one found, for repositories that nest their Go module), does a
/// minimal line-based parse for the module name and Go version, and checks for the
/// conventional <c>cmd/</c>, <c>internal/</c>, and <c>pkg/</c> top-level directories.
/// </summary>
public sealed class GoProjectDetector : IGoProjectDetector
{
    private readonly IFileSystem _fileSystem;

    public GoProjectDetector(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<GoInfo> DetectAsync(string repositoryRoot, IReadOnlyList<string> relativeFilePaths, CancellationToken cancellationToken)
    {
        var goModRelativePath = FindGoMod(relativeFilePaths);
        if (goModRelativePath is null)
        {
            return GoInfo.NotDetected;
        }

        string? moduleName = null;
        string? goVersion = null;

        try
        {
            var contents = await _fileSystem.ReadAllTextAsync(Path.Combine(repositoryRoot, goModRelativePath), cancellationToken)
                .ConfigureAwait(false);
            (moduleName, goVersion) = ParseGoMod(contents);
        }
        catch (IOException)
        {
            // A malformed or unreadable go.mod should not fail detection — go.mod's presence
            // alone is still meaningful evidence.
        }

        return new GoInfo(
            HasGoMod: true,
            ModuleName: moduleName,
            GoVersion: goVersion,
            HasCmdDirectory: relativeFilePaths.Any(p => IsUnderTopLevelDirectory(p, "cmd")),
            HasInternalDirectory: relativeFilePaths.Any(p => IsUnderTopLevelDirectory(p, "internal")),
            HasPkgDirectory: relativeFilePaths.Any(p => IsUnderTopLevelDirectory(p, "pkg")));
    }

    private static string? FindGoMod(IReadOnlyList<string> relativeFilePaths)
    {
        return relativeFilePaths
            .Where(p => string.Equals(Path.GetFileName(p), "go.mod", StringComparison.OrdinalIgnoreCase))
            .OrderBy(Depth)
            .FirstOrDefault();

        static int Depth(string path) => path.Count(c => c is '/' or '\\');
    }

    private static (string? ModuleName, string? GoVersion) ParseGoMod(string contents)
    {
        string? moduleName = null;
        string? goVersion = null;

        foreach (var rawLine in contents.Split('\n'))
        {
            var line = rawLine.Trim();

            if (line.StartsWith("module ", StringComparison.Ordinal))
            {
                moduleName = line["module ".Length..].Trim();
            }
            else if (line.StartsWith("go ", StringComparison.Ordinal))
            {
                goVersion = line["go ".Length..].Trim();
            }
        }

        return (moduleName, goVersion);
    }

    private static bool IsUnderTopLevelDirectory(string relativePath, string directoryName)
    {
        var segments = relativePath.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
        return segments.Length > 1 && string.Equals(segments[0], directoryName, StringComparison.OrdinalIgnoreCase);
    }
}
