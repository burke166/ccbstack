namespace CcbStack.Core.Configuration;

/// <summary>
/// The immutable, effective ccbstack configuration produced by merging all configuration
/// sources. Does not include runtime-discovered information such as resolved executable
/// paths, the operating system, or the current working directory — see
/// <c>IRuntimeEnvironment</c> and <c>IExecutableResolver</c> for those.
/// </summary>
/// <param name="DefaultModel">The default AI model ccbstack commands and skills should use.</param>
/// <param name="SkillsDirectory">The directory ccbstack looks in for skill definitions.</param>
/// <param name="Output">Output-related configuration.</param>
public sealed record CcbStackConfiguration(
    string DefaultModel,
    string SkillsDirectory,
    OutputConfiguration Output);

/// <summary>Output-related effective configuration.</summary>
/// <param name="Format">The default output format (<c>text</c> or <c>json</c>).</param>
public sealed record OutputConfiguration(string Format);
