using CcbStack.Cli.Commands;
using Spectre.Console.Cli;

namespace CcbStack.Cli;

/// <summary>
/// Builds and configures the root ccbstack <see cref="CommandApp"/>.
/// </summary>
/// <remarks>
/// <see cref="Configure"/> is exposed separately from <see cref="Create"/> so that tests can
/// apply the exact same command registration to a <c>CommandAppTester</c> without duplicating it.
/// </remarks>
public static class CcbStackApp
{
    /// <summary>
    /// Creates the ccbstack <see cref="CommandApp"/>, ready to run.
    /// </summary>
    public static CommandApp Create()
    {
        var app = new CommandApp();
        app.Configure(Configure);
        return app;
    }

    /// <summary>
    /// Registers ccbstack's commands and application-level settings.
    /// </summary>
    public static void Configure(IConfigurator config)
    {
        config.SetApplicationName("ccbstack");

        config.AddCommand<VersionCommand>("version")
            .WithDescription("Displays the ccbstack version, .NET runtime version, and operating system.");
    }
}
