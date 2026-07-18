using CcbStack.Core.Configuration;
using FluentAssertions;

namespace CcbStack.Core.Tests.Configuration;

public class CcbStackConfigurationValidatorTests
{
    private readonly CcbStackConfigurationValidator _validator = new();

    private static CcbStackConfiguration ValidConfiguration =>
        new("sonnet", @"C:\Users\test-user\.claude\skills", new OutputConfiguration("text"));

    [Fact]
    public void Validate_ReturnsNoDiagnostics_ForValidConfiguration()
    {
        var diagnostics = _validator.Validate(ValidConfiguration);

        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ReturnsError_WhenDefaultModelIsEmpty()
    {
        var configuration = ValidConfiguration with { DefaultModel = "" };

        var diagnostics = _validator.Validate(configuration);

        diagnostics.Should().ContainSingle(d =>
            d.Severity == ConfigurationDiagnosticSeverity.Error &&
            d.ConfigurationKey == CcbStackConfigurationKeys.DefaultModel);
    }

    [Fact]
    public void Validate_ReturnsError_WhenSkillsDirectoryIsEmpty()
    {
        var configuration = ValidConfiguration with { SkillsDirectory = "   " };

        var diagnostics = _validator.Validate(configuration);

        diagnostics.Should().ContainSingle(d =>
            d.Severity == ConfigurationDiagnosticSeverity.Error &&
            d.ConfigurationKey == CcbStackConfigurationKeys.SkillsDirectory);
    }

    [Fact]
    public void Validate_ReturnsError_WhenSkillsDirectoryHasInvalidPathCharacters()
    {
        var configuration = ValidConfiguration with { SkillsDirectory = "C:\\skills\0invalid" };

        var diagnostics = _validator.Validate(configuration);

        diagnostics.Should().ContainSingle(d =>
            d.Severity == ConfigurationDiagnosticSeverity.Error &&
            d.ConfigurationKey == CcbStackConfigurationKeys.SkillsDirectory);
    }

    [Fact]
    public void Validate_ReturnsError_WhenOutputFormatIsUnsupported()
    {
        var configuration = ValidConfiguration with { Output = new OutputConfiguration("xml") };

        var diagnostics = _validator.Validate(configuration);

        diagnostics.Should().ContainSingle(d =>
            d.Severity == ConfigurationDiagnosticSeverity.Error &&
            d.ConfigurationKey == CcbStackConfigurationKeys.Output.Format);
    }

    [Theory]
    [InlineData("text")]
    [InlineData("json")]
    [InlineData("TEXT")]
    [InlineData("Json")]
    public void Validate_AcceptsSupportedOutputFormats_CaseInsensitively(string format)
    {
        var configuration = ValidConfiguration with { Output = new OutputConfiguration(format) };

        var diagnostics = _validator.Validate(configuration);

        diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void Validate_Throws_WhenConfigurationIsNull()
    {
        var act = () => _validator.Validate(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
