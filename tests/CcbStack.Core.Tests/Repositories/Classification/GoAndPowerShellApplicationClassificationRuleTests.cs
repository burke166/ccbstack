using CcbStack.Core.Repositories.Classification;
using CcbStack.Core.Repositories.Model;
using FluentAssertions;

namespace CcbStack.Core.Tests.Repositories.Classification;

public class GoApplicationClassificationRuleTests
{
    private readonly GoApplicationClassificationRule _rule = new();

    [Fact]
    public void Classify_ReturnsGoCli_WhenCmdDirectoryExists()
    {
        var go = new GoInfo(true, "example", "1.22", HasCmdDirectory: true, false, false);
        var evidence = new RepositoryEvidence([], DotNetInfo.NotDetected, go, PowerShellInfo.NotDetected);

        var result = _rule.Classify(evidence);

        result.Should().Equal(ApplicationClassification.GoCli);
    }

    [Fact]
    public void Classify_ReturnsLibrary_WhenNoCmdDirectoryExists()
    {
        var go = new GoInfo(true, "example", "1.22", HasCmdDirectory: false, false, false);
        var evidence = new RepositoryEvidence([], DotNetInfo.NotDetected, go, PowerShellInfo.NotDetected);

        var result = _rule.Classify(evidence);

        result.Should().Equal(ApplicationClassification.Library);
    }

    [Fact]
    public void Classify_ReturnsNothing_WhenNoGoModExists()
    {
        var evidence = new RepositoryEvidence([], DotNetInfo.NotDetected, GoInfo.NotDetected, PowerShellInfo.NotDetected);

        var result = _rule.Classify(evidence);

        result.Should().BeEmpty();
    }
}

public class PowerShellApplicationClassificationRuleTests
{
    private readonly PowerShellApplicationClassificationRule _rule = new();

    [Fact]
    public void Classify_ReturnsPowerShellModule_WhenModulesExist()
    {
        var powerShell = new PowerShellInfo(0, ModuleFileCount: 1, false);
        var evidence = new RepositoryEvidence([], DotNetInfo.NotDetected, GoInfo.NotDetected, powerShell);

        var result = _rule.Classify(evidence);

        result.Should().Equal(ApplicationClassification.PowerShellModule);
    }

    [Fact]
    public void Classify_ReturnsNothing_ForScriptsWithoutModules()
    {
        var powerShell = new PowerShellInfo(ScriptFileCount: 3, ModuleFileCount: 0, false);
        var evidence = new RepositoryEvidence([], DotNetInfo.NotDetected, GoInfo.NotDetected, powerShell);

        var result = _rule.Classify(evidence);

        result.Should().BeEmpty();
    }
}
