using CcbStack.Cli.Commands.Repo;
using CcbStack.Cli.DependencyInjection;
using CcbStack.Cli.Output;
using CcbStack.Cli.Tests.TestSupport;
using CcbStack.Core.Json;
using CcbStack.Core.Repositories;
using CcbStack.Core.Runtime;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Testing;
using Spectre.Console.Testing;

namespace CcbStack.Cli.Tests.Commands;

[Collection(TestSupport.ConsoleRedirectionCollection.Name)]
public class RepoInspectCommandTests
{
    [Fact]
    public void Execute_TextOutput_ContainsClassificationAndLanguages()
    {
        var analyzer = FakeRepositoryAnalyzer.Returning(FakeRepositoryAnalyzer.DesktopDotNetResult());

        var (exitCode, output, _, _) = Run(analyzer);

        exitCode.Should().Be(CcbStackExitCodes.Success);
        output.Should().Contain("Desktop App");
        output.Should().Contain("C#");
        output.Should().Contain("main");
    }

    [Fact]
    public void Execute_PassesPathOption_AsTheStartingDirectory()
    {
        var analyzer = FakeRepositoryAnalyzer.Returning(FakeRepositoryAnalyzer.DesktopDotNetResult());

        Run(analyzer, "--path", @"C:\some\repo");

        analyzer.LastStartingDirectory.Should().Be(@"C:\some\repo");
    }

    [Fact]
    public void Execute_JsonOutput_IsValidParsableJson()
    {
        var analyzer = FakeRepositoryAnalyzer.Returning(FakeRepositoryAnalyzer.DesktopDotNetResult());

        var (exitCode, _, stdOut, _) = Run(analyzer, "--json");

        exitCode.Should().Be(CcbStackExitCodes.Success);
        var act = () => System.Text.Json.JsonDocument.Parse(stdOut);
        act.Should().NotThrow();
    }

    [Fact]
    public void Execute_JsonOutput_ContainsExpectedFields()
    {
        var analyzer = FakeRepositoryAnalyzer.Returning(FakeRepositoryAnalyzer.DesktopDotNetResult(@"C:\repo"));

        var (_, _, stdOut, _) = Run(analyzer, "--json");

        using var document = System.Text.Json.JsonDocument.Parse(stdOut);
        var root = document.RootElement;

        root.GetProperty("rootPath").GetString().Should().Be(@"C:\repo");
        root.GetProperty("applications")[0].GetString().Should().Be("desktopApp");
    }

    [Fact]
    public void Execute_JsonOutput_IsWrittenToStandardOut_NotTheAnsiConsole()
    {
        var analyzer = FakeRepositoryAnalyzer.Returning(FakeRepositoryAnalyzer.DesktopDotNetResult());

        var (_, testConsoleOutput, stdOut, _) = Run(analyzer, "--json");

        stdOut.Should().Contain("rootPath");
        testConsoleOutput.Should().NotContain("rootPath");
    }

    [Fact]
    public void Execute_UnexpectedException_ReturnsUnexpectedErrorExitCode()
    {
        var analyzer = FakeRepositoryAnalyzer.Throwing(new InvalidOperationException("boom"));

        var (exitCode, _, _, stdErr) = Run(analyzer);

        exitCode.Should().Be(CcbStackExitCodes.UnexpectedError);
        stdErr.Should().Contain("boom");
    }

    private static (int ExitCode, string TestConsoleOutput, string StdOut, string StdErr) Run(
        IRepositoryAnalyzer repositoryAnalyzer, params string[] args)
    {
        var testConsole = new TestConsole();

        var services = new ServiceCollection();
        services.AddSingleton(repositoryAnalyzer);
        services.AddSingleton<IRuntimeEnvironment, RuntimeEnvironment>();
        services.AddSingleton<IAnsiConsole>(testConsole);
        services.AddSingleton<ICcbStackJsonSerializer, CcbStackJsonSerializer>();
        services.AddSingleton<ICommandOutput, ConsoleCommandOutput>();

        var registrar = new TypeRegistrar(services);
        var tester = new CommandAppTester(registrar, console: testConsole);
        tester.SetDefaultCommand<RepoInspectCommand>();

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
