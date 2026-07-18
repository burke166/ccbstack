namespace CcbStack.Core.Repositories.Model;

/// <summary>.NET solution/project evidence gathered from the repository.</summary>
/// <param name="SolutionFiles">Solution file paths, relative to the repository root.</param>
/// <param name="Projects">Every detected project file and its inferred kind.</param>
public sealed record DotNetInfo(
    IReadOnlyList<string> SolutionFiles,
    IReadOnlyList<DotNetProjectInfo> Projects,
    bool HasGlobalJson,
    bool HasDirectoryBuildProps,
    bool HasDirectoryPackagesProps)
{
    public static DotNetInfo NotDetected { get; } = new([], [], false, false, false);

    public bool IsDetected => SolutionFiles.Count > 0 || Projects.Count > 0;

    public IReadOnlyList<string> TargetFrameworks =>
        Projects.SelectMany(p => p.TargetFrameworks).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
}
