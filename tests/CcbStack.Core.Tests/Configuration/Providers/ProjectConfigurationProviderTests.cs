using CcbStack.Core.Configuration;
using CcbStack.Core.Configuration.Providers;
using CcbStack.Core.IO;
using CcbStack.Core.Json;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Configuration.Providers;

public class ProjectConfigurationProviderTests
{
    [Fact]
    public async Task LoadAsync_ReturnsEmptySource_WhenProjectRootHasNoConfigFile()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".git"));
        var provider = CreateProvider();

        var source = await provider.LoadAsync(Context(temp.Path), CancellationToken.None);

        source.Diagnostics.Should().BeEmpty();
        source.Values.DefaultModel.IsSet.Should().BeFalse();
    }

    [Fact]
    public async Task LoadAsync_ReturnsEmptySource_WhenNoProjectRootIsFound()
    {
        using var temp = new TempDirectory();
        var nested = Directory.CreateDirectory(temp.Combine("no-markers"));
        var provider = CreateProvider();

        var act = async () => await provider.LoadAsync(Context(nested.FullName), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task LoadAsync_ReadsValuesFromProjectConfigFile()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".git"));
        WriteConfigFile(temp.Path, """{ "defaultModel": "opus" }""");
        var provider = CreateProvider();

        var source = await provider.LoadAsync(Context(temp.Path), CancellationToken.None);

        source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("opus"));
    }

    [Fact]
    public async Task LoadAsync_FindsProjectConfigFile_FromNestedStartingDirectory()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".git"));
        WriteConfigFile(temp.Path, """{ "defaultModel": "opus" }""");
        var nested = Directory.CreateDirectory(temp.Combine("src", "nested"));
        var provider = CreateProvider();

        var source = await provider.LoadAsync(Context(nested.FullName), CancellationToken.None);

        source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("opus"));
    }

    [Fact]
    public async Task LoadAsync_ReturnsErrorDiagnostic_WhenProjectConfigJsonIsMalformed()
    {
        using var temp = new TempDirectory();
        Directory.CreateDirectory(temp.Combine(".git"));
        WriteConfigFile(temp.Path, "not json at all");
        var provider = CreateProvider();

        var source = await provider.LoadAsync(Context(temp.Path), CancellationToken.None);

        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Error);
    }

    private static ProjectConfigurationProvider CreateProvider()
    {
        var runtimeEnvironment = new FakeRuntimeEnvironment();
        return new ProjectConfigurationProvider(
            new ProjectRootLocator(),
            new FileSystem(),
            new CcbStackJsonSerializer(),
            new ConfigurationPathExpander(),
            runtimeEnvironment);
    }

    private static ConfigurationProviderContext Context(string currentDirectory)
    {
        return new ConfigurationProviderContext(currentDirectory, new CommandLineConfigurationInput(null, null));
    }

    private static void WriteConfigFile(string projectRoot, string json)
    {
        var directory = Directory.CreateDirectory(Path.Combine(projectRoot, ".ccbstack"));
        File.WriteAllText(Path.Combine(directory.FullName, "config.json"), json);
    }
}
