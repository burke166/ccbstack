using CcbStack.Core.Runtime;
using FluentAssertions;

namespace CcbStack.Core.Tests.Runtime;

public class RuntimeEnvironmentTests
{
    private readonly RuntimeEnvironment _runtimeEnvironment = new();

    [Fact]
    public void CurrentDirectory_IsNotEmpty()
    {
        _runtimeEnvironment.CurrentDirectory.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void UserProfileDirectory_IsNotEmpty()
    {
        _runtimeEnvironment.UserProfileDirectory.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void AppDataDirectory_IsNotEmpty()
    {
        _runtimeEnvironment.AppDataDirectory.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void LocalAppDataDirectory_IsNotEmpty()
    {
        _runtimeEnvironment.LocalAppDataDirectory.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void OperatingSystem_MatchesActualPlatform()
    {
        var expected = OperatingSystem.IsWindows()
            ? OperatingSystemKind.Windows
            : OperatingSystem.IsLinux()
                ? OperatingSystemKind.Linux
                : OperatingSystem.IsMacOS()
                    ? OperatingSystemKind.MacOs
                    : OperatingSystemKind.Other;

        _runtimeEnvironment.OperatingSystem.Should().Be(expected);
    }
}
