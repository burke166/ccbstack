using Microsoft.Extensions.DependencyInjection;

namespace CcbStack.Core.Checks;

/// <summary>Registers the doctor checks engine. Registering another <see cref="IDoctorCheck"/> is the supported way to add a check.</summary>
public static class ChecksServiceCollectionExtensions
{
    public static IServiceCollection AddCcbStackDoctorChecks(this IServiceCollection services)
    {
        services.AddSingleton<IDoctorCheck, GitRepositoryDoctorCheck>();
        services.AddSingleton<IDoctorCheck, DotNetSolutionDoctorCheck>();
        services.AddSingleton<IDoctorCheck, GoModuleDoctorCheck>();
        services.AddSingleton<IDoctorCheck, PowerShellScriptsDoctorCheck>();
        services.AddSingleton<IDoctorCheck, WorkingTreeCleanDoctorCheck>();
        services.AddSingleton<IDoctorCheck, SupportedRepositoryDoctorCheck>();

        services.AddSingleton<IDoctorService, DoctorService>();

        return services;
    }
}
