using FluentAssertions;

namespace CcbStack.Cli.Tests;

/// <summary>
/// Exercises <see cref="CcbStackApp.Create"/>'s real DI wiring end-to-end. Unlike
/// <c>VersionCommandTests</c>/<c>ConfigCommandTests</c>, which build commands against
/// <c>Spectre.Console.Cli.Testing</c>'s own default (non-DI) test registrar, these tests
/// catch regressions specific to the production <see cref="CcbStackApp.Create"/> path — for
/// example, a service that resolves fine under Spectre's implicit default registrar but is
/// missing from <see cref="CcbStackApp"/>'s explicit DI registrations.
/// </summary>
[Collection(TestSupport.ConsoleRedirectionCollection.Name)]
public class CcbStackAppTests
{
    [Fact]
    public void Create_ResolvesAndRunsVersionCommand_WithoutThrowing()
    {
        var app = CcbStackApp.Create();

        var exitCode = RunSilently(app, "version");

        exitCode.Should().Be(0);
    }

    [Fact]
    public void Create_ResolvesAndRunsConfigCommand_WithoutThrowing()
    {
        var app = CcbStackApp.Create();

        var exitCode = RunSilently(app, "config", "--json");

        // A real end-to-end run against this machine's actual environment/filesystem should
        // never throw or hit the unexpected-error path; it may legitimately fail validation
        // (3) if this machine happens to have an invalid real user/project config file.
        exitCode.Should().NotBe(CcbStackExitCodes.UnexpectedError);
    }

    [Fact]
    public void Create_ResolvesAndRunsRepoInspectCommand_WithoutThrowing()
    {
        var app = CcbStackApp.Create();

        var exitCode = RunSilently(app, "repo", "inspect", "--json");

        exitCode.Should().Be(0);
    }

    [Fact]
    public void Create_ResolvesAndRunsDoctorCommand_WithoutThrowing()
    {
        var app = CcbStackApp.Create();

        var exitCode = RunSilently(app, "doctor", "--json");

        // May legitimately report exit code 1 (a check failed) depending on the machine's
        // real working-tree state; it must never hit the unexpected-error path.
        exitCode.Should().NotBe(CcbStackExitCodes.UnexpectedError);
    }

    private static int RunSilently(Spectre.Console.Cli.CommandApp app, params string[] args)
    {
        var originalOut = Console.Out;
        var originalError = Console.Error;

        try
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
            return app.Run(args);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalError);
        }
    }
}
