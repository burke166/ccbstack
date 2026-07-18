using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

/// <summary>The default <see cref="ILanguageDetector"/>: a simple file-extension scan.</summary>
public sealed class LanguageDetector : ILanguageDetector
{
    private static readonly IReadOnlyDictionary<string, RepositoryLanguage> ExtensionMap =
        new Dictionary<string, RepositoryLanguage>(StringComparer.OrdinalIgnoreCase)
        {
            [".cs"] = RepositoryLanguage.CSharp,
            [".fs"] = RepositoryLanguage.FSharp,
            [".vb"] = RepositoryLanguage.VisualBasic,
            [".go"] = RepositoryLanguage.Go,
            [".ps1"] = RepositoryLanguage.PowerShell,
            [".psm1"] = RepositoryLanguage.PowerShell,
            [".psd1"] = RepositoryLanguage.PowerShell,
            [".js"] = RepositoryLanguage.JavaScript,
            [".jsx"] = RepositoryLanguage.JavaScript,
            [".ts"] = RepositoryLanguage.TypeScript,
            [".tsx"] = RepositoryLanguage.TypeScript,
        };

    public IReadOnlyList<RepositoryLanguage> Detect(IReadOnlyList<string> relativeFilePaths)
    {
        var detected = new HashSet<RepositoryLanguage>();

        foreach (var path in relativeFilePaths)
        {
            if (ExtensionMap.TryGetValue(Path.GetExtension(path), out var language))
            {
                detected.Add(language);
            }
        }

        // Enum declaration order, not discovery order, so output is stable across runs.
        return Enum.GetValues<RepositoryLanguage>().Where(detected.Contains).ToList();
    }
}
