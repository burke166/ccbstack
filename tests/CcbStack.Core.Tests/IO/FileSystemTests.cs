using CcbStack.Core.IO;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.IO;

public class FileSystemTests
{
    private readonly FileSystem _fileSystem = new();

    [Fact]
    public void FileExists_ReturnsTrue_ForExistingFile()
    {
        using var temp = new TempDirectory();
        var path = temp.Combine("config.json");
        File.WriteAllText(path, "{}");

        _fileSystem.FileExists(path).Should().BeTrue();
    }

    [Fact]
    public void FileExists_ReturnsFalse_ForMissingFile()
    {
        using var temp = new TempDirectory();

        _fileSystem.FileExists(temp.Combine("missing.json")).Should().BeFalse();
    }

    [Fact]
    public void DirectoryExists_ReturnsTrue_ForExistingDirectory()
    {
        using var temp = new TempDirectory();

        _fileSystem.DirectoryExists(temp.Path).Should().BeTrue();
    }

    [Fact]
    public async Task ReadAllTextAsync_ReturnsFileContents()
    {
        using var temp = new TempDirectory();
        var path = temp.Combine("config.json");
        File.WriteAllText(path, """{ "defaultModel": "sonnet" }""");

        var contents = await _fileSystem.ReadAllTextAsync(path, CancellationToken.None);

        contents.Should().Contain("sonnet");
    }

    [Fact]
    public async Task ReadAllTextAsync_CanReadReadOnlyFile()
    {
        using var temp = new TempDirectory();
        var path = temp.Combine("readonly-config.json");
        File.WriteAllText(path, """{ "defaultModel": "sonnet" }""");
        File.SetAttributes(path, FileAttributes.ReadOnly);

        try
        {
            var contents = await _fileSystem.ReadAllTextAsync(path, CancellationToken.None);

            contents.Should().Contain("sonnet");
        }
        finally
        {
            File.SetAttributes(path, FileAttributes.Normal);
        }
    }
}
