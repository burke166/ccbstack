namespace CcbStack.Core.Configuration;

/// <summary>
/// The default <see cref="ICcbStackConfigurationValidator"/>, covering only the
/// configuration properties implemented in this milestone: required values and the
/// supported output formats.
/// </summary>
public sealed class CcbStackConfigurationValidator : ICcbStackConfigurationValidator
{
    private static readonly string[] SupportedOutputFormats = ["text", "json"];

    public IReadOnlyList<ConfigurationDiagnostic> Validate(CcbStackConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var diagnostics = new List<ConfigurationDiagnostic>();

        ValidateRequiredString(diagnostics, configuration.DefaultModel, CcbStackConfigurationKeys.DefaultModel, "Default model");
        ValidateSkillsDirectory(diagnostics, configuration.SkillsDirectory);
        ValidateOutputFormat(diagnostics, configuration.Output.Format);

        return diagnostics;
    }

    private static void ValidateRequiredString(
        List<ConfigurationDiagnostic> diagnostics, string value, string key, string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            diagnostics.Add(new ConfigurationDiagnostic(
                "CFG101",
                ConfigurationDiagnosticSeverity.Error,
                $"{displayName} must not be empty.",
                ConfigurationKey: key));
        }
    }

    private static void ValidateSkillsDirectory(List<ConfigurationDiagnostic> diagnostics, string skillsDirectory)
    {
        if (string.IsNullOrWhiteSpace(skillsDirectory))
        {
            diagnostics.Add(new ConfigurationDiagnostic(
                "CFG101",
                ConfigurationDiagnosticSeverity.Error,
                "Skills directory must not be empty.",
                ConfigurationKey: CcbStackConfigurationKeys.SkillsDirectory));
            return;
        }

        try
        {
            _ = Path.GetFullPath(skillsDirectory);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            diagnostics.Add(new ConfigurationDiagnostic(
                "CFG102",
                ConfigurationDiagnosticSeverity.Error,
                $"Skills directory '{skillsDirectory}' is not a usable path: {ex.Message}",
                ConfigurationKey: CcbStackConfigurationKeys.SkillsDirectory));
        }
    }

    private static void ValidateOutputFormat(List<ConfigurationDiagnostic> diagnostics, string format)
    {
        if (string.IsNullOrWhiteSpace(format) || !SupportedOutputFormats.Contains(format, StringComparer.OrdinalIgnoreCase))
        {
            diagnostics.Add(new ConfigurationDiagnostic(
                "CFG103",
                ConfigurationDiagnosticSeverity.Error,
                $"Unsupported output format '{format}'. Expected one of: {string.Join(", ", SupportedOutputFormats)}.",
                ConfigurationKey: CcbStackConfigurationKeys.Output.Format));
        }
    }
}
