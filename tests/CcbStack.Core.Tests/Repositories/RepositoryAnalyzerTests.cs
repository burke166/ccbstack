using CcbStack.Core.IO;
using CcbStack.Core.Repositories;
using CcbStack.Core.Repositories.Classification;
using CcbStack.Core.Repositories.Model;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Repositories;

/// <summary>
/// Exercises the real detector composition end-to-end against synthetic on-disk repositories,
/// so a regression in how <see cref="RepositoryAnalyzer"/> wires detectors together (as
/// opposed to a bug in one detector) shows up here.
/// </summary>
public class RepositoryAnalyzerTests
{
    [Fact]
    public async Task AnalyzeAsync_ClassifiesADesktopDotNetRepository_LikeMegaMekVersionManager()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".git"));
        Directory.CreateDirectory(temp.Combine("src", "App"));
        File.WriteAllText(temp.Combine("App.sln"), "solution");
        File.WriteAllText(temp.Combine("src", "App", "App.csproj"), """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <OutputType>WinExe</OutputType>
                <TargetFramework>net10.0-windows</TargetFramework>
                <UseWPF>true</UseWPF>
              </PropertyGroup>
            </Project>
            """);
        File.WriteAllText(temp.Combine("src", "App", "Program.cs"), "// entry point");

        var analyzer = CreateAnalyzer();

        var result = await analyzer.AnalyzeAsync(temp.Path, CancellationToken.None);

        result.RootPath.Should().Be(temp.Path);
        result.Languages.Should().Contain(RepositoryLanguage.CSharp);
        result.DotNet.SolutionFiles.Should().ContainSingle();
        result.DotNet.Projects.Should().ContainSingle(p => p.Kind == DotNetProjectKind.Desktop);
        result.Applications.Should().Contain(ApplicationClassification.DesktopApp);
        result.Go.HasGoMod.Should().BeFalse();
    }

    [Fact]
    public async Task AnalyzeAsync_DiscoversRootFromANestedStartingDirectory()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".git"));
        var nested = Directory.CreateDirectory(temp.Combine("src", "deeply", "nested"));
        File.WriteAllText(temp.Combine("go.mod"), "module example\n\ngo 1.22\n");

        var analyzer = CreateAnalyzer();

        var result = await analyzer.AnalyzeAsync(nested.FullName, CancellationToken.None);

        result.RootPath.Should().Be(temp.Path);
        result.Go.HasGoMod.Should().BeTrue();
    }

    [Fact]
    public async Task AnalyzeAsync_FallsBackToStartingDirectory_WhenNoRootMarkerExists()
    {
        // Uses a locator stub (rather than the real ProjectRootLocator) so this assertion is
        // immune to ambient ancestor markers on the machine running the test — see
        // ProjectRootLocatorTests' equivalent caveat.
        using var temp = new TempDirectory();
        File.WriteAllText(temp.Combine("README.md"), "no markers here");

        var analyzer = CreateAnalyzer(new NullProjectRootLocator());

        var result = await analyzer.AnalyzeAsync(temp.Path, CancellationToken.None);

        result.RootPath.Should().Be(temp.Path);
        result.Applications.Should().Equal(ApplicationClassification.Unknown);
    }

    private static RepositoryAnalyzer CreateAnalyzer(IProjectRootLocator? projectRootLocator = null)
    {
        var fileSystem = new FileSystem();
        var gitInspector = new GitInspector(FakeExecutableResolver.Empty, new FakeProcessRunner(), fileSystem);
        var classifier = new ApplicationClassifier(
        [
            new DotNetApplicationClassificationRule(),
            new GoApplicationClassificationRule(),
            new PowerShellApplicationClassificationRule(),
        ]);

        return new RepositoryAnalyzer(
            projectRootLocator ?? new ProjectRootLocator(),
            new RepositoryFileScanner(),
            gitInspector,
            new DotNetProjectDetector(fileSystem),
            new GoProjectDetector(fileSystem),
            new PowerShellProjectDetector(),
            new LanguageDetector(),
            classifier);
    }

    private sealed class NullProjectRootLocator : IProjectRootLocator
    {
        public DirectoryInfo? FindProjectRoot(DirectoryInfo startingDirectory) => null;
    }
}
