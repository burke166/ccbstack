using CcbStack.Core.Checks;
using CcbStack.Core.Repositories.Model;
using FluentAssertions;

namespace CcbStack.Core.Tests.Checks;

public class DoctorServiceTests
{
    [Fact]
    public void Evaluate_OmitsChecksThatReturnNull()
    {
        var service = new DoctorService([new AlwaysApplicableCheck(), new NeverApplicableCheck()]);

        var report = service.Evaluate(EmptyRepository());

        report.Checks.Should().ContainSingle();
    }

    [Fact]
    public void Evaluate_ReportsHasFailures_WhenAnyCheckFails()
    {
        var service = new DoctorService([new FailingCheck()]);

        var report = service.Evaluate(EmptyRepository());

        report.HasFailures.Should().BeTrue();
    }

    [Fact]
    public void Evaluate_DoesNotReportFailures_ForWarningsOnly()
    {
        var service = new DoctorService([new WarningCheck()]);

        var report = service.Evaluate(EmptyRepository());

        report.HasFailures.Should().BeFalse();
    }

    private static RepositoryInfo EmptyRepository() => new(
        @"C:\repo", GitInfo.NotAGitRepository, [], DotNetInfo.NotDetected, GoInfo.NotDetected, PowerShellInfo.NotDetected,
        [ApplicationClassification.Unknown], [], []);

    private sealed class AlwaysApplicableCheck : IDoctorCheck
    {
        public DoctorCheckResult? Evaluate(RepositoryInfo repository) => new("Always", DoctorCheckStatus.Passed);
    }

    private sealed class NeverApplicableCheck : IDoctorCheck
    {
        public DoctorCheckResult? Evaluate(RepositoryInfo repository) => null;
    }

    private sealed class FailingCheck : IDoctorCheck
    {
        public DoctorCheckResult? Evaluate(RepositoryInfo repository) => new("Failing", DoctorCheckStatus.Failed, "boom");
    }

    private sealed class WarningCheck : IDoctorCheck
    {
        public DoctorCheckResult? Evaluate(RepositoryInfo repository) => new("Warning", DoctorCheckStatus.Warning, "heads up");
    }
}
