namespace CcbStack.Core.Runtime;

/// <summary>
/// Resolves external executables (git, PowerShell, Claude, ...) by name. Deliberately kept
/// separate from configuration loading — executable discovery is runtime information, not a
/// configuration value, and a missing executable must never fail configuration loading.
/// </summary>
public interface IExecutableResolver
{
    /// <summary>
    /// Attempts to locate <paramref name="executableName"/>, returning <see langword="null"/>
    /// if it cannot be found rather than throwing.
    /// </summary>
    ValueTask<ResolvedExecutable?> ResolveAsync(string executableName, CancellationToken cancellationToken);
}
