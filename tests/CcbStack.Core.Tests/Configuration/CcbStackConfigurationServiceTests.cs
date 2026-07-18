using CcbStack.Core.Configuration;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Configuration;

public class CcbStackConfigurationServiceTests
{
    private static readonly CommandLineConfigurationInput NoCommandLineInput = new(null, null);
    private readonly FakeRuntimeEnvironment _runtimeEnvironment = new() { CurrentDirectory = @"C:\work" };

    private static CcbStackConfigurationValues FullValues(string model, string skillsDirectory, string format)
    {
        return new CcbStackConfigurationValues
        {
            DefaultModel = OptionalValue<string>.Of(model),
            SkillsDirectory = OptionalValue<string>.Of(skillsDirectory),
            OutputFormat = OptionalValue<string>.Of(format),
        };
    }

    [Fact]
    public async Task LoadAsync_HigherLayerOverridesLowerLayer()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));
        var commandLine = FakeConfigurationProvider.Static(
            "command-line", ConfigurationLayer.CommandLine,
            new CcbStackConfigurationValues { DefaultModel = OptionalValue<string>.Of("opus") });

        var service = CreateService([defaults, commandLine]);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        result.Configuration!.DefaultModel.Should().Be("opus");
    }

    [Fact]
    public async Task LoadAsync_PrecedenceIsIndependentOfRegistrationOrder()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));
        var commandLine = FakeConfigurationProvider.Static(
            "command-line", ConfigurationLayer.CommandLine,
            new CcbStackConfigurationValues { DefaultModel = OptionalValue<string>.Of("opus") });

        // CommandLine (highest layer) registered BEFORE Defaults (lowest layer).
        var service = CreateService([commandLine, defaults]);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        result.Configuration!.DefaultModel.Should().Be("opus");
        result.Configuration.SkillsDirectory.Should().Be(@"C:\skills");
    }

    [Fact]
    public async Task LoadAsync_DoesNotErase_WhenHigherLayerLeavesKeyUnset()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));
        var user = FakeConfigurationProvider.Static(
            "user", ConfigurationLayer.User, new CcbStackConfigurationValues());

        var service = CreateService([defaults, user]);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        result.Configuration!.DefaultModel.Should().Be("sonnet");
        result.Configuration.SkillsDirectory.Should().Be(@"C:\skills");
        result.Configuration.Output.Format.Should().Be("text");
    }

    [Fact]
    public async Task LoadAsync_PreservesExplicitEmptyStringOverride()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));
        var project = FakeConfigurationProvider.Static(
            "project", ConfigurationLayer.Project,
            new CcbStackConfigurationValues { DefaultModel = OptionalValue<string>.Of(string.Empty) });

        var service = CreateService([defaults, project]);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        // The empty string was explicitly supplied, so it wins over the default -- even
        // though the validator will separately flag it as invalid.
        result.Origins[CcbStackConfigurationKeys.DefaultModel].ProviderName.Should().Be("project");
    }

    [Fact]
    public async Task LoadAsync_SameLayerDuplicateProviders_LaterRegisteredWinsDeterministically()
    {
        var first = FakeConfigurationProvider.Static(
            "user-a", ConfigurationLayer.User,
            new CcbStackConfigurationValues { DefaultModel = OptionalValue<string>.Of("from-a") });
        var second = FakeConfigurationProvider.Static(
            "user-b", ConfigurationLayer.User,
            new CcbStackConfigurationValues { DefaultModel = OptionalValue<string>.Of("from-b") });
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));

        var service = CreateService([defaults, first, second]);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        result.Configuration!.DefaultModel.Should().Be("from-b");
        result.Origins[CcbStackConfigurationKeys.DefaultModel].ProviderName.Should().Be("user-b");
    }

    [Fact]
    public async Task LoadAsync_ThrowsOperationCanceled_WhenTokenAlreadyCanceled()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));
        var service = CreateService([defaults]);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var act = async () => await service.LoadAsync(NoCommandLineInput, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task LoadAsync_ConvertsProviderException_ToErrorDiagnostic_AndContinuesOtherProviders()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));
        var broken = FakeConfigurationProvider.Throwing(ConfigurationLayer.User, new InvalidOperationException("boom"));

        var service = CreateService([defaults, broken]);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        result.Diagnostics.Should().Contain(d =>
            d.Severity == ConfigurationDiagnosticSeverity.Error && d.Message.Contains("boom"));
        result.Configuration.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task LoadAsync_ReturnsNullConfiguration_WhenRequiredValueIsMissing()
    {
        var incomplete = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults,
            new CcbStackConfigurationValues { DefaultModel = OptionalValue<string>.Of("sonnet") });

        var service = CreateService([incomplete]);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        result.Configuration.Should().BeNull();
        result.IsSuccess.Should().BeFalse();
        result.Diagnostics.Should().Contain(d =>
            d.Severity == ConfigurationDiagnosticSeverity.Error &&
            d.ConfigurationKey == CcbStackConfigurationKeys.SkillsDirectory);
    }

    [Fact]
    public async Task LoadAsync_InvokesValidator_WhenConfigurationMaterializes()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));
        var validator = new FakeConfigurationValidator();
        var service = CreateService([defaults], validator);

        await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        validator.LastValidated.Should().NotBeNull();
        validator.LastValidated!.DefaultModel.Should().Be("sonnet");
    }

    [Fact]
    public async Task LoadAsync_DoesNotInvokeValidator_WhenConfigurationFailsToMaterialize()
    {
        var incomplete = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, new CcbStackConfigurationValues());
        var validator = new FakeConfigurationValidator();
        var service = CreateService([incomplete], validator);

        await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        validator.LastValidated.Should().BeNull();
    }

    [Fact]
    public async Task LoadAsync_KeepsConfigurationNonNull_WhenValidatorReportsErrors_ButIsSuccessIsFalse()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));
        var validator = new FakeConfigurationValidator(
            new ConfigurationDiagnostic("CFG999", ConfigurationDiagnosticSeverity.Error, "simulated validation failure"));
        var service = CreateService([defaults], validator);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        result.Configuration.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task LoadAsync_OriginMetadata_IdentifiesWinningSource()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"));
        var commandLine = FakeConfigurationProvider.Static(
            "command-line", ConfigurationLayer.CommandLine,
            new CcbStackConfigurationValues { DefaultModel = OptionalValue<string>.Of("opus") });

        var service = CreateService([defaults, commandLine]);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        result.Origins[CcbStackConfigurationKeys.DefaultModel].ProviderName.Should().Be("command-line");
        result.Origins[CcbStackConfigurationKeys.DefaultModel].Layer.Should().Be(ConfigurationLayer.CommandLine);
        result.Origins[CcbStackConfigurationKeys.SkillsDirectory].ProviderName.Should().Be("defaults");
    }

    [Fact]
    public async Task LoadAsync_AggregatesDiagnosticsFromAllProviders()
    {
        var defaults = FakeConfigurationProvider.Static(
            "defaults", ConfigurationLayer.Defaults, FullValues("sonnet", @"C:\skills", "text"),
            new ConfigurationDiagnostic("CFG_A", ConfigurationDiagnosticSeverity.Warning, "warning from defaults"));
        var user = FakeConfigurationProvider.Static(
            "user", ConfigurationLayer.User, new CcbStackConfigurationValues(),
            new ConfigurationDiagnostic("CFG_B", ConfigurationDiagnosticSeverity.Warning, "warning from user"));

        var service = CreateService([defaults, user]);

        var result = await service.LoadAsync(NoCommandLineInput, CancellationToken.None);

        result.Diagnostics.Should().Contain(d => d.Code == "CFG_A");
        result.Diagnostics.Should().Contain(d => d.Code == "CFG_B");
    }

    private CcbStackConfigurationService CreateService(
        IEnumerable<ICcbStackConfigurationProvider> providers, ICcbStackConfigurationValidator? validator = null)
    {
        return new CcbStackConfigurationService(providers, validator ?? new FakeConfigurationValidator(), _runtimeEnvironment);
    }
}
