using CcbStack.Core.Runtime;

namespace CcbStack.Cli.Tests.TestSupport;

public sealed class StubExecutableResolver : IExecutableResolver
{
    private readonly Dictionary<string, ResolvedExecutable> _resolved = new(StringComparer.OrdinalIgnoreCase);

    public static StubExecutableResolver Empty => new();

    public StubExecutableResolver Add(string name, string fullPath)
    {
        _resolved[name] = new ResolvedExecutable(name, fullPath);
        return this;
    }

    public ValueTask<ResolvedExecutable?> ResolveAsync(string executableName, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(_resolved.TryGetValue(executableName, out var resolved) ? resolved : null);
    }
}
