using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

/// <summary>The default <see cref="IPowerShellProjectDetector"/>: counts scripts vs. modules by extension.</summary>
public sealed class PowerShellProjectDetector : IPowerShellProjectDetector
{
    private static readonly string[] BuildScriptNames = ["build.ps1", "make.ps1"];

    public PowerShellInfo Detect(IReadOnlyList<string> relativeFilePaths)
    {
        var scriptCount = relativeFilePaths.Count(p => HasExtension(p, ".ps1"));
        var moduleCount = relativeFilePaths.Count(p => HasExtension(p, ".psm1") || HasExtension(p, ".psd1"));
        var hasBuildScripts = relativeFilePaths.Any(p => BuildScriptNames.Contains(Path.GetFileName(p), StringComparer.OrdinalIgnoreCase));

        return new PowerShellInfo(scriptCount, moduleCount, hasBuildScripts);
    }

    private static bool HasExtension(string path, string extension) =>
        string.Equals(Path.GetExtension(path), extension, StringComparison.OrdinalIgnoreCase);
}
