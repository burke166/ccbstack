using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

public interface ILanguageDetector
{
    IReadOnlyList<RepositoryLanguage> Detect(IReadOnlyList<string> relativeFilePaths);
}
