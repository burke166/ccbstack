namespace CcbStack.Core.Diagnostics;

/// <summary>
/// Represents ccbstack, .NET runtime, and operating system version information.
/// </summary>
/// <param name="ApplicationVersion">The ccbstack application/package version.</param>
/// <param name="RuntimeVersion">The .NET runtime version ccbstack is executing on.</param>
/// <param name="OperatingSystemDescription">A description of the current operating system.</param>
public sealed record VersionInfo(
    string ApplicationVersion,
    string RuntimeVersion,
    string OperatingSystemDescription);
