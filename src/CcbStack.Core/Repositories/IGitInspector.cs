using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

/// <summary>Inspects a repository root for git facts. Kept behind an interface for testability.</summary>
public interface IGitInspector
{
    Task<GitInfo> InspectAsync(string repositoryRoot, CancellationToken cancellationToken);
}
