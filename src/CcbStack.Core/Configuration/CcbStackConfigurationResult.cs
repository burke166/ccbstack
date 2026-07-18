namespace CcbStack.Core.Configuration;

/// <summary>
/// The outcome of loading and merging configuration from all registered providers.
/// <see cref="Configuration"/> may be non-<see langword="null"/> even when
/// <see cref="Diagnostics"/> contains errors, so callers can inspect what was loaded
/// alongside what is wrong with it; <see cref="IsSuccess"/> is the single signal for whether
/// the result is safe to use.
/// </summary>
/// <param name="Configuration">The materialized effective configuration, or <see langword="null"/> if required values could not be resolved.</param>
/// <param name="Origins">Origin metadata for each resolved configuration key, identifying the winning source.</param>
/// <param name="Diagnostics">All warnings and errors collected while loading and validating configuration.</param>
public sealed record CcbStackConfigurationResult(
    CcbStackConfiguration? Configuration,
    IReadOnlyDictionary<string, ConfigurationValueOrigin> Origins,
    IReadOnlyList<ConfigurationDiagnostic> Diagnostics)
{
    public bool IsSuccess =>
        Configuration is not null &&
        Diagnostics.All(x => x.Severity != ConfigurationDiagnosticSeverity.Error);
}
