namespace CcbStack.Core.Configuration;

/// <summary>
/// Identifies where a resolved configuration value came from. Not all fields apply to every
/// source kind (e.g. <see cref="EnvironmentVariableName"/> is only populated by the
/// environment-variable provider); retained for future diagnostic and verbose-output commands.
/// </summary>
/// <param name="ProviderName">The name of the provider that supplied the winning value.</param>
/// <param name="Layer">The precedence layer the winning value came from.</param>
/// <param name="ConfigurationKey">The canonical configuration key (see <see cref="CcbStackConfigurationKeys"/>).</param>
/// <param name="SourcePath">The configuration file path, when the value came from a file.</param>
/// <param name="EnvironmentVariableName">The environment variable name, when the value came from the environment.</param>
/// <param name="JsonPropertyPath">The JSON property path within the source file, when applicable.</param>
/// <param name="LineNumber">The line number within the source file, when available.</param>
/// <param name="ColumnNumber">The column number within the source file, when available.</param>
public sealed record ConfigurationValueOrigin(
    string ProviderName,
    ConfigurationLayer Layer,
    string ConfigurationKey,
    string? SourcePath = null,
    string? EnvironmentVariableName = null,
    string? JsonPropertyPath = null,
    int? LineNumber = null,
    int? ColumnNumber = null);
