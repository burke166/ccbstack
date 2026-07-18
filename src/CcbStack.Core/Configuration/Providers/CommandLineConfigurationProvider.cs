using CcbStack.Core.IO;
using CcbStack.Core.Json;
using CcbStack.Core.Runtime;

namespace CcbStack.Core.Configuration.Providers;

/// <summary>
/// Loads temporary, non-persisted overrides supplied via <c>--config-json</c> or
/// <c>--config-file</c>. Highest precedence. Mutual exclusion of the two options is
/// validated by the command layer before this provider runs; if both are somehow present,
/// <c>--config-json</c> takes precedence deterministically. Unlike the user/project
/// providers, a <c>--config-file</c> target that does not exist is an error, since the user
/// explicitly pointed at it.
/// </summary>
public sealed class CommandLineConfigurationProvider : ICcbStackConfigurationProvider
{
    private const string ProviderName = "command-line";

    private readonly IFileSystem _fileSystem;
    private readonly ICcbStackJsonSerializer _jsonSerializer;
    private readonly ConfigurationPathExpander _pathExpander;
    private readonly IRuntimeEnvironment _runtimeEnvironment;

    public CommandLineConfigurationProvider(
        IFileSystem fileSystem,
        ICcbStackJsonSerializer jsonSerializer,
        ConfigurationPathExpander pathExpander,
        IRuntimeEnvironment runtimeEnvironment)
    {
        _fileSystem = fileSystem;
        _jsonSerializer = jsonSerializer;
        _pathExpander = pathExpander;
        _runtimeEnvironment = runtimeEnvironment;
    }

    public ConfigurationLayer Layer => ConfigurationLayer.CommandLine;

    public async ValueTask<CcbStackConfigurationSource> LoadAsync(ConfigurationProviderContext context, CancellationToken cancellationToken)
    {
        var input = context.CommandLineInput;

        if (input.Json is not null)
        {
            return ConfigFileLoader.ParseJson(
                ProviderName, Layer, sourcePath: null, input.Json, _jsonSerializer, _pathExpander, _runtimeEnvironment);
        }

        if (input.FilePath is null)
        {
            return ConfigFileLoader.EmptySource(ProviderName, Layer);
        }

        var filePath = _pathExpander.ResolveRelativePath(input.FilePath, context.CurrentDirectory);

        if (!_fileSystem.FileExists(filePath))
        {
            return ConfigFileLoader.ErrorSource(
                ProviderName,
                Layer,
                "CFG005",
                $"Configuration file '{filePath}' supplied via --config-file was not found.",
                filePath);
        }

        return await ConfigFileLoader.ReadAndParseAsync(
            ProviderName, Layer, filePath, _fileSystem, _jsonSerializer, _pathExpander, _runtimeEnvironment, cancellationToken)
            .ConfigureAwait(false);
    }
}
