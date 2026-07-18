namespace CcbStack.Core.Repositories.Model;

/// <summary>
/// Git repository facts. When <see cref="IsGitRepository"/> is <see langword="false"/> or the
/// <c>git</c> executable could not be resolved, every other field is left at its default —
/// missing git tooling must never fail repository analysis.
/// </summary>
public sealed record GitInfo(
    bool IsGitRepository,
    string? Branch,
    bool IsDetachedHead,
    bool IsDirty,
    string? RemoteOriginUrl,
    string? DefaultBranch,
    string? CommitHash,
    int StagedFileCount,
    int ModifiedFileCount,
    int UntrackedFileCount)
{
    public static GitInfo NotAGitRepository { get; } = new(false, null, false, false, null, null, null, 0, 0, 0);

    public static GitInfo GitUnavailable { get; } = NotAGitRepository with { IsGitRepository = true };
}
