using CcbStack.Cli.Output;
using CcbStack.Core.Configuration;
using CcbStack.Core.Json;
using FluentAssertions;
using Spectre.Console.Testing;

namespace CcbStack.Cli.Tests.Output;

[Collection(CcbStack.Cli.Tests.TestSupport.ConsoleRedirectionCollection.Name)]
public class ConsoleCommandOutputTests
{
    private readonly TestConsole _testConsole = new();
    private readonly ConsoleCommandOutput _output;

    public ConsoleCommandOutputTests()
    {
        _output = new ConsoleCommandOutput(_testConsole, new CcbStackJsonSerializer());
    }

    [Fact]
    public void WriteText_WritesToTheAnsiConsole()
    {
        _output.WriteText("hello from ccbstack");

        _testConsole.Output.Should().Contain("hello from ccbstack");
    }

    [Fact]
    public void WriteObject_WritesValidJson_ToStandardOut()
    {
        var (stdOut, _) = RunWithRedirectedConsole(() => _output.WriteObject(new { name = "sonnet" }));

        using var document = System.Text.Json.JsonDocument.Parse(stdOut);
        document.RootElement.GetProperty("name").GetString().Should().Be("sonnet");
    }

    [Fact]
    public void WriteObject_DoesNotWriteToTheAnsiConsole()
    {
        RunWithRedirectedConsole(() => _output.WriteObject(new { name = "sonnet" }));

        _testConsole.Output.Should().BeEmpty();
    }

    [Fact]
    public void WriteDiagnostics_WritesEachDiagnostic_ToStandardError()
    {
        var diagnostics = new List<ConfigurationDiagnostic>
        {
            new("CFG001", ConfigurationDiagnosticSeverity.Error, "first problem"),
            new("CFG002", ConfigurationDiagnosticSeverity.Warning, "second problem"),
        };

        var (stdOut, stdErr) = RunWithRedirectedConsole(() => _output.WriteDiagnostics(diagnostics));

        stdErr.Should().Contain("first problem");
        stdErr.Should().Contain("second problem");
        stdOut.Should().BeEmpty();
    }

    [Fact]
    public void WriteDiagnostics_PrefixesErrorsAndWarningsDifferently()
    {
        var diagnostics = new List<ConfigurationDiagnostic>
        {
            new("CFG001", ConfigurationDiagnosticSeverity.Error, "error message"),
            new("CFG002", ConfigurationDiagnosticSeverity.Warning, "warning message"),
        };

        var (_, stdErr) = RunWithRedirectedConsole(() => _output.WriteDiagnostics(diagnostics));

        stdErr.Should().Contain("ERROR");
        stdErr.Should().Contain("WARNING");
    }

    [Fact]
    public void WriteError_WritesMessage_ToStandardError()
    {
        var (stdOut, stdErr) = RunWithRedirectedConsole(() => _output.WriteError(new CommandError("something broke")));

        stdErr.Should().Contain("something broke");
        stdOut.Should().BeEmpty();
    }

    private static (string StdOut, string StdErr) RunWithRedirectedConsole(Action action)
    {
        var originalOut = Console.Out;
        var originalError = Console.Error;
        var stdOutWriter = new StringWriter();
        var stdErrWriter = new StringWriter();

        try
        {
            Console.SetOut(stdOutWriter);
            Console.SetError(stdErrWriter);
            action();
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }

        return (stdOutWriter.ToString().Trim(), stdErrWriter.ToString().Trim());
    }
}
