using CcbStack.Core.Repositories.Classification;
using CcbStack.Core.Repositories.Model;
using FluentAssertions;

namespace CcbStack.Core.Tests.Repositories.Classification;

public class ApplicationClassifierTests
{
    [Fact]
    public void Classify_ReturnsUnknown_WhenNoRuleContributesAnything()
    {
        var classifier = new ApplicationClassifier([]);
        var evidence = new RepositoryEvidence([], DotNetInfo.NotDetected, GoInfo.NotDetected, PowerShellInfo.NotDetected);

        var result = classifier.Classify(evidence);

        result.Should().Equal(ApplicationClassification.Unknown);
    }

    [Fact]
    public void Classify_UnionsResultsFromEveryRegisteredRule()
    {
        var classifier = new ApplicationClassifier([new FixedRule(ApplicationClassification.Cli), new FixedRule(ApplicationClassification.Library)]);
        var evidence = new RepositoryEvidence([], DotNetInfo.NotDetected, GoInfo.NotDetected, PowerShellInfo.NotDetected);

        var result = classifier.Classify(evidence);

        result.Should().Contain([ApplicationClassification.Cli, ApplicationClassification.Library]);
    }

    [Fact]
    public void Classify_AddsMixedRepository_WhenMultipleLanguageEcosystemsAreDetected()
    {
        var classifier = new ApplicationClassifier([new DotNetApplicationClassificationRule(), new GoApplicationClassificationRule()]);
        var dotNet = new DotNetInfo([], [new DotNetProjectInfo("App.csproj", true, ["net10.0"], DotNetProjectKind.Console)], false, false, false);
        var go = new GoInfo(true, "example", "1.22", true, false, false);
        var evidence = new RepositoryEvidence([], dotNet, go, PowerShellInfo.NotDetected);

        var result = classifier.Classify(evidence);

        result.Should().Contain(ApplicationClassification.MixedRepository);
    }

    [Fact]
    public void Classify_DoesNotAddMixedRepository_ForASingleLanguageEcosystem()
    {
        var classifier = new ApplicationClassifier([new DotNetApplicationClassificationRule()]);
        var dotNet = new DotNetInfo([], [new DotNetProjectInfo("App.csproj", true, ["net10.0"], DotNetProjectKind.Console)], false, false, false);
        var evidence = new RepositoryEvidence([], dotNet, GoInfo.NotDetected, PowerShellInfo.NotDetected);

        var result = classifier.Classify(evidence);

        result.Should().NotContain(ApplicationClassification.MixedRepository);
    }

    private sealed class FixedRule : IApplicationClassificationRule
    {
        private readonly ApplicationClassification _classification;

        public FixedRule(ApplicationClassification classification)
        {
            _classification = classification;
        }

        public IReadOnlyCollection<ApplicationClassification> Classify(RepositoryEvidence evidence) => [_classification];
    }
}
