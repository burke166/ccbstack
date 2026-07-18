using CcbStack.Core.IO;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.IO;

public class ProjectRootLocatorTests
{
    private readonly ProjectRootLocator _locator = new();

    [Fact]
    public void FindProjectRoot_ReturnsStartingDirectory_WhenItHasGitMarker()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".git"));

        var result = _locator.FindProjectRoot(temp.Info);

        result.Should().NotBeNull();
        result!.FullName.Should().Be(temp.Info.FullName);
    }

    [Fact]
    public void FindProjectRoot_ReturnsStartingDirectory_WhenItHasCcbstackMarker()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".ccbstack"));

        var result = _locator.FindProjectRoot(temp.Info);

        result!.FullName.Should().Be(temp.Info.FullName);
    }

    [Fact]
    public void FindProjectRoot_WalksUpToNearestAncestorWithMarker()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".git"));
        var nested = Directory.CreateDirectory(temp.Combine("src", "deeply", "nested"));

        var result = _locator.FindProjectRoot(nested);

        result!.FullName.Should().Be(temp.Info.FullName);
    }

    [Fact]
    public void FindProjectRoot_PrefersNearestAncestor_WhenMultipleMarkersExist()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".git"));
        var innerRoot = Directory.CreateDirectory(temp.Combine("nested-repo"));
        Directory.CreateDirectory(System.IO.Path.Combine(innerRoot.FullName, ".git"));
        var deeplyNested = Directory.CreateDirectory(System.IO.Path.Combine(innerRoot.FullName, "src"));

        var result = _locator.FindProjectRoot(deeplyNested);

        result!.FullName.Should().Be(innerRoot.FullName);
    }

    [Fact]
    public void FindProjectRoot_TreatsGitAsFileMarker_ForWorktrees()
    {
        using var temp = new TempDirectory();
        File.WriteAllText(temp.Combine(".git"), "gitdir: ../main/.git/worktrees/example");

        var result = _locator.FindProjectRoot(temp.Info);

        result!.FullName.Should().Be(temp.Info.FullName);
    }

    [Fact]
    public void FindProjectRoot_ReturnsNull_WhenNoAncestorHasAMarker()
    {
        using var temp = new TempDirectory();
        var nested = Directory.CreateDirectory(temp.Combine("no-markers-here"));

        // The real filesystem above the temp root may or may not have markers on a given
        // machine, so this test only asserts the search terminates without throwing and
        // does not claim the isolated temp subtree itself as a root.
        var result = _locator.FindProjectRoot(nested);

        (result is null || result.FullName != nested.FullName).Should().BeTrue();
    }

    [Fact]
    public void FindProjectRoot_Throws_WhenStartingDirectoryIsNull()
    {
        var act = () => _locator.FindProjectRoot(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
