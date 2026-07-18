namespace CcbStack.Core.Configuration;

/// <summary>
/// Built-in application defaults for ccbstack configuration. Values here must not contain
/// machine- or user-specific absolute paths; <see cref="Create"/> accepts the caller's user
/// profile directory (resolved through an injected runtime abstraction, not read directly)
/// so the skills-directory default can be derived without hard-coding it.
/// </summary>
public static class CcbStackConfigurationDefaults
{
    public const string DefaultModel = "sonnet";

    public const string OutputFormat = "text";

    /// <summary>Creates the built-in default configuration values.</summary>
    /// <param name="userProfileDirectory">The current user's profile directory, used to derive the default skills directory.</param>
    public static CcbStackConfigurationValues Create(string userProfileDirectory)
    {
        return new CcbStackConfigurationValues
        {
            DefaultModel = OptionalValue<string>.Of(DefaultModel),
            SkillsDirectory = OptionalValue<string>.Of(Path.Combine(userProfileDirectory, ".claude", "skills")),
            OutputFormat = OptionalValue<string>.Of(OutputFormat),
        };
    }
}
