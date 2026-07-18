using CcbStack.Core.Runtime;

namespace CcbStack.Core.Tests.TestSupport;

public sealed class FakeExecutableResolver : IExecutableResolver
{
    private readonly Dictionary<string, ResolvedExecutable> _resolved = new(StringComparer.OrdinalIgnoreCase);

    public static FakeExecutableResolver Empty => new();

    public FakeExecutableResolver Add(string name, string fullPath)
    {
        _resolved[name] = new ResolvedExecutable(name, fullPath);
        return this;
    }

    public ValueTask<ResolvedExecutable?> ResolveAsync(string executableName, CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(_resolved.TryGetValue(executableName, out var resolved) ? resolved : null);
    }
}
