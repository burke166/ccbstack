using CcbStack.Core.Configuration;
using CcbStack.Core.Configuration.Providers;
using CcbStack.Core.IO;
using CcbStack.Core.Json;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Configuration.Providers;

public class CommandLineConfigurationProviderTests
{
    private readonly CcbStackJsonSerializer _jsonSerializer = new();
    private readonly ConfigurationPathExpander _pathExpander = new();
    private readonly FakeRuntimeEnvironment _runtimeEnvironment = new() { CurrentDirectory = @"C:\work" };

    [Fact]
    public void Layer_IsCommandLine_TheHighestPrecedenceLayer()
    {
        var provider = CreateProvider(new FakeFileSystem());

        provider.Layer.Should().Be(ConfigurationLayer.CommandLine);
        ((int)ConfigurationLayer.CommandLine).Should().BeGreaterThan((int)ConfigurationLayer.Environment);
    }

    [Fact]
    public async Task LoadAsync_ReturnsEmptySource_WhenNeitherOptionSupplied()
    {
        var provider = CreateProvider(new FakeFileSystem());

        var source = await provider.LoadAsync(Context(null, null), CancellationToken.None);

        source.Diagnostics.Should().BeEmpty();
        source.Values.DefaultModel.IsSet.Should().BeFalse();
    }

    [Fact]
    public async Task LoadAsync_ParsesInlineJson()
    {
        var provider = CreateProvider(new FakeFileSystem());

        var source = await provider.LoadAsync(Context("""{ "defaultModel": "sonnet" }""", null), CancellationToken.None);

        source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("sonnet"));
        source.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadAsync_InlineJson_RelativeSkillsDirectoryResolvesAgainstCurrentDirectory()
    {
        var provider = CreateProvider(new FakeFileSystem());

        var source = await provider.LoadAsync(Context("""{ "skillsDirectory": "skills" }""", null), CancellationToken.None);

        source.Values.SkillsDirectory.Value.Should().Be(Path.GetFullPath(Path.Combine(@"C:\work", "skills")));
    }

    [Fact]
    public async Task LoadAsync_MalformedInlineJson_ReturnsErrorDiagnostic()
    {
        var provider = CreateProvider(new FakeFileSystem());

        var source = await provider.LoadAsync(Context("{ not json", null), CancellationToken.None);

        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Error);
    }

    [Fact]
    public async Task LoadAsync_LoadsFromFile_WhenFilePathSupplied()
    {
        using var temp = new TempDirectory();
        var path = temp.Combine("temporary-config.json");
        File.WriteAllText(path, """{ "defaultModel": "opus" }""");
        var provider = CreateProvider(new FileSystem());

        var source = await provider.LoadAsync(Context(null, path), CancellationToken.None);

        source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("opus"));
        source.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadAsync_RelativeFilePath_ResolvesAgainstCurrentDirectory()
    {
        var fileSystem = new FakeFileSystem().AddFile(@"C:\work\temporary-config.json", """{ "defaultModel": "opus" }""");
        var provider = CreateProvider(fileSystem);

        var source = await provider.LoadAsync(Context(null, "temporary-config.json"), CancellationToken.None);

        source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("opus"));
    }

    [Fact]
    public async Task LoadAsync_MissingConfigFile_ReturnsErrorDiagnostic()
    {
        var provider = CreateProvider(new FakeFileSystem());

        var source = await provider.LoadAsync(Context(null, @"C:\work\missing.json"), CancellationToken.None);

        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Error);
        source.Diagnostics.Single().Message.Should().Contain("was not found");
    }

    [Fact]
    public async Task LoadAsync_UnreadableConfigFile_ReturnsErrorDiagnostic()
    {
        var fileSystem = new FakeFileSystem()
            .AddFile(@"C:\work\locked.json", "{}")
            .MarkUnreadable(@"C:\work\locked.json");
        var provider = CreateProvider(fileSystem);

        var source = await provider.LoadAsync(Context(null, @"C:\work\locked.json"), CancellationToken.None);

        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Error);
    }

    [Fact]
    public async Task LoadAsync_MalformedFileJson_ReturnsErrorDiagnostic()
    {
        var fileSystem = new FakeFileSystem().AddFile(@"C:\work\bad.json", "{ not json");
        var provider = CreateProvider(fileSystem);

        var source = await provider.LoadAsync(Context(null, @"C:\work\bad.json"), CancellationToken.None);

        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Error);
    }

    [Fact]
    public async Task LoadAsync_JsonTakesPrecedence_WhenBothOptionsSomehowSupplied()
    {
        var fileSystem = new FakeFileSystem().AddFile(@"C:\work\file.json", """{ "defaultModel": "from-file" }""");
        var provider = CreateProvider(fileSystem);

        var source = await provider.LoadAsync(
            Context("""{ "defaultModel": "from-json" }""", @"C:\work\file.json"), CancellationToken.None);

        source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("from-json"));
    }

    private CommandLineConfigurationProvider CreateProvider(IFileSystem fileSystem)
    {
        return new CommandLineConfigurationProvider(fileSystem, _jsonSerializer, _pathExpander, _runtimeEnvironment);
    }

    private static ConfigurationProviderContext Context(string? json, string? filePath)
    {
        return new ConfigurationProviderContext(@"C:\work", new CommandLineConfigurationInput(json, filePath));
    }
}
