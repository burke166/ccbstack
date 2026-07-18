using CcbStack.Cli.Commands;
using CcbStack.Cli.DependencyInjection;
using CcbStack.Cli.Output;
using CcbStack.Cli.Tests.TestSupport;
using CcbStack.Core.Checks;
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
public class DoctorCommandTests
{
    [Fact]
    public void Execute_AllChecksPassing_ReturnsSuccessExitCode()
    {
        var (exitCode, output, _, _) = Run(FakeDoctorService.AllPassingReport());

        exitCode.Should().Be(CcbStackExitCodes.Success);
        output.Should().Contain("Doctor Report");
        output.Should().Contain("Git repository");
    }

    [Fact]
    public void Execute_AnyFailedCheck_ReturnsCheckFailedExitCode()
    {
        var (exitCode, output, _, _) = Run(FakeDoctorService.ReportWithFailure());

        exitCode.Should().Be(CcbStackExitCodes.CheckFailed);
        output.Should().Contain("Repository supported");
    }

    [Fact]
    public void Execute_WarningsOnly_StillReturnsSuccessExitCode()
    {
        var (exitCode, output, _, _) = Run(FakeDoctorService.ReportWithWarningOnly());

        exitCode.Should().Be(CcbStackExitCodes.Success);
        output.Should().Contain("Working tree clean");
    }

    [Fact]
    public void Execute_JsonOutput_IsValidParsableJson()
    {
        var (exitCode, _, stdOut, _) = Run(FakeDoctorService.AllPassingReport(), args: ["--json"]);

        exitCode.Should().Be(CcbStackExitCodes.Success);
        var act = () => System.Text.Json.JsonDocument.Parse(stdOut);
        act.Should().NotThrow();
    }

    [Fact]
    public void Execute_UnexpectedException_ReturnsUnexpectedErrorExitCode()
    {
        var analyzer = FakeRepositoryAnalyzer.Throwing(new InvalidOperationException("boom"));

        var (exitCode, _, _, stdErr) = Run(FakeDoctorService.AllPassingReport(), analyzer: analyzer);

        exitCode.Should().Be(CcbStackExitCodes.UnexpectedError);
        stdErr.Should().Contain("boom");
    }

    private static (int ExitCode, string TestConsoleOutput, string StdOut, string StdErr) Run(
        DoctorReport report, IRepositoryAnalyzer? analyzer = null, params string[] args)
    {
        var testConsole = new TestConsole();

        var services = new ServiceCollection();
        services.AddSingleton(analyzer ?? FakeRepositoryAnalyzer.Returning(FakeRepositoryAnalyzer.DesktopDotNetResult()));
        services.AddSingleton<IDoctorService>(FakeDoctorService.Returning(report));
        services.AddSingleton<IRuntimeEnvironment, RuntimeEnvironment>();
        services.AddSingleton<IAnsiConsole>(testConsole);
        services.AddSingleton<ICcbStackJsonSerializer, CcbStackJsonSerializer>();
        services.AddSingleton<ICommandOutput, ConsoleCommandOutput>();

        var registrar = new TypeRegistrar(services);
        var tester = new CommandAppTester(registrar, console: testConsole);
        tester.SetDefaultCommand<DoctorCommand>();

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
