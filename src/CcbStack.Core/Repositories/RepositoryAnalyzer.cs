using CcbStack.Core.IO;
using CcbStack.Core.Repositories.Classification;
using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Repositories;

/// <summary>
/// The default <see cref="IRepositoryAnalyzer"/>: locates the repository root, scans it once,
/// and fans the resulting file list out to each detector. Detectors are independent and never
/// call each other directly — this is the only class that composes them into
/// <see cref="RepositoryInfo"/>.
/// </summary>
public sealed class RepositoryAnalyzer : IRepositoryAnalyzer
{
    private readonly IProjectRootLocator _projectRootLocator;
    private readonly IRepositoryFileScanner _fileScanner;
    private readonly IGitInspector _gitInspector;
    private readonly IDotNetProjectDetector _dotNetProjectDetector;
    private readonly IGoProjectDetector _goProjectDetector;
    private readonly IPowerShellProjectDetector _powerShellProjectDetector;
    private readonly ILanguageDetector _languageDetector;
    private readonly IApplicationClassifier _applicationClassifier;

    public RepositoryAnalyzer(
        IProjectRootLocator projectRootLocator,
        IRepositoryFileScanner fileScanner,
        IGitInspector gitInspector,
        IDotNetProjectDetector dotNetProjectDetector,
        IGoProjectDetector goProjectDetector,
        IPowerShellProjectDetector powerShellProjectDetector,
        ILanguageDetector languageDetector,
        IApplicationClassifier applicationClassifier)
    {
        _projectRootLocator = projectRootLocator;
        _fileScanner = fileScanner;
        _gitInspector = gitInspector;
        _dotNetProjectDetector = dotNetProjectDetector;
        _goProjectDetector = goProjectDetector;
        _powerShellProjectDetector = powerShellProjectDetector;
        _languageDetector = languageDetector;
        _applicationClassifier = applicationClassifier;
    }

    public async Task<RepositoryInfo> AnalyzeAsync(string startingDirectory, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startingDirectory);

        var startingInfo = new DirectoryInfo(startingDirectory);
        var root = _projectRootLocator.FindProjectRoot(startingInfo) ?? startingInfo;
        var rootPath = root.FullName;

        var relativeFilePaths = _fileScanner.EnumerateFiles(rootPath, cancellationToken);

        var git = await _gitInspector.InspectAsync(rootPath, cancellationToken).ConfigureAwait(false);
        var dotNet = await _dotNetProjectDetector.DetectAsync(rootPath, relativeFilePaths, cancellationToken).ConfigureAwait(false);
        var go = await _goProjectDetector.DetectAsync(rootPath, relativeFilePaths, cancellationToken).ConfigureAwait(false);
        var powerShell = _powerShellProjectDetector.Detect(relativeFilePaths);
        var languages = _languageDetector.Detect(relativeFilePaths);

        var evidence = new RepositoryEvidence(languages, dotNet, go, powerShell);
        var applications = _applicationClassifier.Classify(evidence);

        return new RepositoryInfo(
            rootPath,
            git,
            languages,
            dotNet,
            go,
            powerShell,
            applications,
            dotNet.SolutionFiles,
            dotNet.Projects.Select(p => p.Path).ToList());
    }
}
