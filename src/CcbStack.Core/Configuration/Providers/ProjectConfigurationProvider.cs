using CcbStack.Core.IO;
using CcbStack.Core.Json;
using CcbStack.Core.Runtime;

namespace CcbStack.Core.Configuration.Providers;

/// <summary>
/// Reads <c>&lt;project-root&gt;\.ccbstack\config.json</c>, where the project root is the
/// nearest ancestor of the current directory containing a <c>.git</c> or <c>.ccbstack</c>
/// marker (see <see cref="IProjectRootLocator"/>). Neither a missing project root nor a
/// missing config file is an error.
/// </summary>
public sealed class ProjectConfigurationProvider : ICcbStackConfigurationProvider
{
    private const string ProviderName = "project";

    private readonly IProjectRootLocator _projectRootLocator;
    private readonly IFileSystem _fileSystem;
    private readonly ICcbStackJsonSerializer _jsonSerializer;
    private readonly ConfigurationPathExpander _pathExpander;
    private readonly IRuntimeEnvironment _runtimeEnvironment;

    public ProjectConfigurationProvider(
        IProjectRootLocator projectRootLocator,
        IFileSystem fileSystem,
        ICcbStackJsonSerializer jsonSerializer,
        ConfigurationPathExpander pathExpander,
        IRuntimeEnvironment runtimeEnvironment)
    {
        _projectRootLocator = projectRootLocator;
        _fileSystem = fileSystem;
        _jsonSerializer = jsonSerializer;
        _pathExpander = pathExpander;
        _runtimeEnvironment = runtimeEnvironment;
    }

    public ConfigurationLayer Layer => ConfigurationLayer.Project;

    public ValueTask<CcbStackConfigurationSource> LoadAsync(ConfigurationProviderContext context, CancellationToken cancellationToken)
    {
        var projectRoot = _projectRootLocator.FindProjectRoot(new DirectoryInfo(context.CurrentDirectory));

        if (projectRoot is null)
        {
            return ValueTask.FromResult(ConfigFileLoader.EmptySource(ProviderName, Layer));
        }

        var filePath = Path.Combine(projectRoot.FullName, ".ccbstack", "config.json");

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
