namespace CcbStack.Cli.Tests.TestSupport;

/// <summary>
/// Groups every test class that temporarily swaps the process-wide <see cref="Console.Out"/>/
/// <see cref="Console.Error"/> (to capture stdout/stderr around a command run). xUnit runs
/// different test classes in parallel by default, and that global, mutable state races across
/// classes if they aren't pinned to the same collection — a collection's tests always run
/// sequentially relative to each other, while still running in parallel with unrelated tests.
/// </summary>
[CollectionDefinition(Name)]
public sealed class ConsoleRedirectionCollection
{
    public const string Name = "Console redirection tests";
}
