using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Checks;

/// <summary>Only applies once .NET projects have been detected — a non-.NET repository should not be warned about a missing solution.</summary>
public sealed class DotNetSolutionDoctorCheck : IDoctorCheck
{
    public DoctorCheckResult? Evaluate(RepositoryInfo repository)
    {
        if (!repository.DotNet.IsDetected)
        {
            return null;
        }

        return repository.DotNet.SolutionFiles.Count > 0
            ? new DoctorCheckResult(".NET solution detected", DoctorCheckStatus.Passed)
            : new DoctorCheckResult(".NET solution detected", DoctorCheckStatus.Warning, "Projects were found but no .sln file exists.");
    }
}
