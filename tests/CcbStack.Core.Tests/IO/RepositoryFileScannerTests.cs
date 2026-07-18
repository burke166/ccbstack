using CcbStack.Core.IO;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.IO;

public class RepositoryFileScannerTests
{
    private readonly RepositoryFileScanner _scanner = new();

    [Fact]
    public void EnumerateFiles_ReturnsRelativePaths_ForNestedFiles()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine("src", "deep"));
        File.WriteAllText(temp.Combine("readme.md"), "hi");
        File.WriteAllText(temp.Combine("src", "deep", "Program.cs"), "// code");

        var files = _scanner.EnumerateFiles(temp.Path, CancellationToken.None);

        files.Should().Contain("readme.md");
        files.Should().Contain(Path.Combine("src", "deep", "Program.cs"));
    }

    [Theory]
    [InlineData(".git")]
    [InlineData("bin")]
    [InlineData("obj")]
    [InlineData("node_modules")]
    public void EnumerateFiles_PrunesExcludedDirectories(string excludedDirectoryName)
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(excludedDirectoryName));
        File.WriteAllText(temp.Combine(excludedDirectoryName, "noise.txt"), "noise");
        File.WriteAllText(temp.Combine("real.txt"), "real");

        var files = _scanner.EnumerateFiles(temp.Path, CancellationToken.None);

        files.Should().Contain("real.txt");
        files.Should().NotContain(f => f.Contains(excludedDirectoryName));
    }

    [Fact]
    public void EnumerateFiles_ReturnsEmpty_WhenRootDoesNotExist()
    {
        var files = _scanner.EnumerateFiles(@"C:\this\path\does\not\exist\ccbstack-test", CancellationToken.None);

        files.Should().BeEmpty();
    }

    [Fact]
    public void EnumerateFiles_Throws_WhenCancellationAlreadyRequested()
    {
        using var temp = new TempDirectory();
        File.WriteAllText(temp.Combine("file.txt"), "hi");
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => _scanner.EnumerateFiles(temp.Path, cts.Token);

        act.Should().Throw<OperationCanceledException>();
    }
}
