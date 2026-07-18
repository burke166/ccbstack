namespace CcbStack.Core.Execution;

/// <summary>The captured outcome of a completed process run.</summary>
public sealed record ProcessExecutionResult(int ExitCode, string StandardOutput, string StandardError)
{
    public bool Succeeded => ExitCode == 0;
}
