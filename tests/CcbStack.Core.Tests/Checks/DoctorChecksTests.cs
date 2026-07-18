using CcbStack.Core.Checks;
using CcbStack.Core.Repositories.Model;
using FluentAssertions;

namespace CcbStack.Core.Tests.Checks;

public class DoctorChecksTests
{
    private static readonly GitInfo CleanGit = GitInfo.NotAGitRepository with { IsGitRepository = true };
    private static readonly GitInfo DirtyGit = CleanGit with { IsDirty = true };

    [Fact]
    public void GitRepositoryDoctorCheck_Passes_WhenGitRepositoryDetected()
    {
        var result = new GitRepositoryDoctorCheck().Evaluate(Repository(git: CleanGit));

        result!.Status.Should().Be(DoctorCheckStatus.Passed);
    }

    [Fact]
    public void GitRepositoryDoctorCheck_Warns_WhenNoGitRepositoryDetected()
    {
        var result = new GitRepositoryDoctorCheck().Evaluate(Repository(git: GitInfo.NotAGitRepository));

        result!.Status.Should().Be(DoctorCheckStatus.Warning);
    }

    [Fact]
    public void DotNetSolutionDoctorCheck_IsNotApplicable_WhenNoDotNetProjectsExist()
    {
        var result = new DotNetSolutionDoctorCheck().Evaluate(Repository(dotNet: DotNetInfo.NotDetected));

        result.Should().BeNull();
    }

    [Fact]
    public void DotNetSolutionDoctorCheck_Passes_WhenASolutionFileExists()
    {
        var dotNet = new DotNetInfo(["App.sln"], [new DotNetProjectInfo("App.csproj", true, [], DotNetProjectKind.Console)], false, false, false);

        var result = new DotNetSolutionDoctorCheck().Evaluate(Repository(dotNet: dotNet));

        result!.Status.Should().Be(DoctorCheckStatus.Passed);
    }

    [Fact]
    public void DotNetSolutionDoctorCheck_Warns_WhenProjectsExistWithoutASolution()
    {
        var dotNet = new DotNetInfo([], [new DotNetProjectInfo("App.csproj", true, [], DotNetProjectKind.Console)], false, false, false);

        var result = new DotNetSolutionDoctorCheck().Evaluate(Repository(dotNet: dotNet));

        result!.Status.Should().Be(DoctorCheckStatus.Warning);
    }

    [Fact]
    public void GoModuleDoctorCheck_IsNotApplicable_WhenNoGoModExists()
    {
        var result = new GoModuleDoctorCheck().Evaluate(Repository(go: GoInfo.NotDetected));

        result.Should().BeNull();
    }

    [Fact]
    public void GoModuleDoctorCheck_Passes_WhenModuleNameIsReadable()
    {
        var go = new GoInfo(true, "example", "1.22", true, false, false);

        var result = new GoModuleDoctorCheck().Evaluate(Repository(go: go));

        result!.Status.Should().Be(DoctorCheckStatus.Passed);
    }

    [Fact]
    public void PowerShellScriptsDoctorCheck_IsNotApplicable_WhenNoScriptsExist()
    {
        var result = new PowerShellScriptsDoctorCheck().Evaluate(Repository(powerShell: PowerShellInfo.NotDetected));

        result.Should().BeNull();
    }

    [Fact]
    public void PowerShellScriptsDoctorCheck_Passes_WhenScriptsExist()
    {
        var powerShell = new PowerShellInfo(1, 0, false);

        var result = new PowerShellScriptsDoctorCheck().Evaluate(Repository(powerShell: powerShell));

        result!.Status.Should().Be(DoctorCheckStatus.Passed);
    }

    [Fact]
    public void WorkingTreeCleanDoctorCheck_IsNotApplicable_WhenNotAGitRepository()
    {
        var result = new WorkingTreeCleanDoctorCheck().Evaluate(Repository(git: GitInfo.NotAGitRepository));

        result.Should().BeNull();
    }

    [Fact]
    public void WorkingTreeCleanDoctorCheck_Passes_WhenClean()
    {
        var result = new WorkingTreeCleanDoctorCheck().Evaluate(Repository(git: CleanGit));

        result!.Status.Should().Be(DoctorCheckStatus.Passed);
    }

    [Fact]
    public void WorkingTreeCleanDoctorCheck_Warns_WhenDirty()
    {
        var result = new WorkingTreeCleanDoctorCheck().Evaluate(Repository(git: DirtyGit));

        result!.Status.Should().Be(DoctorCheckStatus.Warning);
    }

    [Fact]
    public void SupportedRepositoryDoctorCheck_Passes_WhenAnyClassificationIsRecognized()
    {
        var result = new SupportedRepositoryDoctorCheck().Evaluate(Repository(applications: [ApplicationClassification.DesktopApp]));

        result!.Status.Should().Be(DoctorCheckStatus.Passed);
    }

    [Fact]
    public void SupportedRepositoryDoctorCheck_Warns_WhenOnlyUnknown()
    {
        var result = new SupportedRepositoryDoctorCheck().Evaluate(Repository(applications: [ApplicationClassification.Unknown]));

        result!.Status.Should().Be(DoctorCheckStatus.Warning);
    }

    private static RepositoryInfo Repository(
        GitInfo? git = null,
        DotNetInfo? dotNet = null,
        GoInfo? go = null,
        PowerShellInfo? powerShell = null,
        IReadOnlyList<ApplicationClassification>? applications = null)
    {
        return new RepositoryInfo(
            @"C:\repo",
            git ?? GitInfo.NotAGitRepository,
            [],
            dotNet ?? DotNetInfo.NotDetected,
            go ?? GoInfo.NotDetected,
            powerShell ?? PowerShellInfo.NotDetected,
            applications ?? [ApplicationClassification.Unknown],
            [],
            []);
    }
}
