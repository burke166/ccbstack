using CcbStack.Cli.DependencyInjection;
using CcbStack.Cli.Output;
using CcbStack.Cli.Tests.TestSupport;
using CcbStack.Core.Configuration;
using CcbStack.Core.Json;
using CcbStack.Core.Runtime;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Testing;
using Spectre.Console.Testing;

namespace CcbStack.Cli.Tests.Commands;

public class ConfigCommandTests
{
    [Fact]
    public void Execute_TextOutput_ContainsFormattedConfigurationValues()
    {
        var configurationService = FakeCcbStackConfigurationService.Returning(
            FakeCcbStackConfigurationService.SuccessResult("sonnet", @"C:\Users\test\.claude\skills", "text"));

        var (exitCode, testConsoleOutput, _, _) = Run(configurationService, StubExecutableResolver.Empty);

        exitCode.Should().Be(CcbStackExitCodes.Success);
        testConsoleOutput.Should().Contain("Default Model").And.Contain("sonnet");
        testConsoleOutput.Should().Contain("Skills Directory").And.Contain(@"C:\Users\test\.claude\skills");
        testConsoleOutput.Should().Contain("Output Format").And.Contain("text");
        testConsoleOutput.Should().Contain("Runtime Environment");
    }

    [Fact]
    public void Execute_TextOutput_ShowsNotFound_ForUnresolvedExecutables()
    {
        var configurationService = FakeCcbStackConfigurationService.Returning(
            FakeCcbStackConfigurationService.SuccessResult());

        var (_, testConsoleOutput, _, _) = Run(configurationService, StubExecutableResolver.Empty);

        testConsoleOutput.Should().Contain("PowerShell").And.Contain("not found");
        testConsoleOutput.Should().Contain("Git").And.Contain("not found");
        testConsoleOutput.Should().Contain("Claude").And.Contain("not found");
    }

    [Fact]
    public void Execute_TextOutput_ShowsResolvedExecutablePaths()
    {
        var configurationService = FakeCcbStackConfigurationService.Returning(
            FakeCcbStackConfigurationService.SuccessResult());
        var resolver = new StubExecutableResolver()
            .Add("git", @"C:\Program Files\Git\cmd\git.exe")
            .Add("pwsh", @"C:\Program Files\PowerShell\7\pwsh.exe");

        var (_, testConsoleOutput, _, _) = Run(configurationService, resolver);

        testConsoleOutput.Should().Contain(@"C:\Program Files\Git\cmd\git.exe");
        testConsoleOutput.Should().Contain(@"C:\Program Files\PowerShell\7\pwsh.exe");
    }

    [Fact]
    public void Execute_JsonOutput_IsValidParsableJson()
    {
        var configurationService = FakeCcbStackConfigurationService.Returning(
            FakeCcbStackConfigurationService.SuccessResult("opus", @"C:\skills", "json"));

        var (exitCode, _, stdOut, _) = Run(configurationService, StubExecutableResolver.Empty, "--json");

        exitCode.Should().Be(CcbStackExitCodes.Success);
        var act = () => System.Text.Json.JsonDocument.Parse(stdOut);
        act.Should().NotThrow();
    }

    [Fact]
    public void Execute_JsonOutput_ContainsNoAnsiEscapeSequences()
    {
        var configurationService = FakeCcbStackConfigurationService.Returning(
            FakeCcbStackConfigurationService.SuccessResult());

        var (_, _, stdOut, _) = Run(configurationService, StubExecutableResolver.Empty, "--json");

        stdOut.Should().NotContain("[");
    }

    [Fact]
    public void Execute_JsonOutput_ContainsExpectedFields()
    {
        var configurationService = FakeCcbStackConfigurationService.Returning(
            FakeCcbStackConfigurationService.SuccessResult("opus", @"C:\skills", "json"));

        var (_, _, stdOut, _) = Run(configurationService, StubExecutableResolver.Empty, "--json");

        using var document = System.Text.Json.JsonDocument.Parse(stdOut);
        var root = document.RootElement;

        root.GetProperty("configuration").GetProperty("defaultModel").GetString().Should().Be("opus");
        root.GetProperty("configuration").GetProperty("skillsDirectory").GetString().Should().Be(@"C:\skills");
        root.GetProperty("configuration").GetProperty("output").GetProperty("format").GetString().Should().Be("json");
        root.TryGetProperty("runtime", out _).Should().BeTrue();
    }

    [Fact]
    public void Execute_JsonOutput_IsWrittenToStandardOut_NotTheAnsiConsole()
    {
        var configurationService = FakeCcbStackConfigurationService.Returning(
            FakeCcbStackConfigurationService.SuccessResult());

        var (_, testConsoleOutput, stdOut, _) = Run(configurationService, StubExecutableResolver.Empty, "--json");

        stdOut.Should().Contain("defaultModel");
        testConsoleOutput.Should().NotContain("defaultModel");
    }

