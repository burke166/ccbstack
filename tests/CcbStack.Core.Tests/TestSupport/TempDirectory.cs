namespace CcbStack.Core.Tests.TestSupport;

/// <summary>Creates an isolated, real temporary directory for a single test and deletes it on dispose.</summary>
public sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ccbstack-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
    }

    public string Path { get; }

    public DirectoryInfo Info => new(Path);

    public string Combine(params string[] segments)
    {
        return System.IO.Path.Combine([Path, .. segments]);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
        catch (IOException)
        {
            // Best-effort cleanup; leftover temp directories don't fail the test run.
        }
    }
}
