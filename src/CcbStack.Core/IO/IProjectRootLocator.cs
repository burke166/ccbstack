namespace CcbStack.Core.IO;

/// <summary>
/// Locates the ccbstack project root for a given starting directory. Kept behind an
/// interface so future project-root strategies (git worktrees, solution files, other
/// repository markers) can replace this implementation without changing the configuration
/// service.
/// </summary>
public interface IProjectRootLocator
{
    /// <summary>
    /// Returns the nearest ancestor of <paramref name="startingDirectory"/> (inclusive) that
    /// looks like a project root, or <see langword="null"/> if none is found before reaching
    /// the filesystem root.
    /// </summary>
    DirectoryInfo? FindProjectRoot(DirectoryInfo startingDirectory);
}
