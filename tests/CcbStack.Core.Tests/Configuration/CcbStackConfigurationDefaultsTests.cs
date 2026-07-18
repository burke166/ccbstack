using CcbStack.Core.Configuration;
using FluentAssertions;

namespace CcbStack.Core.Tests.Configuration;

public class CcbStackConfigurationDefaultsTests
{
    private const string UserProfileDirectory = @"C:\Users\test-user";

    [Fact]
    public void Create_ReturnsExpectedDefaultModel()
    {
        var values = CcbStackConfigurationDefaults.Create(UserProfileDirectory);

        values.DefaultModel.Should().Be(OptionalValue<string>.Of("sonnet"));
    }

    [Fact]
    public void Create_ReturnsExpectedOutputFormat()
    {
        var values = CcbStackConfigurationDefaults.Create(UserProfileDirectory);

        values.OutputFormat.Should().Be(OptionalValue<string>.Of("text"));
    }

    [Fact]
    public void Create_DerivesSkillsDirectoryFromSuppliedUserProfileDirectory()
    {
        var values = CcbStackConfigurationDefaults.Create(UserProfileDirectory);

        values.SkillsDirectory.IsSet.Should().BeTrue();
        values.SkillsDirectory.Value.Should().Be(Path.Combine(UserProfileDirectory, ".claude", "skills"));
    }

    [Fact]
    public void Create_DoesNotHardCodeAnAbsolutePath()
    {
        var values = CcbStackConfigurationDefaults.Create(@"C:\Users\someone-else");

        values.SkillsDirectory.Value.Should().StartWith(@"C:\Users\someone-else");
    }

    [Fact]
    public void Create_AllPropertiesAreSet()
    {
        var values = CcbStackConfigurationDefaults.Create(UserProfileDirectory);

        values.DefaultModel.IsSet.Should().BeTrue();
        values.SkillsDirectory.IsSet.Should().BeTrue();
        values.OutputFormat.IsSet.Should().BeTrue();
    }
}
