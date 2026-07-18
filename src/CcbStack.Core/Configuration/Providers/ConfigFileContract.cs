using CcbStack.Core.Configuration;

namespace CcbStack.Core.Configuration.Providers;

/// <summary>
/// The on-disk JSON shape shared by the user config file, project config file, and
/// <c>--config-file</c>/<c>--config-json</c> command-line input. Nested to mirror
/// <c>{ "output": { "format": "json" } }</c>; flattened into <see cref="CcbStackConfigurationValues"/>
/// by <see cref="ConfigFileLoader"/>.
/// </summary>
internal sealed record ConfigFileContract
{
    public OptionalValue<string> DefaultModel { get; init; }

    public OptionalValue<string> SkillsDirectory { get; init; }

    public ConfigFileOutputContract? Output { get; init; }
}

internal sealed record ConfigFileOutputContract
{
    public OptionalValue<string> Format { get; init; }
}
