namespace CcbStack.Core.Repositories.Model;

/// <summary>
/// The reusable repository intelligence model produced by <c>IRepositoryAnalyzer</c>.
/// Every ccbstack command that needs to reason about the repository (<c>repo inspect</c>,
/// <c>doctor</c>, and future commands like <c>/spec</c>, <c>/review</c>, <c>/plan</c>) should
/// consume this rather than touching the filesystem or shelling out to git directly.
/// </summary>
/// <param name="RootPath">The discovered repository root (a <c>.git</c>/<c>.ccbstack</c> ancestor, or the starting directory if none was found).</param>
/// <param name="SolutionFiles">Convenience alias for <see cref="DotNet"/>'s solution files.</param>
/// <param name="ProjectFiles">Convenience alias for <see cref="DotNet"/>'s project file paths.</param>
public sealed record RepositoryInfo(
    string RootPath,
    GitInfo Git,
    IReadOnlyList<RepositoryLanguage> Languages,
    DotNetInfo DotNet,
    GoInfo Go,
    PowerShellInfo PowerShell,
    IReadOnlyList<ApplicationClassification> Applications,
    IReadOnlyList<string> SolutionFiles,
    IReadOnlyList<string> ProjectFiles);
