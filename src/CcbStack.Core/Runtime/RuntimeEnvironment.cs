using System.Runtime.InteropServices;

namespace CcbStack.Core.Runtime;

/// <summary>The real <see cref="IRuntimeEnvironment"/> implementation, backed by the .NET BCL.</summary>
public sealed class RuntimeEnvironment : IRuntimeEnvironment
{
    public string CurrentDirectory => Environment.CurrentDirectory;

    public string UserProfileDirectory => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public string AppDataDirectory => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public string LocalAppDataDirectory => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public OperatingSystemKind OperatingSystem => DetermineOperatingSystem();

    private static OperatingSystemKind DetermineOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OperatingSystemKind.Windows;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OperatingSystemKind.Linux;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OperatingSystemKind.MacOs;
        }

        return OperatingSystemKind.Other;
    }
}
