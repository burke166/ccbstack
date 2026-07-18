using CcbStack.Core.IO;
using CcbStack.Core.Repositories;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Repositories;

public class GoProjectDetectorTests
{
    private readonly GoProjectDetector _detector = new(new FileSystem());

    [Fact]
    public async Task DetectAsync_ReturnsNotDetected_WhenNoGoModExists()
    {
        var result = await _detector.DetectAsync(@"C:\repo", [], CancellationToken.None);

        result.HasGoMod.Should().BeFalse();
    }

    [Fact]
    public async Task DetectAsync_ParsesModuleNameAndGoVersion()
    {
        using var temp = new TempDirectory();
        File.WriteAllText(temp.Combine("go.mod"), "module github.com/example/tool\n\ngo 1.22\n");

        var result = await _detector.DetectAsync(temp.Path, ["go.mod"], CancellationToken.None);

        result.HasGoMod.Should().BeTrue();
        result.ModuleName.Should().Be("github.com/example/tool");
        result.GoVersion.Should().Be("1.22");
    }

    [Fact]
    public async Task DetectAsync_DetectsConventionalTopLevelDirectories()
    {
        using var temp = new TempDirectory();
        File.WriteAllText(temp.Combine("go.mod"), "module example\n\ngo 1.22\n");
        var relativeFiles = new[]
        {
            "go.mod",
            Path.Combine("cmd", "tool", "main.go"),
            Path.Combine("internal", "core", "core.go"),
            Path.Combine("pkg", "util", "util.go"),
        };

        var result = await _detector.DetectAsync(temp.Path, relativeFiles, CancellationToken.None);

        result.HasCmdDirectory.Should().BeTrue();
        result.HasInternalDirectory.Should().BeTrue();
        result.HasPkgDirectory.Should().BeTrue();
    }

    [Fact]
    public async Task DetectAsync_DoesNotFlagCmdDirectory_ForAFileNamedCmd()
    {
        using var temp = new TempDirectory();
        File.WriteAllText(temp.Combine("go.mod"), "module example\n\ngo 1.22\n");

        var result = await _detector.DetectAsync(temp.Path, ["go.mod", "cmd"], CancellationToken.None);

        result.HasCmdDirectory.Should().BeFalse();
    }

    [Fact]
    public async Task DetectAsync_DoesNotThrow_WhenGoModIsUnreadable()
    {
        var fileSystem = new FakeFileSystem().MarkUnreadable(@"C:\repo\go.mod");
        var detector = new GoProjectDetector(fileSystem);

        var result = await detector.DetectAsync(@"C:\repo", ["go.mod"], CancellationToken.None);

        result.HasGoMod.Should().BeTrue();
        result.ModuleName.Should().BeNull();
    }
}
