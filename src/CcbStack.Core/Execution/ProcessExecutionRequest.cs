namespace CcbStack.Core.Execution;

/// <summary>
/// Describes a single external process invocation. <see cref="Arguments"/> are passed through
/// <see cref="System.Diagnostics.ProcessStartInfo.ArgumentList"/> rather than a concatenated
/// string, so callers never need to hand-quote arguments and untrusted text can never be
/// reinterpreted by a shell — no shell is invoked.
/// </summary>
/// <param name="FileName">The executable to run (a full path, or a name resolvable on PATH).</param>
/// <param name="Arguments">Arguments passed to the process, unquoted.</param>
/// <param name="WorkingDirectory">The working directory, or <see langword="null"/> for the current process's.</param>
/// <param name="Timeout">An optional timeout after which the process is killed and the run is cancelled.</param>
public sealed record ProcessExecutionRequest(
    string FileName,
    IReadOnlyList<string> Arguments,
    string? WorkingDirectory = null,
    TimeSpan? Timeout = null);
