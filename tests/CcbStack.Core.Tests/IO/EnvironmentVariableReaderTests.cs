using CcbStack.Core.IO;
using FluentAssertions;

namespace CcbStack.Core.Tests.IO;

public class EnvironmentVariableReaderTests
{
    private readonly EnvironmentVariableReader _reader = new();

    [Fact]
    public void GetVariables_ReturnsNonEmptySnapshot()
    {
        var variables = _reader.GetVariables();

        variables.Should().NotBeEmpty();
    }

    [Fact]
    public void GetVariables_IncludesPath()
    {
        var variables = _reader.GetVariables();

        variables.Should().ContainKey("PATH");
    }

    [Fact]
    public void GetVariables_MatchesEnvironmentVariableNamesCaseInsensitively_OnWindows()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var variables = _reader.GetVariables();

        variables.TryGetValue("path", out var lower).Should().BeTrue();
        variables.TryGetValue("PATH", out var upper).Should().BeTrue();
        lower.Should().Be(upper);
    }
}
