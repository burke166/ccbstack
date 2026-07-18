namespace CcbStack.Core.Runtime;

/// <summary>
/// Runtime environment information — as distinct from configuration — such as the current
/// working directory, well-known special-folder locations, and the operating system.
/// ccbstack commands and configuration providers should read this instead of calling
/// <see cref="Environment"/> directly, so tests can substitute fixed values.
/// </summary>
public interface IRuntimeEnvironment
{
    string CurrentDirectory { get; }

    string UserProfileDirectory { get; }

    string AppDataDirectory { get; }

    string LocalAppDataDirectory { get; }

    OperatingSystemKind OperatingSystem { get; }
}
