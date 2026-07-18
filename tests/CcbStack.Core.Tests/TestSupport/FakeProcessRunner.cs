using CcbStack.Core.Execution;

namespace CcbStack.Core.Tests.TestSupport;

/// <summary>Maps a joined argument string to a canned <see cref="ProcessExecutionResult"/>, so git-shelling logic can be tested without a real git binary.</summary>
public sealed class FakeProcessRunner : IProcessRunner
{
    private readonly Dictionary<string, ProcessExecutionResult> _results = new();
    public List<ProcessExecutionRequest> Requests { get; } = [];

    public FakeProcessRunner AddResult(IEnumerable<string> arguments, ProcessExecutionResult result)
    {
        _results[Key(arguments)] = result;
        return this;
    }

    public Task<ProcessExecutionResult> RunAsync(ProcessExecutionRequest request, CancellationToken cancellationToken)
    {
        Requests.Add(request);

        if (_results.TryGetValue(Key(request.Arguments), out var result))
        {
            return Task.FromResult(result);
        }

        return Task.FromResult(new ProcessExecutionResult(1, string.Empty, "no fake result configured"));
    }

    private static string Key(IEnumerable<string> arguments) => string.Join(' ', arguments);
}
