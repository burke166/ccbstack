using CcbStack.Core.Configuration;
using CcbStack.Core.Json;
using Spectre.Console;

namespace CcbStack.Cli.Output;

/// <summary>
/// The console-backed <see cref="ICommandOutput"/> implementation. <see cref="WriteObject{T}"/>
/// bypasses <see cref="IAnsiConsole"/> and writes directly to <see cref="Console.Out"/> so
/// JSON output is guaranteed free of ANSI escape sequences regardless of Spectre's terminal
/// detection.
/// </summary>
public sealed class ConsoleCommandOutput : ICommandOutput
{
    private readonly IAnsiConsole _console;
    private readonly ICcbStackJsonSerializer _jsonSerializer;

    public ConsoleCommandOutput(IAnsiConsole console, ICcbStackJsonSerializer jsonSerializer)
    {
        _console = console;
        _jsonSerializer = jsonSerializer;
    }

    public void WriteText(string text)
    {
        _console.WriteLine(text);
    }

    public void WriteObject<T>(T value)
    {
        var json = _jsonSerializer.Serialize(value, indented: true);
        Console.Out.WriteLine(json);
    }

    public void WriteDiagnostics(IReadOnlyList<ConfigurationDiagnostic> diagnostics)
    {
        foreach (var diagnostic in diagnostics)
        {
            var prefix = diagnostic.Severity == ConfigurationDiagnosticSeverity.Error ? "ERROR" : "WARNING";
            Console.Error.WriteLine($"{prefix} [{diagnostic.Code}]: {diagnostic.Message}");
        }
    }

    public void WriteError(CommandError error)
    {
        Console.Error.WriteLine(error.Message);
    }
}
