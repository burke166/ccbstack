namespace CcbStack.Core.Configuration;

/// <summary>
/// The result of a single <see cref="ICcbStackConfigurationProvider"/> invocation: the
/// partial values it supplied, per-value origin metadata for the values it set, and any
/// source-specific loading or parsing diagnostics. A provider must not validate the final
/// effective configuration — validation is centralized in
/// <see cref="ICcbStackConfigurationValidator"/>.
/// </summary>
/// <param name="Name">The provider's name, used as origin metadata and in diagnostics.</param>
/// <param name="Layer">The precedence layer this source belongs to.</param>
/// <param name="Values">The partial configuration values this source supplied.</param>
/// <param name="Diagnostics">Loading or parsing diagnostics specific to this source.</param>
public sealed record CcbStackConfigurationSource(
    string Name,
    ConfigurationLayer Layer,
    CcbStackConfigurationValues Values,
    IReadOnlyList<ConfigurationDiagnostic> Diagnostics)
{
    /// <summary>
    /// Origin metadata for each configuration key this source set, keyed by the canonical
    /// configuration key (see <see cref="CcbStackConfigurationKeys"/>). A provider may leave
    /// this empty for keys where richer origin detail (file path, line/column) is not
    /// practical to supply.
    /// </summary>
    public IReadOnlyDictionary<string, ConfigurationValueOrigin> Origins { get; init; } =
        new Dictionary<string, ConfigurationValueOrigin>();
}
