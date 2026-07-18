using CcbStack.Core.Execution;
using CcbStack.Core.Repositories;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Repositories;

public class GitInspectorTests
{
    private const string CleanStatus =
        "# branch.oid abc123\n# branch.head main\n";

    private const string DirtyStatus =
        "# branch.oid abc123\n# branch.head main\n" +
        "1 M. N... file1.txt\n" + // staged
        "1 .M N... file2.txt\n" + // modified, unstaged
        "1 MM N... file3.txt\n" + // staged and modified
        "? file4.txt\n" + // untracked
        "u UU N... file5.txt\n"; // unmerged, counts as modified

    private const string DetachedStatus =
        "# branch.oid abc123\n# branch.head (detached)\n";

    private const string EmptyRepoStatus =
        "# branch.oid (initial)\n# branch.head main\n";

    [Fact]
    public async Task InspectAsync_ReturnsNotAGitRepository_WhenNoGitMarkerExists()
    {
        var fileSystem = new FakeFileSystem();
        var inspector = new GitInspector(FakeExecutableResolver.Empty, new FakeProcessRunner(), fileSystem);

        var result = await inspector.InspectAsync(@"C:\repo", CancellationToken.None);

        result.IsGitRepository.Should().BeFalse();
    }

    [Fact]
    public async Task InspectAsync_ReturnsGitUnavailable_WhenGitMarkerExistsButGitIsNotOnPath()
    {
        var fileSystem = new FakeFileSystem().AddDirectory(@"C:\repo\.git");
        var inspector = new GitInspector(FakeExecutableResolver.Empty, new FakeProcessRunner(), fileSystem);

        var result = await inspector.InspectAsync(@"C:\repo", CancellationToken.None);

        result.IsGitRepository.Should().BeTrue();
        result.Branch.Should().BeNull();
    }

    [Fact]
    public async Task InspectAsync_ParsesCleanStatus()
    {
        var (inspector, _) = CreateInspector(CleanStatus);

        var result = await inspector.InspectAsync(@"C:\repo", CancellationToken.None);

        result.IsGitRepository.Should().BeTrue();
        result.Branch.Should().Be("main");
        result.IsDetachedHead.Should().BeFalse();
        result.IsDirty.Should().BeFalse();
        result.CommitHash.Should().Be("abc123");
        result.StagedFileCount.Should().Be(0);
        result.ModifiedFileCount.Should().Be(0);
        result.UntrackedFileCount.Should().Be(0);
    }

    [Fact]
    public async Task InspectAsync_ParsesDirtyStatusCounts()
    {
        var (inspector, _) = CreateInspector(DirtyStatus);

        var result = await inspector.InspectAsync(@"C:\repo", CancellationToken.None);

        result.IsDirty.Should().BeTrue();
        result.StagedFileCount.Should().Be(2); // file1 (staged only) + file3 (staged and modified)
        result.ModifiedFileCount.Should().Be(3); // file2 + file3 + file5 (unmerged)
        result.UntrackedFileCount.Should().Be(1);
    }

    [Fact]
    public async Task InspectAsync_DetectsDetachedHead()
    {
        var (inspector, _) = CreateInspector(DetachedStatus);

        var result = await inspector.InspectAsync(@"C:\repo", CancellationToken.None);

        result.IsDetachedHead.Should().BeTrue();
        result.Branch.Should().BeNull();
    }

    [Fact]
    public async Task InspectAsync_TreatsInitialOid_AsNoCommitYet()
    {
        var (inspector, _) = CreateInspector(EmptyRepoStatus);

        var result = await inspector.InspectAsync(@"C:\repo", CancellationToken.None);

        result.CommitHash.Should().BeNull();
    }

    [Fact]
    public async Task InspectAsync_ReadsRemoteOriginAndDefaultBranch_WhenAvailable()
    {
        var (inspector, processRunner) = CreateInspector(CleanStatus);
        processRunner.AddResult(["remote", "get-url", "origin"], new ProcessExecutionResult(0, "https://example.com/repo.git\n", string.Empty));
        processRunner.AddResult(["symbolic-ref", "--short", "refs/remotes/origin/HEAD"], new ProcessExecutionResult(0, "origin/main\n", string.Empty));

        var result = await inspector.InspectAsync(@"C:\repo", CancellationToken.None);

        result.RemoteOriginUrl.Should().Be("https://example.com/repo.git");
        result.DefaultBranch.Should().Be("main");
    }

    [Fact]
    public async Task InspectAsync_LeavesRemoteAndDefaultBranchNull_WhenNoRemoteConfigured()
    {
        var (inspector, processRunner) = CreateInspector(CleanStatus);
        processRunner.AddResult(["remote", "get-url", "origin"], new ProcessExecutionResult(1, string.Empty, "fatal: No such remote"));
        processRunner.AddResult(["symbolic-ref", "--short", "refs/remotes/origin/HEAD"], new ProcessExecutionResult(1, string.Empty, "fatal: not a symbolic ref"));

        var result = await inspector.InspectAsync(@"C:\repo", CancellationToken.None);

        result.RemoteOriginUrl.Should().BeNull();
        result.DefaultBranch.Should().BeNull();
    }

    [Fact]
    public async Task InspectAsync_ReturnsGitUnavailable_WhenStatusCommandFails()
    {
        var executableResolver = FakeExecutableResolver.Empty.Add("git", @"C:\tools\git.exe");
        var processRunner = new FakeProcessRunner()
            .AddResult(["status", "--porcelain=v2", "--branch"], new ProcessExecutionResult(128, string.Empty, "fatal: not a git repository"));
        var fileSystem = new FakeFileSystem().AddDirectory(@"C:\repo\.git");
        var inspector = new GitInspector(executableResolver, processRunner, fileSystem);

        var result = await inspector.InspectAsync(@"C:\repo", CancellationToken.None);

        result.IsGitRepository.Should().BeTrue();
        result.Branch.Should().BeNull();
    }

    private static (GitInspector Inspector, FakeProcessRunner ProcessRunner) CreateInspector(string statusOutput)
    {
        var executableResolver = FakeExecutableResolver.Empty.Add("git", @"C:\tools\git.exe");
        var processRunner = new FakeProcessRunner()
            .AddResult(["status", "--porcelain=v2", "--branch"], new ProcessExecutionResult(0, statusOutput, string.Empty));
        var fileSystem = new FakeFileSystem().AddDirectory(@"C:\repo\.git");
        var inspector = new GitInspector(executableResolver, processRunner, fileSystem);

        return (inspector, processRunner);
    }
}
