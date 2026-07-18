using CcbStack.Core.Configuration;

namespace CcbStack.Cli.Output;

/// <summary>
/// A reusable, command-agnostic structured output abstraction. Terminal presentation and
/// JSON presentation are CLI-boundary concerns (not <c>CcbStack.Core</c>'s), and this
/// interface keeps that presentation decoupled from configuration loading and other core
/// services so future commands can reuse it.
/// </summary>
public interface ICommandOutput
{
    /// <summary>Writes human-readable text to standard output.</summary>
    void WriteText(string text);

    /// <summary>Serializes <paramref name="value"/> as JSON (no ANSI, no decorative text) to standard output.</summary>
    void WriteObject<T>(T value);

    /// <summary>Writes each diagnostic as a human-readable line to standard error.</summary>
    void WriteDiagnostics(IReadOnlyList<ConfigurationDiagnostic> diagnostics);

    /// <summary>Writes an unexpected, command-level failure to standard error.</summary>
    void WriteError(CommandError error);
}
