namespace CcbStack.Core.Execution;

/// <summary>
/// Runs external processes (git, dotnet, go, ...) with argument-list quoting, working-directory
/// control, output capture, and cancellation. The single controlled seam every ccbstack feature
/// that shells out must use, per CLAUDE.md's process-execution conventions.
/// </summary>
public interface IProcessRunner
{
    Task<ProcessExecutionResult> RunAsync(ProcessExecutionRequest request, CancellationToken cancellationToken);
}
