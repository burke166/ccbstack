using CcbStack.Cli.Commands;
using CcbStack.Cli.DependencyInjection;
using CcbStack.Cli.Output;
using CcbStack.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
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
    /// Creates the ccbstack <see cref="CommandApp"/>, ready to run, wired to a DI container
    /// so commands (and their dependencies, like <see cref="ICcbStackConfigurationService"/>)
    /// are constructed through Spectre.Console.Cli's <see cref="ITypeRegistrar"/> integration
    /// seam rather than requiring parameterless constructors.
    /// </summary>
    public static CommandApp Create()
    {
        var registrar = new TypeRegistrar(BuildServices());
        var app = new CommandApp(registrar);
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

        config.AddCommand<ConfigCommand>("config")
            .WithDescription("Displays the effective ccbstack configuration.");
    }

    /// <summary>
    /// Builds the CLI's service collection: ccbstack's configuration system (from
    /// <c>CcbStack.Core</c>) plus CLI-only registrations. <see cref="IAnsiConsole"/> must be
    /// registered explicitly here — Spectre's own *default* registrar (used only when
    /// <see cref="CommandApp"/> is constructed with no registrar at all) resolves it
    /// implicitly, but that implicit resolution does not apply once a custom
    /// <see cref="ITypeRegistrar"/> like <see cref="TypeRegistrar"/> is supplied.
    /// </summary>
    private static ServiceCollection BuildServices()
    {
        var services = new ServiceCollection();

        services.AddCcbStackConfiguration();

        services.AddSingleton<IAnsiConsole>(AnsiConsole.Console);
        services.AddSingleton<ICommandOutput, ConsoleCommandOutput>();

        return services;
    }
}
