using CcbStack.Core.Checks;
using CcbStack.Core.Repositories.Model;

namespace CcbStack.Cli.Tests.TestSupport;

public sealed class FakeDoctorService : IDoctorService
{
    private readonly Func<RepositoryInfo, DoctorReport> _evaluate;

    private FakeDoctorService(Func<RepositoryInfo, DoctorReport> evaluate)
    {
        _evaluate = evaluate;
    }

    public static FakeDoctorService Returning(DoctorReport result)
    {
        return new FakeDoctorService(_ => result);
    }

    public static DoctorReport AllPassingReport()
    {
        return new DoctorReport(
        [
            new DoctorCheckResult("Git repository", DoctorCheckStatus.Passed),
            new DoctorCheckResult(".NET solution detected", DoctorCheckStatus.Passed),
            new DoctorCheckResult("Repository supported", DoctorCheckStatus.Passed),
        ]);
    }

    public static DoctorReport ReportWithFailure()
    {
        return new DoctorReport(
        [
            new DoctorCheckResult("Git repository", DoctorCheckStatus.Passed),
            new DoctorCheckResult("Repository supported", DoctorCheckStatus.Failed, "No recognized language detected."),
        ]);
    }

    public static DoctorReport ReportWithWarningOnly()
    {
        return new DoctorReport(
        [
            new DoctorCheckResult("Git repository", DoctorCheckStatus.Passed),
            new DoctorCheckResult("Working tree clean", DoctorCheckStatus.Warning, "Working tree has uncommitted changes."),
        ]);
    }

    public DoctorReport Evaluate(RepositoryInfo repository) => _evaluate(repository);
}
