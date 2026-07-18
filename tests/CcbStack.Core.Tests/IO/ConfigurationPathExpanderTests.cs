using CcbStack.Core.IO;
using CcbStack.Core.Tests.TestSupport;
using FluentAssertions;

namespace CcbStack.Core.Tests.IO;

public class ConfigurationPathExpanderTests
{
    private readonly ConfigurationPathExpander _expander = new();

    private readonly FakeRuntimeEnvironment _runtimeEnvironment = new()
    {
        UserProfileDirectory = @"C:\Users\test-user",
        AppDataDirectory = @"C:\Users\test-user\AppData\Roaming",
        LocalAppDataDirectory = @"C:\Users\test-user\AppData\Local",
    };

    [Fact]
    public void ExpandSubstitutions_ExpandsUserProfileToken()
    {
        var result = _expander.ExpandSubstitutions(@"%USERPROFILE%\.claude\skills", _runtimeEnvironment);

        result.Should().Be(@"C:\Users\test-user\.claude\skills");
    }

    [Fact]
    public void ExpandSubstitutions_ExpandsAppDataToken()
    {
        var result = _expander.ExpandSubstitutions(@"%APPDATA%\ccbstack", _runtimeEnvironment);

        result.Should().Be(@"C:\Users\test-user\AppData\Roaming\ccbstack");
    }

    [Fact]
    public void ExpandSubstitutions_ExpandsLocalAppDataToken()
    {
        var result = _expander.ExpandSubstitutions(@"%LOCALAPPDATA%\ccbstack", _runtimeEnvironment);

        result.Should().Be(@"C:\Users\test-user\AppData\Local\ccbstack");
    }

    [Fact]
    public void ExpandSubstitutions_ExpandsLeadingTilde()
    {
        var result = _expander.ExpandSubstitutions("~/.claude/skills", _runtimeEnvironment);

        result.Should().Be(@"C:\Users\test-user/.claude/skills");
    }

    [Fact]
    public void ExpandSubstitutions_MatchesTokensCaseInsensitively()
    {
        var result = _expander.ExpandSubstitutions(@"%userprofile%\skills", _runtimeEnvironment);

        result.Should().Be(@"C:\Users\test-user\skills");
    }

    [Fact]
    public void ExpandSubstitutions_LeavesPathWithoutTokensUnchanged()
    {
        var result = _expander.ExpandSubstitutions(@"C:\already\absolute\path", _runtimeEnvironment);

        result.Should().Be(@"C:\already\absolute\path");
    }

    [Fact]
    public void ResolveRelativePath_ResolvesAgainstSuppliedBaseDirectory()
    {
        var result = _expander.ResolveRelativePath(@"skills", @"C:\repo\.ccbstack");

        result.Should().Be(System.IO.Path.GetFullPath(@"C:\repo\.ccbstack\skills"));
    }

    [Fact]
    public void ResolveRelativePath_ReturnsRootedPathUnchanged()
    {
        var result = _expander.ResolveRelativePath(@"C:\already\rooted", @"C:\repo\.ccbstack");

        result.Should().Be(@"C:\already\rooted");
    }

    [Fact]
    public void ResolveRelativePath_HandlesBaseDirectoriesContainingSpaces()
    {
        var result = _expander.ResolveRelativePath(@"skills", @"C:\Program Files\ccbstack config");

        result.Should().Be(System.IO.Path.GetFullPath(@"C:\Program Files\ccbstack config\skills"));
    }

    [Fact]
    public void Expand_SubstitutesThenResolvesRelativeToBaseDirectory()
    {
        // %USERPROFILE% expansion makes this path rooted, so the base directory (the
        // project config file's directory) must not affect the result.
        var result = _expander.Expand(@"%USERPROFILE%\.claude\skills", @"C:\repo\.ccbstack", _runtimeEnvironment);

        result.Should().Be(@"C:\Users\test-user\.claude\skills");
    }

    [Fact]
    public void Expand_ResolvesRelativePathAgainstContainingFileDirectory()
    {
        var result = _expander.Expand("skills", @"C:\repo\.ccbstack", _runtimeEnvironment);

        result.Should().Be(System.IO.Path.GetFullPath(@"C:\repo\.ccbstack\skills"));
    }
}
