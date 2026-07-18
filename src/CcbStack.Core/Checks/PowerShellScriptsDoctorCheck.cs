using CcbStack.Core.Repositories.Model;

namespace CcbStack.Core.Checks;

public sealed class PowerShellScriptsDoctorCheck : IDoctorCheck
{
    public DoctorCheckResult? Evaluate(RepositoryInfo repository)
    {
        return repository.PowerShell.IsDetected
            ? new DoctorCheckResult("PowerShell scripts detected", DoctorCheckStatus.Passed)
            : null;
    }
}
