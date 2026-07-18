using CcbStack.Core.Configuration;
using CcbStack.Core.Configuration.Providers;
using CcbStack.Core.IO;
using CcbStack.Core.Json;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.Configuration.Providers;

public class UserConfigurationProviderTests
{
    [Fact]
    public async Task LoadAsync_ReturnsEmptySuccessfulSource_WhenFileIsMissing()
    {
        using var temp = new TempDirectory();
        var provider = CreateProvider(temp.Path);

        var source = await provider.LoadAsync(Context(temp), CancellationToken.None);

        source.Diagnostics.Should().BeEmpty();
        source.Values.DefaultModel.IsSet.Should().BeFalse();
    }

    [Fact]
    public async Task LoadAsync_ReadsValuesFromRealFile()
    {
        using var temp = new TempDirectory();
        WriteConfigFile(temp, """{ "defaultModel": "haiku", "output": { "format": "json" } }""");
        var provider = CreateProvider(temp.Path);

        var source = await provider.LoadAsync(Context(temp), CancellationToken.None);

        source.Diagnostics.Should().BeEmpty();
        source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("haiku"));
        source.Values.OutputFormat.Should().Be(OptionalValue<string>.Of("json"));
    }

    [Fact]
    public async Task LoadAsync_ReturnsErrorDiagnostic_WhenJsonIsMalformed()
    {
        using var temp = new TempDirectory();
        WriteConfigFile(temp, "{ not valid json");
        var provider = CreateProvider(temp.Path);

        var source = await provider.LoadAsync(Context(temp), CancellationToken.None);

        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Error);
        source.Values.DefaultModel.IsSet.Should().BeFalse();
    }

    [Fact]
    public async Task LoadAsync_MalformedJsonDiagnostic_IdentifiesTheFilePath()
    {
        using var temp = new TempDirectory();
        var path = WriteConfigFile(temp, "{ not valid json");
        var provider = CreateProvider(temp.Path);

        var source = await provider.LoadAsync(Context(temp), CancellationToken.None);

        source.Diagnostics.Single().Source.Should().Be(path);
    }

    [Fact]
    public async Task LoadAsync_ReturnsErrorDiagnostic_WhenFileIsLockedForExclusiveAccess()
    {
        using var temp = new TempDirectory();
        var path = WriteConfigFile(temp, """{ "defaultModel": "sonnet" }""");
        var provider = CreateProvider(temp.Path);

        await using var lockingStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

        var source = await provider.LoadAsync(Context(temp), CancellationToken.None);

        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Error);
    }

    [Fact]
    public async Task LoadAsync_CanReadReadOnlyFile()
    {
        using var temp = new TempDirectory();
        var path = WriteConfigFile(temp, """{ "defaultModel": "sonnet" }""");
        File.SetAttributes(path, FileAttributes.ReadOnly);
        var provider = CreateProvider(temp.Path);

        try
        {
            var source = await provider.LoadAsync(Context(temp), CancellationToken.None);

            source.Diagnostics.Should().BeEmpty();
            source.Values.DefaultModel.Should().Be(OptionalValue<string>.Of("sonnet"));
        }
        finally
        {
            File.SetAttributes(path, FileAttributes.Normal);
        }
    }

    [Fact]
    public async Task LoadAsync_ExpandsUserProfileSubstitution_InSkillsDirectory()
    {
        using var temp = new TempDirectory();
        WriteConfigFile(temp, """{ "skillsDirectory": "%USERPROFILE%\\custom-skills" }""");
        var provider = CreateProvider(temp.Path);

        var source = await provider.LoadAsync(Context(temp), CancellationToken.None);

        source.Values.SkillsDirectory.Value.Should().Be(Path.Combine(temp.Path, "custom-skills"));
    }

    [Fact]
    public async Task LoadAsync_ResolvesRelativeSkillsDirectory_RelativeToConfigFileDirectory()
    {
        using var temp = new TempDirectory();
        WriteConfigFile(temp, """{ "skillsDirectory": "relative-skills" }""");
        var provider = CreateProvider(temp.Path);

        var source = await provider.LoadAsync(Context(temp), CancellationToken.None);

        var expectedDirectory = Path.Combine(temp.Path, ".ccbstack");
        source.Values.SkillsDirectory.Value.Should().Be(Path.GetFullPath(Path.Combine(expectedDirectory, "relative-skills")));
    }

    [Fact]
    public async Task LoadAsync_UnknownProperty_ProducesWarningNotError()
    {
        using var temp = new TempDirectory();
        WriteConfigFile(temp, """{ "defaultmodell": "sonnet" }""");
        var provider = CreateProvider(temp.Path);

        var source = await provider.LoadAsync(Context(temp), CancellationToken.None);

        source.Diagnostics.Should().ContainSingle(d => d.Severity == ConfigurationDiagnosticSeverity.Warning);
        source.Diagnostics.Single().Message.Should().Contain("defaultModel");
    }

    private static UserConfigurationProvider CreateProvider(string userProfileDirectory)
    {
        var runtimeEnvironment = new FakeRuntimeEnvironment { UserProfileDirectory = userProfileDirectory };
        return new UserConfigurationProvider(runtimeEnvironment, new FileSystem(), new CcbStackJsonSerializer(), new ConfigurationPathExpander());
    }

    private static ConfigurationProviderContext Context(TempDirectory temp)
    {
        return new ConfigurationProviderContext(temp.Path, new CommandLineConfigurationInput(null, null));
    }

    private static string WriteConfigFile(TempDirectory temp, string json)
    {
        var directory = Directory.CreateDirectory(temp.Combine(".ccbstack"));
        var path = Path.Combine(directory.FullName, "config.json");
        File.WriteAllText(path, json);
        return path;
    }
}
