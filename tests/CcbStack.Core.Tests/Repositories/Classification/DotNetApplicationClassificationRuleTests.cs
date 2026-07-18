using CcbStack.Core.Repositories.Classification;
using CcbStack.Core.Repositories.Model;
using FluentAssertions;

namespace CcbStack.Core.Tests.Repositories.Classification;

public class DotNetApplicationClassificationRuleTests
{
    private readonly DotNetApplicationClassificationRule _rule = new();

    [Theory]
    [InlineData(DotNetProjectKind.Desktop, ApplicationClassification.DesktopApp)]
    [InlineData(DotNetProjectKind.Console, ApplicationClassification.ConsoleTool)]
    [InlineData(DotNetProjectKind.Cli, ApplicationClassification.Cli)]
    [InlineData(DotNetProjectKind.AspNetCoreWebApp, ApplicationClassification.AspNetCoreWebApp)]
    [InlineData(DotNetProjectKind.MinimalApi, ApplicationClassification.MinimalApi)]
    [InlineData(DotNetProjectKind.Blazor, ApplicationClassification.Blazor)]
    [InlineData(DotNetProjectKind.Worker, ApplicationClassification.WorkerService)]
    [InlineData(DotNetProjectKind.Library, ApplicationClassification.Library)]
    public void Classify_MapsProjectKindToClassification(DotNetProjectKind kind, ApplicationClassification expected)
    {
        var dotNet = new DotNetInfo([], [new DotNetProjectInfo("App.csproj", true, [], kind)], false, false, false);
        var evidence = new RepositoryEvidence([], dotNet, GoInfo.NotDetected, PowerShellInfo.NotDetected);

        var result = _rule.Classify(evidence);

        result.Should().Equal(expected);
    }

    [Theory]
    [InlineData(DotNetProjectKind.Test)]
    [InlineData(DotNetProjectKind.Unknown)]
    public void Classify_ContributesNothing_ForTestOrUnknownProjects(DotNetProjectKind kind)
    {
        var dotNet = new DotNetInfo([], [new DotNetProjectInfo("App.csproj", true, [], kind)], false, false, false);
        var evidence = new RepositoryEvidence([], dotNet, GoInfo.NotDetected, PowerShellInfo.NotDetected);

        var result = _rule.Classify(evidence);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Classify_ReturnsMultipleClassifications_ForMultipleProjectKinds()
    {
        var dotNet = new DotNetInfo(
            [],
            [
                new DotNetProjectInfo("App.csproj", true, [], DotNetProjectKind.Console),
                new DotNetProjectInfo("Lib.csproj", true, [], DotNetProjectKind.Library),
            ],
            false, false, false);
        var evidence = new RepositoryEvidence([], dotNet, GoInfo.NotDetected, PowerShellInfo.NotDetected);

        var result = _rule.Classify(evidence);

        result.Should().Contain([ApplicationClassification.ConsoleTool, ApplicationClassification.Library]);
    }
}
