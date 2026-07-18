using CcbStack.Core.IO;

namespace CcbStack.Core.Tests.TestSupport;

public sealed class FakeFileSystem : IFileSystem
{
    private readonly Dictionary<string, string> _files = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _directories = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _unreadablePaths = new(StringComparer.OrdinalIgnoreCase);

    public FakeFileSystem AddFile(string path, string contents)
    {
        _files[path] = contents;
        return this;
    }

    public FakeFileSystem AddDirectory(string path)
    {
        _directories[path] = path;
        return this;
    }

    public FakeFileSystem MarkUnreadable(string path)
    {
        _unreadablePaths.Add(path);
        return this;
    }

    public bool FileExists(string path) => _files.ContainsKey(path);

    public bool DirectoryExists(string path) => _directories.ContainsKey(path);

    public ValueTask<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
    {
        if (_unreadablePaths.Contains(path))
        {
            throw new IOException($"Simulated unreadable file: {path}");
        }

        if (!_files.TryGetValue(path, out var contents))
        {
            throw new FileNotFoundException("Simulated missing file.", path);
        }

        return ValueTask.FromResult(contents);
    }
}
