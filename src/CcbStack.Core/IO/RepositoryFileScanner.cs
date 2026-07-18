namespace CcbStack.Core.IO;

/// <summary>
/// The default <see cref="IRepositoryFileScanner"/>: a recursive walk that never descends into
/// <see cref="ExcludedDirectoryNames"/>, so large generated trees (bin/obj/node_modules/...)
/// are pruned instead of walked and filtered afterward. Inaccessible directories are skipped
/// rather than failing the whole scan, per CLAUDE.md's "fail gracefully" requirement.
/// </summary>
public sealed class RepositoryFileScanner : IRepositoryFileScanner
{
    private static readonly string[] ExcludedDirectoryNames =
    [
        ".git", ".hg", ".svn", ".ccbstack",
        "bin", "obj", "node_modules", ".vs", ".idea", "packages", "dist", "build",
    ];

    public IReadOnlyList<string> EnumerateFiles(string rootPath, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);

        var root = new DirectoryInfo(rootPath);
        if (!root.Exists)
        {
            return [];
        }

        var results = new List<string>();
        Walk(root, root, results, cancellationToken);
        return results;
    }

    private static void Walk(DirectoryInfo root, DirectoryInfo current, List<string> results, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IEnumerable<FileSystemInfo> entries;
        try
        {
            entries = current.EnumerateFileSystemInfos();
        }
        catch (UnauthorizedAccessException)
        {
            return;
        }
        catch (IOException)
        {
            return;
        }

        foreach (var entry in entries)
        {
            switch (entry)
            {
                case DirectoryInfo directory when !ExcludedDirectoryNames.Contains(directory.Name, StringComparer.OrdinalIgnoreCase):
                    Walk(root, directory, results, cancellationToken);
                    break;
                case FileInfo file:
                    results.Add(Path.GetRelativePath(root.FullName, file.FullName));
                    break;
            }
        }
    }
}
