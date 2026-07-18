using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Checks;

public sealed class GoModuleDoctorCheck : IDoctorCheck
{
    public DoctorCheckResult? Evaluate(RepositoryInfo repository)
    {
        if (!repository.Go.HasGoMod)
        {
            return null;
        }

        return repository.Go.ModuleName is not null
            ? new DoctorCheckResult("Go module detected", DoctorCheckStatus.Passed)
            : new DoctorCheckResult("Go module detected", DoctorCheckStatus.Warning, "go.mod was found but its module name could not be read.");
    }
}
