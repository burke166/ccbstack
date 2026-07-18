using CcbStack.Core.IO;

namespace CcbStack.Core.Runtime;

/// <summary>
/// The default <see cref="IExecutableResolver"/>: scans the directories in the <c>PATH</c>
/// environment variable, probing common Windows executable extensions
/// (<c>.exe</c>/<c>.cmd</c>/<c>.bat</c>) when <paramref name="executableName"/> has none.
/// Resolution is synchronous, lightweight file-existence probing — no process is started.
/// </summary>
public sealed class PathExecutableResolver : IExecutableResolver
{
    private static readonly string[] WindowsExtensions = [".exe", ".cmd", ".bat"];

    private readonly IRuntimeEnvironment _runtimeEnvironment;
    private readonly IEnvironmentVariableReader _environmentVariableReader;
    private readonly IFileSystem _fileSystem;

    public PathExecutableResolver(
        IRuntimeEnvironment runtimeEnvironment,
        IEnvironmentVariableReader environmentVariableReader,
        IFileSystem fileSystem)
    {
        _runtimeEnvironment = runtimeEnvironment;
        _environmentVariableReader = environmentVariableReader;
        _fileSystem = fileSystem;
    }

    public ValueTask<ResolvedExecutable?> ResolveAsync(string executableName, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executableName);

        var variables = _environmentVariableReader.GetVariables();
        var pathVariable = variables.TryGetValue("PATH", out var value) ? value : null;
        var searchDirectories = (pathVariable ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        var candidateNames = BuildCandidateNames(executableName);

        foreach (var directory in searchDirectories)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var candidateName in candidateNames)
            {
                if (!TryCombine(directory, candidateName, out var candidatePath))
                {
                    continue;
                }

                if (_fileSystem.FileExists(candidatePath))
                {
                    return ValueTask.FromResult<ResolvedExecutable?>(new ResolvedExecutable(executableName, candidatePath));
                }
            }
        }

        return ValueTask.FromResult<ResolvedExecutable?>(null);
    }

    private IReadOnlyList<string> BuildCandidateNames(string executableName)
    {
        if (_runtimeEnvironment.OperatingSystem != OperatingSystemKind.Windows || Path.HasExtension(executableName))
        {
            return [executableName];
        }

        var candidates = new List<string>(WindowsExtensions.Length);

        foreach (var extension in WindowsExtensions)
        {
            candidates.Add(executableName + extension);
        }

        return candidates;
    }

    private static bool TryCombine(string directory, string fileName, out string combined)
    {
        try
        {
            combined = Path.Combine(directory, fileName);
            return true;
        }
        catch (ArgumentException)
        {
            combined = string.Empty;
            return false;
        }
    }
}
