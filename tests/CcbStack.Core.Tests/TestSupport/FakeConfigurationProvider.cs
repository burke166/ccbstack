using CcbStack.Core.Configuration;

namespace CcbStack.Core.Tests.TestSupport;

public sealed class FakeConfigurationProvider : ICcbStackConfigurationProvider
{
    private readonly Func<ConfigurationProviderContext, CancellationToken, ValueTask<CcbStackConfigurationSource>> _load;

    public FakeConfigurationProvider(
        ConfigurationLayer layer,
        Func<ConfigurationProviderContext, CancellationToken, ValueTask<CcbStackConfigurationSource>> load)
    {
        Layer = layer;
        _load = load;
    }

    public ConfigurationLayer Layer { get; }

    public ValueTask<CcbStackConfigurationSource> LoadAsync(ConfigurationProviderContext context, CancellationToken cancellationToken)
    {
        return _load(context, cancellationToken);
    }

    public static FakeConfigurationProvider Static(
        string name,
        ConfigurationLayer layer,
        CcbStackConfigurationValues values,
        params ConfigurationDiagnostic[] diagnostics)
    {
        var origins = new Dictionary<string, ConfigurationValueOrigin>();
        AddOriginIfSet(origins, values.DefaultModel, CcbStackConfigurationKeys.DefaultModel, name, layer);
        AddOriginIfSet(origins, values.SkillsDirectory, CcbStackConfigurationKeys.SkillsDirectory, name, layer);
        AddOriginIfSet(origins, values.OutputFormat, CcbStackConfigurationKeys.Output.Format, name, layer);

        var source = new CcbStackConfigurationSource(name, layer, values, diagnostics) { Origins = origins };
        return new FakeConfigurationProvider(layer, (_, _) => ValueTask.FromResult(source));
    }

    private static void AddOriginIfSet(
        Dictionary<string, ConfigurationValueOrigin> origins,
        OptionalValue<string> value,
        string key,
        string providerName,
        ConfigurationLayer layer)
    {
        if (value.IsSet)
        {
            origins[key] = new ConfigurationValueOrigin(providerName, layer, key);
        }
    }

    public static FakeConfigurationProvider Throwing(ConfigurationLayer layer, Exception exception)
    {
        return new FakeConfigurationProvider(layer, (_, _) => throw exception);
    }
}
