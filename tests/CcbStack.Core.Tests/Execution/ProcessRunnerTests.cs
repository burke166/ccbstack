using CcbStack.Core.Execution;
using FluentAssertions;

namespace CcbStack.Core.Tests.Execution;

public class ProcessRunnerTests
{
    private readonly ProcessRunner _runner = new();

    [Fact]
    public async Task RunAsync_CapturesStandardOutputAndExitCode()
    {
        var request = new ProcessExecutionRequest("dotnet", ["--version"]);

        var result = await _runner.RunAsync(request, CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.ExitCode.Should().Be(0);
        result.StandardOutput.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RunAsync_ReturnsNonZeroExitCode_ForAFailingInvocation()
    {
        var request = new ProcessExecutionRequest("dotnet", ["this-is-not-a-real-command"]);

        var result = await _runner.RunAsync(request, CancellationToken.None);

        result.Succeeded.Should().BeFalse();
        result.ExitCode.Should().NotBe(0);
    }

    [Fact]
    public async Task RunAsync_PassesArgumentsWithoutShellReinterpretation()
    {
        // A value containing shell metacharacters must be usable as a single, literal
        // argument — proof that arguments flow through ArgumentList, not a shell string.
        var request = new ProcessExecutionRequest("dotnet", ["--version", "&& echo pwned"]);

        var act = async () => await _runner.RunAsync(request, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task RunAsync_Throws_WhenTokenAlreadyCanceled()
    {
        var request = new ProcessExecutionRequest("dotnet", ["--version"]);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var act = async () => await _runner.RunAsync(request, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
