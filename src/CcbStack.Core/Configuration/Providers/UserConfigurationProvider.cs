using CcbStack.Core.IO;
using CcbStack.Core.Json;
using CcbStack.Core.Runtime;

namespace CcbStack.Core.Configuration.Providers;

/// <summary>
/// Reads <c>%USERPROFILE%\.ccbstack\config.json</c>. A missing file is normal and produces
/// an empty, successful source rather than a diagnostic.
/// </summary>
public sealed class UserConfigurationProvider : ICcbStackConfigurationProvider
{
    private const string ProviderName = "user";

    private readonly IRuntimeEnvironment _runtimeEnvironment;
    private readonly IFileSystem _fileSystem;
    private readonly ICcbStackJsonSerializer _jsonSerializer;
    private readonly ConfigurationPathExpander _pathExpander;

    public UserConfigurationProvider(
        IRuntimeEnvironment runtimeEnvironment,
        IFileSystem fileSystem,
        ICcbStackJsonSerializer jsonSerializer,
        ConfigurationPathExpander pathExpander)
    {
        _runtimeEnvironment = runtimeEnvironment;
        _fileSystem = fileSystem;
        _jsonSerializer = jsonSerializer;
        _pathExpander = pathExpander;
    }

    public ConfigurationLayer Layer => ConfigurationLayer.User;

    public ValueTask<CcbStackConfigurationSource> LoadAsync(ConfigurationProviderContext context, CancellationToken cancellationToken)
    {
        var filePath = Path.Combine(_runtimeEnvironment.UserProfileDirectory, ".ccbstack", "config.json");

        return ConfigFileLoader.LoadOptionalFileAsync(
            ProviderName,
            Layer,
            filePath,
            _fileSystem,
            _jsonSerializer,
            _pathExpander,
            _runtimeEnvironment,
            cancellationToken);
    }
}