    [Fact]
    public void Execute_MutuallyExclusiveOptions_ReturnsInvalidCommandLineExitCode()
    {
        var configurationService = FakeCcbStackConfigurationService.Returning(
            FakeCcbStackConfigurationService.SuccessResult());

        var (exitCode, _, _, stdErr) = Run(
            configurationService, StubExecutableResolver.Empty,
            "--config-json", "{}", "--config-file", "file.json");

        exitCode.Should().Be(CcbStackExitCodes.InvalidCommandLine);
        stdErr.Should().Contain("mutually exclusive");
    }

    [Fact]
    public void Execute_ConfigurationLoadReportsErrors_ReturnsInvalidConfigurationExitCode()
    {
        var errorResult = new CcbStackConfigurationResult(
            null,
            new Dictionary<string, ConfigurationValueOrigin>(),
            [new ConfigurationDiagnostic("CFG001", ConfigurationDiagnosticSeverity.Error, "malformed JSON")]);
        var configurationService = FakeCcbStackConfigurationService.Returning(errorResult);

        var (exitCode, _, stdOut, stdErr) = Run(configurationService, StubExecutableResolver.Empty);

        exitCode.Should().Be(CcbStackExitCodes.InvalidConfiguration);
        stdErr.Should().Contain("malformed JSON");
        stdOut.Should().BeEmpty();
    }

    [Fact]
    public void Execute_ValidationFailure_KeepsConfigurationButReturnsInvalidConfigurationExitCode()
    {
        var configuration = new CcbStackConfiguration("sonnet", @"C:\skills", new OutputConfiguration("xml"));
        var failingResult = new CcbStackConfigurationResult(
            configuration,
            new Dictionary<string, ConfigurationValueOrigin>(),
            [new ConfigurationDiagnostic("CFG103", ConfigurationDiagnosticSeverity.Error, "Unsupported output format 'xml'.")]);
        var configurationService = FakeCcbStackConfigurationService.Returning(failingResult);

        var (exitCode, _, _, stdErr) = Run(configurationService, StubExecutableResolver.Empty);

        exitCode.Should().Be(CcbStackExitCodes.InvalidConfiguration);
        stdErr.Should().Contain("Unsupported output format");
    }

    [Fact]
    public void Execute_UnexpectedException_ReturnsUnexpectedErrorExitCode()
    {
        var configurationService = FakeCcbStackConfigurationService.Throwing(new InvalidOperationException("boom"));

        var (exitCode, _, _, stdErr) = Run(configurationService, StubExecutableResolver.Empty);

        exitCode.Should().Be(CcbStackExitCodes.UnexpectedError);
        stdErr.Should().Contain("boom");
    }

    [Fact]
    public void Execute_Warnings_AreWrittenToStandardError_NotStandardOut()
    {
        var configuration = new CcbStackConfiguration("sonnet", @"C:\skills", new OutputConfiguration("text"));
        var warningResult = new CcbStackConfigurationResult(
            configuration,
            new Dictionary<string, ConfigurationValueOrigin>(),
            [new ConfigurationDiagnostic("CFG003", ConfigurationDiagnosticSeverity.Warning, "Unrecognized configuration property 'foo'.")]);
        var configurationService = FakeCcbStackConfigurationService.Returning(warningResult);

        var (exitCode, _, stdOut, stdErr) = Run(configurationService, StubExecutableResolver.Empty, "--json");

        exitCode.Should().Be(CcbStackExitCodes.Success);
        stdErr.Should().Contain("Unrecognized configuration property");
        var act = () => System.Text.Json.JsonDocument.Parse(stdOut);
        act.Should().NotThrow("stdout must remain valid JSON even when warnings were reported to stderr");
    }

    private static (int ExitCode, string TestConsoleOutput, string StdOut, string StdErr) Run(
        ICcbStackConfigurationService configurationService,
        IExecutableResolver executableResolver,
        params string[] args)
    {
        var testConsole = new TestConsole();

        var services = new ServiceCollection();
        services.AddSingleton(configurationService);
        services.AddSingleton(executableResolver);
        services.AddSingleton<IAnsiConsole>(testConsole);
        services.AddSingleton<ICcbStackJsonSerializer, CcbStackJsonSerializer>();
        services.AddSingleton<ICommandOutput, ConsoleCommandOutput>();

        var registrar = new TypeRegistrar(services);
        var tester = new CommandAppTester(registrar, console: testConsole);
        tester.SetDefaultCommand<CcbStack.Cli.Commands.ConfigCommand>();

        var originalOut = Console.Out;
        var originalError = Console.Error;
        var stdOutWriter = new StringWriter();
        var stdErrWriter = new StringWriter();

        CommandAppResult result;

        try
        {
            Console.SetOut(stdOutWriter);
            Console.SetError(stdErrWriter);
            result = tester.Run(args);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }

        return (result.ExitCode, testConsole.Output, stdOutWriter.ToString(), stdErrWriter.ToString());
    }
}
