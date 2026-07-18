using CcbStack.Core.Repositories;
using FluentAssertions;

namespace CcbStack.Core.Tests.Repositories;

public class PowerShellProjectDetectorTests
{
    private readonly PowerShellProjectDetector _detector = new();

    [Fact]
    public void Detect_ReturnsNotDetected_WhenNoPowerShellFilesExist()
    {
        var result = _detector.Detect(["Program.cs", "README.md"]);

        result.IsDetected.Should().BeFalse();
    }

    [Fact]
    public void Detect_CountsScriptsAndModulesSeparately()
    {
        var result = _detector.Detect(["deploy.ps1", "install.ps1", "MyModule.psm1", "MyModule.psd1"]);

        result.ScriptFileCount.Should().Be(2);
        result.ModuleFileCount.Should().Be(2);
        result.HasScripts.Should().BeTrue();
        result.HasModules.Should().BeTrue();
    }

    [Theory]
    [InlineData("build.ps1")]
    [InlineData("make.ps1")]
    public void Detect_FlagsBuildScripts_ByConventionalName(string fileName)
    {
        var result = _detector.Detect([fileName]);

        result.HasBuildScripts.Should().BeTrue();
    }

    [Fact]
    public void Detect_DoesNotFlagBuildScripts_ForUnrelatedScripts()
    {
        var result = _detector.Detect(["deploy.ps1"]);

        result.HasBuildScripts.Should().BeFalse();
    }
}
