namespace CcbStack.Core.Configuration;

/// <summary>
/// Canonical configuration key names, used consistently for environment-variable mapping,
/// diagnostics, JSON property paths, and (in the future) plugin-contributed configuration.
/// Do not reference property-name string literals elsewhere — reference these constants.
/// </summary>
public static class CcbStackConfigurationKeys
{
    public const string DefaultModel = "defaultModel";

    public const string SkillsDirectory = "skillsDirectory";

    public static class Output
    {
        public const string Format = "output.format";
    }
}
