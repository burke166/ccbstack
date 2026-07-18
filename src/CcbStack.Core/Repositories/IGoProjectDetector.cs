using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

public interface IGoProjectDetector
{
    Task<GoInfo> DetectAsync(string repositoryRoot, IReadOnlyList<string> relativeFilePaths, CancellationToken cancellationToken);
}
