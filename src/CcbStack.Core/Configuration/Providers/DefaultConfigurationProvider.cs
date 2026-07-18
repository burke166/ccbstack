using CcbStack.Core.Runtime;

namespace CcbStack.Core.Configuration.Providers;

/// <summary>
/// The lowest-precedence provider: supplies ccbstack's built-in defaults, deriving the
/// user-dependent skills-directory default from the injected <see cref="IRuntimeEnvironment"/>
/// rather than a hard-coded path.
/// </summary>
public sealed class DefaultConfigurationProvider : ICcbStackConfigurationProvider
{
    private const string ProviderName = "defaults";

    private readonly IRuntimeEnvironment _runtimeEnvironment;

    public DefaultConfigurationProvider(IRuntimeEnvironment runtimeEnvironment)
    {
        _runtimeEnvironment = runtimeEnvironment;
    }

    public ConfigurationLayer Layer => ConfigurationLayer.Defaults;

    public ValueTask<CcbStackConfigurationSource> LoadAsync(ConfigurationProviderContext context, CancellationToken cancellationToken)
    {
        var values = CcbStackConfigurationDefaults.Create(_runtimeEnvironment.UserProfileDirectory);

        var origins = new Dictionary<string, ConfigurationValueOrigin>
        {
            [CcbStackConfigurationKeys.DefaultModel] =
                new ConfigurationValueOrigin(ProviderName, Layer, CcbStackConfigurationKeys.DefaultModel),
            [CcbStackConfigurationKeys.SkillsDirectory] =
                new ConfigurationValueOrigin(ProviderName, Layer, CcbStackConfigurationKeys.SkillsDirectory),
            [CcbStackConfigurationKeys.Output.Format] =
                new ConfigurationValueOrigin(ProviderName, Layer, CcbStackConfigurationKeys.Output.Format),
        };

        var source = new CcbStackConfigurationSource(ProviderName, Layer, values, [])
        {
            Origins = origins,
        };

        return ValueTask.FromResult(source);
    }
}
