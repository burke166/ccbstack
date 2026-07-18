using CcbStack.Core.Configuration;
using CcbStack.Core.IO;
using CcbStack.Core.Json;
using CcbStack.Core.Runtime;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CcbStack.Core.Tests.Configuration;

public class ConfigurationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddCcbStackConfiguration_RegistersAllFiveProviders()
    {
        var services = new ServiceCollection();

        services.AddCcbStackConfiguration();

        services.Count(d => d.ServiceType == typeof(ICcbStackConfigurationProvider)).Should().Be(5);
    }

    [Theory]
    [InlineData(typeof(ICcbStackConfigurationService))]
    [InlineData(typeof(ICcbStackConfigurationValidator))]
    [InlineData(typeof(IRuntimeEnvironment))]
    [InlineData(typeof(IFileSystem))]
    [InlineData(typeof(IEnvironmentVariableReader))]
    [InlineData(typeof(IProjectRootLocator))]
    [InlineData(typeof(IExecutableResolver))]
    [InlineData(typeof(ConfigurationPathExpander))]
    [InlineData(typeof(ICcbStackJsonSerializer))]
    public void AddCcbStackConfiguration_RegistersEachSupportingService(Type serviceType)
    {
        var services = new ServiceCollection();

        services.AddCcbStackConfiguration();

        services.Should().Contain(d => d.ServiceType == serviceType);
    }

    [Fact]
    public void AddCcbStackConfiguration_ReturnsTheSameServiceCollection_ForChaining()
    {
        var services = new ServiceCollection();

        var result = services.AddCcbStackConfiguration();

        result.Should().BeSameAs(services);
    }
}
