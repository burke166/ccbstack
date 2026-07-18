namespace CcbStack.Core.Repositories.Model;

/// <summary>A single detected .NET project file and what could be inferred about it.</summary>
/// <param name="Path">Path relative to the repository root.</param>
public sealed record DotNetProjectInfo(
    string Path,
    bool IsSdkStyle,
    IReadOnlyList<string> TargetFrameworks,
    DotNetProjectKind Kind);
