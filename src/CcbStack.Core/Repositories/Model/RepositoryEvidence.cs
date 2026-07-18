namespace CcbStack.Core.Repositories.Model;

/// <summary>
/// The subset of a repository's detected facts that application classification reasons over.
/// Deliberately excludes <see cref="GitInfo"/> (irrelevant to "what kind of app is this") so
/// classification rules can't accidentally depend on it.
/// </summary>
public sealed record RepositoryEvidence(
    IReadOnlyList<RepositoryLanguage> Languages,
    DotNetInfo DotNet,
    GoInfo Go,
    PowerShellInfo PowerShell);
