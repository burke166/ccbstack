namespace CcbStack.Core.IO;

/// <summary>
/// A minimal filesystem abstraction covering only the operations configuration providers
/// need, so unit tests can substitute a fake instead of touching the real filesystem.
/// </summary>
public interface IFileSystem
{
    bool FileExists(string path);

    bool DirectoryExists(string path);

    ValueTask<string> ReadAllTextAsync(string path, CancellationToken cancellationToken);
}
