using CcbStack.Core.Runtime;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Runtime;

public class PathExecutableResolverTests
{
    private static readonly Dictionary<string, string?> WindowsPath = new()
    {
        ["PATH"] = @"C:\tools\bin;C:\other\bin",
    };

    [Fact]
    public async Task ResolveAsync_FindsExecutable_WhenExeFilePresentInPathDirectory()
    {
        var fileSystem = new FakeFileSystem().AddFile(@"C:\tools\bin\git.exe", "stub");
        var resolver = CreateResolver(fileSystem);

        var result = await resolver.ResolveAsync("git", CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("git");
        result.FullPath.Should().Be(@"C:\tools\bin\git.exe");
    }

    [Fact]
    public async Task ResolveAsync_ChecksAdditionalWindowsExtensions_WhenExeNotFound()
    {
        var fileSystem = new FakeFileSystem().AddFile(@"C:\tools\bin\pwsh.cmd", "stub");
        var resolver = CreateResolver(fileSystem);

        var result = await resolver.ResolveAsync("pwsh", CancellationToken.None);

        result.Should().NotBeNull();
        result!.FullPath.Should().Be(@"C:\tools\bin\pwsh.cmd");
    }

    [Fact]
    public async Task ResolveAsync_SearchesLaterPathDirectories_WhenNotFoundInEarlierOnes()
    {
        var fileSystem = new FakeFileSystem().AddFile(@"C:\other\bin\claude.exe", "stub");
        var resolver = CreateResolver(fileSystem);

        var result = await resolver.ResolveAsync("claude", CancellationToken.None);

        result.Should().NotBeNull();
        result!.FullPath.Should().Be(@"C:\other\bin\claude.exe");
    }

    [Fact]
    public async Task ResolveAsync_ReturnsNull_WhenExecutableIsNotFoundAnywhere()
    {
        var resolver = CreateResolver(new FakeFileSystem());

        var result = await resolver.ResolveAsync("does-not-exist", CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ResolveAsync_DoesNotAppendExtension_WhenNameAlreadyHasOne()
    {
        var fileSystem = new FakeFileSystem().AddFile(@"C:\tools\bin\tool.ps1", "stub");
        var resolver = CreateResolver(fileSystem);

        var result = await resolver.ResolveAsync("tool.ps1", CancellationToken.None);

        result.Should().NotBeNull();
        result!.FullPath.Should().Be(@"C:\tools\bin\tool.ps1");
    }

    [Fact]
    public async Task ResolveAsync_DoesNotThrow_WhenExecutableMissing()
    {
        var resolver = CreateResolver(new FakeFileSystem());

        var act = async () => await resolver.ResolveAsync("missing-tool", CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ResolveAsync_ThrowsOperationCanceled_WhenTokenAlreadyCanceled()
    {
        var resolver = CreateResolver(new FakeFileSystem());
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var act = async () => await resolver.ResolveAsync("git", cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ResolveAsync_DoesNotAppendWindowsExtensions_OnNonWindows()
    {
        // Path.Combine/PathSeparator are tied to the actual host OS regardless of the
        // simulated OperatingSystemKind, so build the expected path the same way the
        // resolver does rather than hard-coding a separator style.
        var directory = OperatingSystem.IsWindows() ? @"C:\tools\bin" : "/tools/bin";
        var expectedPath = Path.Combine(directory, "git");
        var fileSystem = new FakeFileSystem().AddFile(expectedPath, "stub");
        var environmentReader = new FakeEnvironmentVariableReader(
            new Dictionary<string, string?> { ["PATH"] = directory });
        var runtimeEnvironment = new FakeRuntimeEnvironment { OperatingSystem = OperatingSystemKind.Linux };
        var resolver = new PathExecutableResolver(runtimeEnvironment, environmentReader, fileSystem);

        var result = await resolver.ResolveAsync("git", CancellationToken.None);

        result.Should().NotBeNull();
        result!.FullPath.Should().Be(expectedPath);
        result.FullPath.Should().NotEndWith(".exe");
    }

    private static PathExecutableResolver CreateResolver(FakeFileSystem fileSystem)
    {
        var environmentReader = new FakeEnvironmentVariableReader(WindowsPath);
        var runtimeEnvironment = new FakeRuntimeEnvironment { OperatingSystem = OperatingSystemKind.Windows };
        return new PathExecutableResolver(runtimeEnvironment, environmentReader, fileSystem);
    }
}
