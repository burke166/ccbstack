using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Checks;

public sealed class GitRepositoryDoctorCheck : IDoctorCheck
{
    public DoctorCheckResult? Evaluate(RepositoryInfo repository)
    {
        return repository.Git.IsGitRepository
            ? new DoctorCheckResult("Git repository", DoctorCheckStatus.Passed)
            : new DoctorCheckResult("Git repository", DoctorCheckStatus.Warning, "No git repository detected.");
    }
}
