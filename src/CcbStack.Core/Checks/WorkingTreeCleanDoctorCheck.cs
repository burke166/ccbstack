using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Checks;

public sealed class WorkingTreeCleanDoctorCheck : IDoctorCheck
{
    public DoctorCheckResult? Evaluate(RepositoryInfo repository)
    {
        if (!repository.Git.IsGitRepository)
        {
            return null;
        }

        return repository.Git.IsDirty
            ? new DoctorCheckResult("Working tree clean", DoctorCheckStatus.Warning, "Working tree has uncommitted changes.")
            : new DoctorCheckResult("Working tree clean", DoctorCheckStatus.Passed);
    }
}
