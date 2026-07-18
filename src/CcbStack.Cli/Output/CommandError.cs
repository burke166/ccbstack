namespace CcbStack.Cli.Output;

/// <summary>An unexpected, command-level failure — not a structured configuration diagnostic.</summary>
/// <param name="Message">A human-readable description of the failure.</param>
/// <param name="Exception">The underlying exception, when applicable.</param>
public sealed record CommandError(string Message, Exception? Exception = null);
