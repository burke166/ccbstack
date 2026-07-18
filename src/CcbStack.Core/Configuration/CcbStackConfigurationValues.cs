namespace CcbStack.Core.Configuration;

/// <summary>
/// A partial set of configuration values contributed by a single provider. Every property
/// is an <see cref="OptionalValue{T}"/> so the merge algorithm can distinguish a property
/// that was not supplied from one explicitly supplied with a default-like value.
/// </summary>
public sealed record CcbStackConfigurationValues
{
    public OptionalValue<string> DefaultModel { get; init; }

    public OptionalValue<string> SkillsDirectory { get; init; }

    public OptionalValue<string> OutputFormat { get; init; }
}
