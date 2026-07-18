using CcbStack.Core.Configuration.Providers;
using CcbStack.Core.IO;
using CcbStack.Core.Json;
using CcbStack.Core.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace CcbStack.Core.Configuration;

/// <summary>
/// Registers the configuration system and its dependencies. Registering additional
/// <see cref="ICcbStackConfigurationProvider"/> implementations (from this method, a future
/// module, or a plugin) is the supported way to extend configuration loading — the merge
/// algorithm in <see cref="CcbStackConfigurationService"/> operates on whatever the DI
/// container resolves for <c>IEnumerable&lt;ICcbStackConfigurationProvider&gt;</c>, so no
/// core code changes as providers are added.
/// </summary>
public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddCcbStackConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IRuntimeEnvironment, RuntimeEnvironment>();
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<IEnvironmentVariableReader, EnvironmentVariableReader>();
        services.AddSingleton<IProjectRootLocator, ProjectRootLocator>();
        services.AddSingleton<IExecutableResolver, PathExecutableResolver>();
        services.AddSingleton<ConfigurationPathExpander>();
        services.AddSingleton<ICcbStackJsonSerializer, CcbStackJsonSerializer>();

        services.AddSingleton<ICcbStackConfigurationValidator, CcbStackConfigurationValidator>();
        services.AddSingleton<ICcbStackConfigurationService, CcbStackConfigurationService>();

        services.AddSingleton<ICcbStackConfigurationProvider, DefaultConfigurationProvider>();
        services.AddSingleton<ICcbStackConfigurationProvider, UserConfigurationProvider>();
        services.AddSingleton<ICcbStackConfigurationProvider, ProjectConfigurationProvider>();
        services.AddSingleton<ICcbStackConfigurationProvider, EnvironmentConfigurationProvider>();
        services.AddSingleton<ICcbStackConfigurationProvider, CommandLineConfigurationProvider>();

        return services;
    }
}
