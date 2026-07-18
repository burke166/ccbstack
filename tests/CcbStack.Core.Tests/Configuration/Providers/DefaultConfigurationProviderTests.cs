using CcbStack.Core.Configuration;
using CcbStack.Core.Configuration.Providers;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Configuration.Providers;

public class DefaultConfigurationProviderTests
{
    private readonly FakeRuntimeEnvironment _runtimeEnvironment = new() { UserProfileDirectory = @"C:\Users\test-user" };

    [Fact]
    public void Layer_IsDefaults()
    {
        var provider = new DefaultConfigurationProvider(_runtimeEnvironment);

        provider.Layer.Should().Be(ConfigurationLayer.Defaults);
    }

    [Fact]
    public async Task LoadAsync_SuppliesAllThreeValues()
    {
        var provider = new DefaultConfigurationProvider(_runtimeEnvironment);

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Values.DefaultModel.IsSet.Should().BeTrue();
        source.Values.SkillsDirectory.IsSet.Should().BeTrue();
        source.Values.OutputFormat.IsSet.Should().BeTrue();
        source.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadAsync_ProducesOriginMetadataForEachKey()
    {
        var provider = new DefaultConfigurationProvider(_runtimeEnvironment);

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Origins.Should().ContainKey(CcbStackConfigurationKeys.DefaultModel);
        source.Origins.Should().ContainKey(CcbStackConfigurationKeys.SkillsDirectory);
        source.Origins.Should().ContainKey(CcbStackConfigurationKeys.Output.Format);
        source.Origins[CcbStackConfigurationKeys.DefaultModel].Layer.Should().Be(ConfigurationLayer.Defaults);
    }

    [Fact]
    public async Task LoadAsync_DerivesSkillsDirectoryFromInjectedUserProfile()
    {
        var provider = new DefaultConfigurationProvider(_runtimeEnvironment);

        var source = await provider.LoadAsync(Context(), CancellationToken.None);

        source.Values.SkillsDirectory.Value.Should().StartWith(_runtimeEnvironment.UserProfileDirectory);
    }

    private static ConfigurationProviderContext Context()
    {
        return new ConfigurationProviderContext(@"C:\work", new CommandLineConfigurationInput(null, null));
    }
}
