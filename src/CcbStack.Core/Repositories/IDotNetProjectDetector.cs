using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

public interface IDotNetProjectDetector
{
    Task<DotNetInfo> DetectAsync(string repositoryRoot, IReadOnlyList<string> relativeFilePaths, CancellationToken cancellationToken);
}
