namespace CcbStack.Core.Configuration;

/// <summary>
/// The severity of a <see cref="ConfigurationDiagnostic"/>. Warnings do not prevent
/// configuration loading from succeeding; errors do.
/// </summary>
public enum ConfigurationDiagnosticSeverity
{
    Warning,
    Error,
}

/// <summary>
/// A structured configuration loading, parsing, or validation problem. Diagnostics are the
/// primary representation of expected configuration problems; exceptions are reserved for
/// unexpected I/O or runtime failures.
/// </summary>
/// <param name="Code">A stable, short identifier for the diagnostic (e.g. <c>"CFG001"</c>).</param>
/// <param name="Severity">Whether this diagnostic is a warning or an error.</param>
/// <param name="Message">A human-readable description of the problem.</param>
/// <param name="Source">The provider name or file path the diagnostic originated from, when known.</param>
/// <param name="ConfigurationKey">The configuration key the diagnostic relates to, when applicable.</param>
public sealed record ConfigurationDiagnostic(
    string Code,
    ConfigurationDiagnosticSeverity Severity,
    string Message,
    string? Source = null,
    string? ConfigurationKey = null);
