using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

public interface IPowerShellProjectDetector
{
    PowerShellInfo Detect(IReadOnlyList<string> relativeFilePaths);
}
