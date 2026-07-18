using CcbStack.Core.Diagnostics;

namespace CcbStack.Core.Tests.Diagnostics;

public class VersionInfoProviderTests
{
    [Fact]
    public void GetVersionInfo_ReturnsPopulatedValues()
    {
        var provider = new VersionInfoProvider();

        var versionInfo = provider.GetVersionInfo();

        Assert.False(string.IsNullOrWhiteSpace(versionInfo.ApplicationVersion));
        Assert.False(string.IsNullOrWhiteSpace(versionInfo.RuntimeVersion));
        Assert.False(string.IsNullOrWhiteSpace(versionInfo.OperatingSystemDescription));
    }

    [Fact]
    public void GetVersionInfo_ApplicationVersionExcludesSourceControlMetadata()
    {
        var provider = new VersionInfoProvider();

        var versionInfo = provider.GetVersionInfo();

        Assert.DoesNotContain('+', versionInfo.ApplicationVersion);
    }

    [Fact]
    public void GetVersionInfo_ReturnsConsistentValuesAcrossCalls()
    {
        var provider = new VersionInfoProvider();

        var first = provider.GetVersionInfo();
        var second = provider.GetVersionInfo();

        Assert.Equal(first, second);
    }
}
