namespace CcbStack.Core.IO;

/// <summary>
/// The default <see cref="IProjectRootLocator"/>: walks upward from a starting directory,
/// treating a directory as a project-root candidate when it contains a <c>.git</c> or
/// <c>.ccbstack</c> entry (either a directory, or a file — <c>.git</c> is a file in git
/// worktrees and submodules). Stops at the filesystem root.
/// </summary>
public sealed class ProjectRootLocator : IProjectRootLocator
{
    private static readonly string[] MarkerNames = [".git", ".ccbstack"];

    public DirectoryInfo? FindProjectRoot(DirectoryInfo startingDirectory)
    {
        ArgumentNullException.ThrowIfNull(startingDirectory);

        for (var current = startingDirectory; current is not null; current = current.Parent)
        {
            if (MarkerNames.Any(marker => HasMarker(current, marker)))
            {
                return current;
            }
        }

        return null;
    }

    private static bool HasMarker(DirectoryInfo directory, string markerName)
    {
        var markerPath = Path.Combine(directory.FullName, markerName);
        return File.Exists(markerPath) || Directory.Exists(markerPath);
    }
}
