using System.Text.Json;
using CcbStack.Core.Diagnostics;

namespace CcbStack.Cli.Rendering;

/// <summary>
/// Renders a <see cref="VersionInfo"/> result as terminal text or JSON.
/// </summary>
internal static class VersionOutputFormatter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    /// <summary>
    /// Renders a simple, human-readable representation of <paramref name="versionInfo"/>.
    /// </summary>
    public static string ToText(VersionInfo versionInfo)
    {
        return string.Join(
            Environment.NewLine,
            $"ccbstack {versionInfo.ApplicationVersion}",
            $".NET {versionInfo.RuntimeVersion}",
            versionInfo.OperatingSystemDescription);
    }

    /// <summary>
    /// Renders <paramref name="versionInfo"/> as JSON, with no ANSI formatting, using the
    /// same result model as <see cref="ToText"/>.
    /// </summary>
    public static string ToJson(VersionInfo versionInfo)
    {
        return JsonSerializer.Serialize(versionInfo, JsonOptions);
    }
}
