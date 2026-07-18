namespace CcbStack.Core.IO;

/// <summary>
/// Enumerates the files under a repository root that repository detectors should consider,
/// pruning common generated/vendor directories rather than filtering them out after a full
/// walk. Shared by every detector (language, .NET, Go, PowerShell, ...) so the walk happens
/// once per <c>repo inspect</c>/<c>doctor</c> run.
/// </summary>
public interface IRepositoryFileScanner
{
    /// <summary>
    /// Returns every file under <paramref name="rootPath"/>, as paths relative to it, using
    /// the platform directory separator. Returns an empty list if the root does not exist.
    /// </summary>
    IReadOnlyList<string> EnumerateFiles(string rootPath, CancellationToken cancellationToken);
}
