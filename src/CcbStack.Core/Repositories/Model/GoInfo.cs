namespace CcbStack.Core.Repositories.Model;

/// <summary>Go module evidence gathered from the repository.</summary>
public sealed record GoInfo(
    bool HasGoMod,
    string? ModuleName,
    string? GoVersion,
    bool HasCmdDirectory,
    bool HasInternalDirectory,
    bool HasPkgDirectory)
{
    public static GoInfo NotDetected { get; } = new(false, null, null, false, false, false);
}
