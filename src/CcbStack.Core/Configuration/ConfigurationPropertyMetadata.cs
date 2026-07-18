namespace CcbStack.Core.Configuration;

/// <summary>
/// Lightweight metadata allowing a configuration property to be marked sensitive so
/// structured output can mask it. No property in the current configuration model is
/// sensitive; this exists so future properties (e.g. API keys) can opt in without changing
/// the output pipeline.
/// </summary>
/// <param name="Key">The canonical configuration key (see <see cref="CcbStackConfigurationKeys"/>).</param>
/// <param name="IsSensitive">Whether values for this key must be masked in output.</param>
public sealed record ConfigurationPropertyMetadata(string Key, bool IsSensitive);
