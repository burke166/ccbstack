using CcbStack.Core.Runtime;

namespace CcbStack.Core.Tests.TestSupport;

public sealed class FakeRuntimeEnvironment : IRuntimeEnvironment
{
    public string CurrentDirectory { get; set; } = @"C:\work";

    public string UserProfileDirectory { get; set; } = @"C:\Users\test-user";

    public string AppDataDirectory { get; set; } = @"C:\Users\test-user\AppData\Roaming";

    public string LocalAppDataDirectory { get; set; } = @"C:\Users\test-user\AppData\Local";

    public OperatingSystemKind OperatingSystem { get; set; } = OperatingSystemKind.Windows;
}
