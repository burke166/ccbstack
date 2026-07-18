using CcbStack.Core.Execution;
using CcbStack.Core.IO;
using CcbStack.Core.Repositories.Classification;
using Microsoft.Extensions.DependencyInjection;

namespace CcbStack.Core.Repositories;

/// <summary>
/// Registers the repository analysis engine. Registering additional
/// <see cref="IApplicationClassificationRule"/> implementations is the supported way to teach
/// classification about new application shapes without touching
/// <see cref="Classification.ApplicationClassifier"/>.
/// </summary>
public static class RepositoryServiceCollectionExtensions
{
    public static IServiceCollection AddCcbStackRepositoryAnalysis(this IServiceCollection services)
    {
        services.AddSingleton<IProcessRunner, ProcessRunner>();
        services.AddSingleton<IRepositoryFileScanner, RepositoryFileScanner>();

        services.AddSingleton<IGitInspector, GitInspector>();
        services.AddSingleton<IDotNetProjectDetector, DotNetProjectDetector>();
        services.AddSingleton<IGoProjectDetector, GoProjectDetector>();
        services.AddSingleton<IPowerShellProjectDetector, PowerShellProjectDetector>();
        services.AddSingleton<ILanguageDetector, LanguageDetector>();

        services.AddSingleton<IApplicationClassificationRule, DotNetApplicationClassificationRule>();
        services.AddSingleton<IApplicationClassificationRule, GoApplicationClassificationRule>();
        services.AddSingleton<IApplicationClassificationRule, PowerShellApplicationClassificationRule>();
        services.AddSingleton<IApplicationClassifier, ApplicationClassifier>();

        services.AddSingleton<IRepositoryAnalyzer, RepositoryAnalyzer>();

        return services;
    }
}
