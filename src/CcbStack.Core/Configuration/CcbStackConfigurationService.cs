using CcbStack.Core.Runtime;

namespace CcbStack.Core.Configuration;

/// <summary>
/// The default <see cref="ICcbStackConfigurationService"/>: discovers all registered
/// <see cref="ICcbStackConfigurationProvider"/> implementations, invokes them in
/// <see cref="ConfigurationLayer"/> order regardless of registration order, merges their
/// partial values (a higher layer only overwrites a key it explicitly set), materializes the
/// immutable effective configuration, and runs <see cref="ICcbStackConfigurationValidator"/>
/// against the result.
/// </summary>
public sealed class CcbStackConfigurationService : ICcbStackConfigurationService
{
    private readonly IEnumerable<ICcbStackConfigurationProvider> _providers;
    private readonly ICcbStackConfigurationValidator _validator;
    private readonly IRuntimeEnvironment _runtimeEnvironment;

    public CcbStackConfigurationService(
        IEnumerable<ICcbStackConfigurationProvider> providers,
        ICcbStackConfigurationValidator validator,
        IRuntimeEnvironment runtimeEnvironment)
    {
        _providers = providers;
        _validator = validator;
        _runtimeEnvironment = runtimeEnvironment;
    }

    public async ValueTask<CcbStackConfigurationResult> LoadAsync(
        CommandLineConfigurationInput commandLineInput, CancellationToken cancellationToken)
    {
        var context = new ConfigurationProviderContext(_runtimeEnvironment.CurrentDirectory, commandLineInput);
        var orderedProviders = _providers.OrderBy(provider => (int)provider.Layer).ToList();

        var diagnostics = new List<ConfigurationDiagnostic>();
        var origins = new Dictionary<string, ConfigurationValueOrigin>();
        var merged = new CcbStackConfigurationValues();

        foreach (var provider in orderedProviders)
        {
            cancellationToken.ThrowIfCancellationRequested();

            CcbStackConfigurationSource source;

            try
            {
                source = await provider.LoadAsync(context, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                diagnostics.Add(new ConfigurationDiagnostic(
                    "CFG901",
                    ConfigurationDiagnosticSeverity.Error,
                    $"Configuration provider '{provider.GetType().Name}' failed: {ex.Message}",
                    provider.GetType().Name));
                continue;
            }

            diagnostics.AddRange(source.Diagnostics);
            merged = Merge(lower: merged, higher: source.Values);

            // Iterating providers in ascending layer order means a later (higher-precedence,
            // or same-layer later-registered) source's origin deterministically overwrites an
            // earlier one for any key both set.
            foreach (var (key, origin) in source.Origins)
            {
                origins[key] = origin;
            }
        }

        var (configuration, materializationDiagnostics) = Materialize(merged);
        diagnostics.AddRange(materializationDiagnostics);

        if (configuration is not null)
        {
            diagnostics.AddRange(_validator.Validate(configuration));
        }

        return new CcbStackConfigurationResult(configuration, origins, diagnostics);
    }

    private static CcbStackConfigurationValues Merge(CcbStackConfigurationValues lower, CcbStackConfigurationValues higher)
    {
        return new CcbStackConfigurationValues
        {
            DefaultModel = higher.DefaultModel.IsSet ? higher.DefaultModel : lower.DefaultModel,
            SkillsDirectory = higher.SkillsDirectory.IsSet ? higher.SkillsDirectory : lower.SkillsDirectory,
            OutputFormat = higher.OutputFormat.IsSet ? higher.OutputFormat : lower.OutputFormat,
        };
    }

    private static (CcbStackConfiguration? Configuration, IReadOnlyList<ConfigurationDiagnostic> Diagnostics) Materialize(
        CcbStackConfigurationValues values)
    {
        var diagnostics = new List<ConfigurationDiagnostic>();

        var defaultModel = RequireValue(values.DefaultModel, CcbStackConfigurationKeys.DefaultModel, diagnostics);
        var skillsDirectory = RequireValue(values.SkillsDirectory, CcbStackConfigurationKeys.SkillsDirectory, diagnostics);
        var outputFormat = RequireValue(values.OutputFormat, CcbStackConfigurationKeys.Output.Format, diagnostics);

        if (defaultModel is null || skillsDirectory is null || outputFormat is null)
        {
            return (null, diagnostics);
        }

        var configuration = new CcbStackConfiguration(defaultModel, skillsDirectory, new OutputConfiguration(outputFormat));
        return (configuration, diagnostics);
    }

    private static string? RequireValue(OptionalValue<string> value, string key, List<ConfigurationDiagnostic> diagnostics)
    {
        if (!value.IsSet || value.IsNull || value.Value is null)
        {
            diagnostics.Add(new ConfigurationDiagnostic(
                "CFG100",
                ConfigurationDiagnosticSeverity.Error,
                $"Configuration key '{key}' could not be resolved to a value.",
                ConfigurationKey: key));
            return null;
        }

        return value.Value;
    }
}
