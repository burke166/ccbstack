using CcbStack.Core.IO;
using CcbStack.Core.Runtime;

namespace CcbStack.Core.Configuration.Providers;

/// <summary>
/// Reads recognized <c>CCBSTACK_*</c> environment variables (nested keys via double
/// underscore, e.g. <c>CCBSTACK_OUTPUT__FORMAT</c>). Unrecognized <c>CCBSTACK_*</c>
/// variables produce a warning rather than failing configuration loading. Never mutates the
/// process environment.
/// </summary>
public sealed class EnvironmentConfigurationProvider : ICcbStackConfigurationProvider
{
    private const string ProviderName = "environment";
    private const string Prefix = "CCBSTACK_";

    private readonly IEnvironmentVariableReader _environmentVariableReader;
    private readonly ConfigurationPathExpander _pathExpander;
    private readonly IRuntimeEnvironment _runtimeEnvironment;
    private readonly IReadOnlyDictionary<string, string> _knownVariableToKey;

    public EnvironmentConfigurationProvider(
        IEnvironmentVariableReader environmentVariableReader,
        ConfigurationPathExpander pathExpander,
        IRuntimeEnvironment runtimeEnvironment)
    {
        _environmentVariableReader = environmentVariableReader;
        _pathExpander = pathExpander;
        _runtimeEnvironment = runtimeEnvironment;

        // Environment-variable name matching is case-insensitive on Windows and follows the
        // platform's normal (case-sensitive) behavior elsewhere.
        var comparer = runtimeEnvironment.OperatingSystem == OperatingSystemKind.Windows
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

        _knownVariableToKey = new Dictionary<string, string>(comparer)
        {
            ["CCBSTACK_DEFAULTMODEL"] = CcbStackConfigurationKeys.DefaultModel,
            ["CCBSTACK_SKILLSDIRECTORY"] = CcbStackConfigurationKeys.SkillsDirectory,
            ["CCBSTACK_OUTPUT__FORMAT"] = CcbStackConfigurationKeys.Output.Format,
        };
    }

    public ConfigurationLayer Layer => ConfigurationLayer.Environment;

    public ValueTask<CcbStackConfigurationSource> LoadAsync(ConfigurationProviderContext context, CancellationToken cancellationToken)
    {
        var variables = _environmentVariableReader.GetVariables();
        var diagnostics = new List<ConfigurationDiagnostic>();
        var matchedByKey = new Dictionary<string, (string Value, string VariableName)>();

        foreach (var pair in variables)
        {
            var name = pair.Key;

            if (!name.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (_knownVariableToKey.TryGetValue(name, out var key))
            {
                matchedByKey[key] = (pair.Value ?? string.Empty, name);
            }
            else
            {
                diagnostics.Add(new ConfigurationDiagnostic(
                    "CFG004",
                    ConfigurationDiagnosticSeverity.Warning,
                    $"Unrecognized environment variable '{name}' is ignored.",
                    ProviderName));
            }
        }

        var values = new CcbStackConfigurationValues
        {
            DefaultModel = ValueFor(matchedByKey, CcbStackConfigurationKeys.DefaultModel),
            SkillsDirectory = ExpandedSkillsDirectory(matchedByKey),
            OutputFormat = ValueFor(matchedByKey, CcbStackConfigurationKeys.Output.Format),
        };

        var origins = matchedByKey.ToDictionary(
            pair => pair.Key,
            pair => new ConfigurationValueOrigin(ProviderName, Layer, pair.Key, EnvironmentVariableName: pair.Value.VariableName));

        var source = new CcbStackConfigurationSource(ProviderName, Layer, values, diagnostics)
        {
            Origins = origins,
        };

        return ValueTask.FromResult(source);
    }

    private static OptionalValue<string> ValueFor(Dictionary<string, (string Value, string VariableName)> matches, string key)
    {
        return matches.TryGetValue(key, out var match) ? OptionalValue<string>.Of(match.Value) : OptionalValue<string>.Unset;
    }

    private OptionalValue<string> ExpandedSkillsDirectory(Dictionary<string, (string Value, string VariableName)> matches)
    {
        if (!matches.TryGetValue(CcbStackConfigurationKeys.SkillsDirectory, out var match))
        {
            return OptionalValue<string>.Unset;
        }

        return OptionalValue<string>.Of(_pathExpander.Expand(match.Value, _runtimeEnvironment.CurrentDirectory, _runtimeEnvironment));
    }
}
