using CcbStack.Core.Configuration;
using CcbStack.Core.Configuration.Providers;
using CcbStack.Core.IO;
using CcbStack.Core.Runtime;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Configuration.Providers;

public class EnvironmentConfigurationProviderTests
{
    [Fact]
    public async Task LoadAsync_MapsDefaultModelVariable()
    {
        var provider = CreateProvider(new Dictionary<string, string?> { ["CCBSTACK_DEFAULTMODEL"] = "sonnet" });

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("sonnet"));
    }

    [Fact]
    public async Task LoadAsync_MapsSkillsDirectoryVariable()
    {
        var provider = CreateProvider(new Dictionary<string, string?> { ["CCBSTACK_SKILLSDIRECTORY"] = @"C:\skills" });

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Values.SkillsDirectory.Value.Should().Be(@"C:\skills");
    }

    [Fact]
    public async Task LoadAsync_MapsDoubleUnderscoreNestedOutputFormatVariable()
    {
        var provider = CreateProvider(new Dictionary<string, string?> { ["CCBSTACK_OUTPUT__FORMAT"] = "json" });

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Values.OutputFormat.Should().Be(OptionalValue<string>.Of("json"));
    }

    [Fact]
    public async Task LoadAsync_IgnoresVariablesWithoutCcbstackPrefix()
    {
        var provider = CreateProvider(new Dictionary<string, string?> { ["PATH"] = @"C:\tools", ["OTHER_VAR"] = "x" });

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Values.DefaultModel.IsSet.Should().BeFalse();
        source.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadAsync_UnrecognizedPrefixedVariable_ProducesWarningAndIsIgnored()
    {
        var provider = CreateProvider(new Dictionary<string, string?> { ["CCBSTACK_NOT_A_REAL_KEY"] = "x" });

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Warning);
        source.Values.DefaultModel.IsSet.Should().BeFalse();
    }

    [Fact]
    public async Task LoadAsync_MatchesVariableNamesCaseInsensitively_OnWindows()
    {
        var provider = CreateProvider(
            new Dictionary<string, string?> { ["ccbstack_defaultmodel"] = "sonnet" },
            OperatingSystemKind.Windows);

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("sonnet"));
    }

    [Fact]
    public async Task LoadAsync_DoesNotMatchWrongCaseVariable_OnNonWindows()
    {
        var provider = CreateProvider(
            new Dictionary<string, string?> { ["ccbstack_defaultmodel"] = "sonnet" },
            OperatingSystemKind.Linux);

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Values.DefaultModel.IsSet.Should().BeFalse();
        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Warning);
    }

    [Fact]
    public async Task LoadAsync_OriginMetadata_IncludesEnvironmentVariableName()
    {
        var provider = CreateProvider(new Dictionary<string, string?> { ["CCBSTACK_DEFAULTMODEL"] = "sonnet" });

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Origins[CcbStackConfigurationKeys.DefaultModel].EnvironmentVariableName.Should().Be("CCBSTACK_DEFAULTMODEL");
        source.Origins[CcbStackConfigurationKeys.DefaultModel].Layer.Should().Be(ConfigurationLayer.Environment);
    }

    [Fact]
    public async Task LoadAsync_NeverMutatesRealProcessEnvironment()
    {
        var provider = CreateProvider(new Dictionary<string, string?> { ["CCBSTACK_DEFAULTMODEL"] = "sonnet" });

        await provider.LoadAsync(Context(), CancellationToken.None);

        Environment.GetEnvironmentVariable("CCBSTACK_DEFAULTMODEL").Should().BeNull();
    }

    private static EnvironmentConfigurationProvider CreateProvider(
        Dictionary<string, string?> variables,
        OperatingSystemKind operatingSystem = OperatingSystemKind.Windows)
    {
        var reader = new FakeEnvironmentVariableReader(variables);
        var runtimeEnvironment = new FakeRuntimeEnvironment { OperatingSystem = operatingSystem };
        return new EnvironmentConfigurationProvider(reader, new ConfigurationPathExpander(), runtimeEnvironment);
    }

    private static ConfigurationProviderContext Context()
    {
        return new ConfigurationProviderContext(@"C:\work", new CommandLineConfigurationInput(null, null));
    }
}
