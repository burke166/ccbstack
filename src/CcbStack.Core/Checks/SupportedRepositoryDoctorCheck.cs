using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Checks;

/// <summary>The terminal "is this a repository ccbstack understands" check, run last by convention.</summary>
public sealed class SupportedRepositoryDoctorCheck : IDoctorCheck
{
    public DoctorCheckResult? Evaluate(RepositoryInfo repository)
    {
        var isSupported = repository.Applications.Any(a => a != ApplicationClassification.Unknown);

        return isSupported
            ? new DoctorCheckResult("Repository supported", DoctorCheckStatus.Passed)
            : new DoctorCheckResult("Repository supported", DoctorCheckStatus.Warning, "No recognized language, framework, or project type was detected.");
    }
}
