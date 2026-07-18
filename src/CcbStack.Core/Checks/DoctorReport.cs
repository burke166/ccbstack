namespace CcbStack.Core.Checks;

public sealed record DoctorReport(IReadOnlyList<DoctorCheckResult> Checks)
{
    public bool HasFailures => Checks.Any(c => c.Status == DoctorCheckStatus.Failed);
}
