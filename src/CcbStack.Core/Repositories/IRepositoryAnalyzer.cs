using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

/// <summary>
/// Produces the reusable <see cref="RepositoryInfo"/> model. Every command that needs
/// repository facts should depend on this rather than touching the filesystem, git, or
/// project files directly.
/// </summary>
public interface IRepositoryAnalyzer
{
    Task<RepositoryInfo> AnalyzeAsync(string startingDirectory, CancellationToken cancellationToken);
}
