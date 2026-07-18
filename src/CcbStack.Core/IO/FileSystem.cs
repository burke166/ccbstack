namespace CcbStack.Core.IO;

/// <summary>The real, disk-backed <see cref="IFileSystem"/> implementation.</summary>
public sealed class FileSystem : IFileSystem
{
    public bool FileExists(string path) => File.Exists(path);

    public bool DirectoryExists(string path) => Directory.Exists(path);

    public async ValueTask<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
    {
        return await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
    }
}
