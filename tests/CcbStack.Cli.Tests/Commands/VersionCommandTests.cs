using System.Text.Json;
using CcbStack.Cli;
using Spectre.Console.Cli.Testing;

namespace CcbStack.Cli.Tests.Commands;

public class VersionCommandTests
{
    private static CommandAppTester CreateApp()
    {
        var app = new CommandAppTester();
        app.Configure(CcbStackApp.Configure);
        return app;
    }

    [Fact]
    public void Version_IsRegistered_AndExecutesSuccessfully()
    {
        var app = CreateApp();

        var result = app.Run("version");

        Assert.Equal(0, result.ExitCode);
    }

    [Fact]
    public void Version_DefaultFormat_TextOutputIncludesApplicationVersion()
    {
        var app = CreateApp();

        var result = app.Run("version");

        Assert.Equal(0, result.ExitCode);
        Assert.Contains("ccbstack", result.Output);
    }

    [Fact]
    public void Version_JsonFormat_ProducesValidJsonWithExpectedFields()
    {
        var app = CreateApp();

        var result = app.Run("version", "--format", "json");

        Assert.Equal(0, result.ExitCode);

        using var document = JsonDocument.Parse(result.Output);
        var root = document.RootElement;

        Assert.True(root.TryGetProperty("applicationVersion", out _));
        Assert.True(root.TryGetProperty("runtimeVersion", out _));
        Assert.True(root.TryGetProperty("operatingSystemDescription", out _));
    }

    [Fact]
    public void Version_UnsupportedFormat_FailsValidationWithNonZeroExitCode()
    {
        var app = CreateApp();

        var result = app.Run("version", "--format", "xml");

        Assert.NotEqual(0, result.ExitCode);
    }

    [Fact]
    public void Help_ListsVersionCommand()
    {
        var app = CreateApp();

        var result = app.Run("--help");

        Assert.Contains("version", result.Output, StringComparison.OrdinalIgnoreCase);
    }
}
