using CcbStack.Core.Repositories;
using CcbStack.Core.Repositories.Model;

namespace CcbStack.Cli.Tests.TestSupport;

public sealed class FakeRepositoryAnalyzer : IRepositoryAnalyzer
{
    private readonly Func<string, RepositoryInfo> _analyze;

    public string? LastStartingDirectory { get; private set; }

    private FakeRepositoryAnalyzer(Func<string, RepositoryInfo> analyze)
    {
        _analyze = analyze;
    }

    public static FakeRepositoryAnalyzer Returning(RepositoryInfo result)
    {
        return new FakeRepositoryAnalyzer(_ => result);
    }

    public static FakeRepositoryAnalyzer Throwing(Exception exception)
    {
        return new FakeRepositoryAnalyzer(_ => throw exception);
    }

    public static RepositoryInfo DesktopDotNetResult(string rootPath = @"C:\repo")
    {
        var dotNet = new DotNetInfo(
            [@"App.sln"],
            [new DotNetProjectInfo(@"src\App\App.csproj", true, ["net10.0-windows"], DotNetProjectKind.Desktop)],
            false, false, false);
        var git = new GitInfo(true, "main", false, false, null, null, "abc123", 0, 0, 0);

        return new RepositoryInfo(
            rootPath,
            git,
            [RepositoryLanguage.CSharp],
            dotNet,
            GoInfo.NotDetected,
            PowerShellInfo.NotDetected,
            [ApplicationClassification.DesktopApp],
            dotNet.SolutionFiles,
            [dotNet.Projects[0].Path]);
    }

    public Task<RepositoryInfo> AnalyzeAsync(string startingDirectory, CancellationToken cancellationToken)
    {
        LastStartingDirectory = startingDirectory;
        return Task.FromResult(_analyze(startingDirectory));
    }
}
