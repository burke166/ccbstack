using System.Reflection;
using System.Runtime.InteropServices;

namespace CcbStack.Core.Diagnostics;

/// <summary>
/// Collects ccbstack, .NET runtime, and operating system version information from
/// authoritative assembly and runtime metadata.
/// </summary>
public sealed class VersionInfoProvider
{
    /// <summary>
    /// Returns the current ccbstack, .NET runtime, and operating system version information.
    /// </summary>
    public VersionInfo GetVersionInfo()
    {
        return new VersionInfo(
            ApplicationVersion: GetApplicationVersion(),
            RuntimeVersion: RuntimeInformation.FrameworkDescription,
            OperatingSystemDescription: RuntimeInformation.OSDescription);
    }

    private static string GetApplicationVersion()
    {
        var assembly = typeof(VersionInfoProvider).Assembly;

        var informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            // Strip any source-control metadata suffix (e.g. "0.1.0-alpha.1+abcdef0123")
            // so the reported version matches the package's semantic version.
            var metadataSeparatorIndex = informationalVersion.IndexOf('+');
            return metadataSeparatorIndex >= 0
                ? informationalVersion[..metadataSeparatorIndex]
                : informationalVersion;
        }

        return assembly.GetName().Version?.ToString() ?? "unknown";
    }
}
